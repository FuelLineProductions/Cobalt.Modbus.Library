using Cobalt.Modbus.FunctionCodes;

namespace Cobalt.Modbus.Device;

/// <summary>
///     Modbus Accessor, for example holding register or coil or discrete input
/// </summary>
public class ModbusAccessor(
    ModbusFunctionCodes.FunctionCode functionCode,
    ushort startingRegisterAddress,
    string readableName,
    ushort countOrValue,
    bool reverseByteOrder,
    ushort? writeValue = null)
{
    public ModbusFunctionCodes.FunctionCode FunctionCode { get; set; } = functionCode;

    /// <summary>
    ///     Set the register address
    /// </summary>
    public ushort StartingRegisterAddress { get; set; } = startingRegisterAddress;

    /// <summary>
    ///     Value of registers to read. Or in case of write the value to write
    /// </summary>
    public ushort CountOrValue { get; set; } = countOrValue;

    /// <summary>
    ///     A user friendly readable name if one is set
    /// </summary>
    public string ReadableName { get; set; } = readableName;

    /// <summary>
    ///     Optional specific write value
    /// </summary>
    public ushort? WriteValue { get; set; } = writeValue;

    /// <summary>
    ///     Whether to reverse the byte order for reading from this accessor when reading data
    /// </summary>
    public bool ReverseResponseByteOrder { get; set; } = reverseByteOrder;
}