using System.Net.Sockets;

namespace Cobalt.Modbus.Connections;

/// <summary>
///     Handles a modbus TCP connection, and provides communication functionality
/// </summary>
/// <param name="ipAddress"></param>
/// <param name="port"></param>
public class TcpConnection(string ipAddress, int port)
{
    /// <summary>
    ///     For reconnection store the IP
    /// </summary>
    public string IpAddress { get; set; } = ipAddress;

    /// <summary>
    ///     For reconnection store the port
    /// </summary>
    public int Port { get; set; } = port;

    /// <summary>
    ///     Create the TCP Client
    /// </summary>
    public TcpClient TcpClient { get; } = new(ipAddress, port);

    /// <summary>
    ///     Read a given packet against a TCP modbus device
    /// </summary>
    /// <param name="request"></param>
    /// <returns>PDU Byte array response</returns>
    public async Task<byte[]> ReadPacket(byte[] request)
    {
        await using var stream = TcpClient.GetStream();
        await stream.WriteAsync(request);
        var responseBuffer = new byte[TcpClient.ReceiveBufferSize];
        if (stream.CanRead) await stream.ReadAsync(responseBuffer.AsMemory(0, TcpClient.ReceiveBufferSize));

        // Structure of bytes
        // MBAP Header (https://devicebase.net/en/schneider-electric-modbus-tcp/questions/what-is-a-mbap-header-and-what-is-it-for/38q)
        // Transaction ID [2 bytes]
        // Protocol ID [2 bytes]
        // Length (Unit ID + PDU Byte Array Length Length) [2 bytes]
        // Unit ID (1 byte)

        // Modbus PDU
        // Function code [1 byte]
        // Data [As much length as required] 
        // var transactionIdBytes = responseBuffer.Take(2).ToArray();
        //var protocolIdBytes = responseBuffer.Skip(2).Take(2).ToArray();
        var lengthBytes = responseBuffer.Skip(5).Take(1).ToArray();
        // var unitIdByte = responseBuffer.Skip(6).Take(1).ToArray().FirstOrDefault();

        var length = lengthBytes[0]; //BitConverter.ToUInt16(lengthBytes);
        var endOfPacket = length - 1;
        if (endOfPacket < 0) endOfPacket = 0;

        var pduBytes = new byte[endOfPacket];

        Array.Copy(responseBuffer, 7, pduBytes, 0, endOfPacket);

        var pduStringBytes = BitConverter.ToString(pduBytes).Replace("-", "");
        Console.WriteLine("PDU Response bytes");
        Console.WriteLine(pduStringBytes);

        return pduBytes;
    }
}