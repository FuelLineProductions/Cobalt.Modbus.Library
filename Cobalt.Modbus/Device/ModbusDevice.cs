using Cobalt.Modbus.Connections;

namespace Cobalt.Modbus.Device;

/// <summary>
///     A TCP based modbus device representation with actionable methods
/// </summary>
/// <param name="ipAddress"></param>
/// <param name="port"></param>
public class ModbusDevice(
    string ipAddress,
    int port,
    byte unitId,
    List<ModbusAccessor> accessors,
    string name,
    ushort protocolId = 0)
{
    /// <summary>
    ///     TCP Connection to the physical device
    /// </summary>
    public TcpConnection TcpConnection { get; set; } = new(ipAddress, port);

    /// <summary>
    ///     The Modbus Accessors, for example, coils, holding registers, discrete inputs
    /// </summary>
    public List<ModbusAccessor> Accessors { get; set; } = accessors;

    /// <summary>
    ///     The devices unit ID
    /// </summary>
    public byte UnitId { get; set; } = unitId;

    /// <summary>
    ///     Usually this is 0
    /// </summary>
    public ushort ProtocolId { get; set; } = protocolId;

    /// <summary>
    ///     Name the device for easier reference
    /// </summary>
    public string Name { get; set; } = name;
}