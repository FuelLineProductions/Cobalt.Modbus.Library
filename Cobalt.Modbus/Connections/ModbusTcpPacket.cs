namespace Cobalt.Modbus.Connections;

/// <summary>
///     Modbus TCP Packet is the data to communicate with the device
/// </summary>
/// <param name="transactionId"></param>
/// <param name="unitId"></param>
/// <param name="pdu"></param>
/// <param name="protocolId"></param>
public class ModbusTcpPacket(ushort transactionId, byte unitId, byte[] pduRequest, ushort protocolId = 0)
{
    /// <summary>
    ///     Unique transaction identifier for this operation
    /// </summary>
    private ushort TransactionId { get; } = transactionId;

    /// <summary>
    ///     Unit ID for communication to the device
    /// </summary>
    private byte UnitId { get; } = unitId;

    /// <summary>
    ///     The Protocol Data Unit Information
    /// </summary>
    private byte[] PduRequest { get; } = pduRequest;

    /// <summary>
    ///     The protocol identifier, usually 0
    /// </summary>
    private ushort ProtocolId { get; } = protocolId;

    /// <summary>
    ///     Get this packet as a byte array for communication
    /// </summary>
    /// <returns></returns>
    public byte[] PacketAsByteArray()
    {
        // Structure of bytes
        // MBAP Header (https://devicebase.net/en/schneider-electric-modbus-tcp/questions/what-is-a-mbap-header-and-what-is-it-for/38q)
        // Transaction ID [2 bytes]
        // Protocol ID [2 bytes]
        // Length (Unit ID + PDU Byte Array Length Length) [2 bytes]
        // Unit ID (1 byte)

        // Modbus PDU
        // Function code [1 byte]
        // Data [As much length as required] 

        var length = (ushort)(PduRequest.Length + UnitId);
        // 6 refers to total required bytes as referenced in MBAP Header comment
        var buffer = new byte[length + 6];
        // right shift (The >> operator shifts its left-hand operand right by the number of bits defined by its right-hand operand.)
        buffer[0] = (byte)(TransactionId >> 8);
        buffer[1] = (byte)(TransactionId & 0xff);
        buffer[2] = (byte)(ProtocolId >> 8);
        buffer[3] = (byte)(ProtocolId & 0xff);
        buffer[4] = (byte)(length >> 8);
        buffer[5] = (byte)(length & 0xff);
        buffer[6] = UnitId;

        Array.Copy(PduRequest, 0, buffer, 7, PduRequest.Length);

        var requestString = BitConverter.ToString(buffer).Replace("-", "");
        Console.WriteLine("TCP Request bytes");
        Console.WriteLine(requestString);

        return buffer;
    }
}