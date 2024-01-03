using Cobalt.Modbus.FunctionCodes;

namespace Cobalt.Modbus.ProtocolDataUnit;

/// <summary>
///     A request with the function code preserved
/// </summary>
/// <param name="functionCode"></param>
/// <param name="content"></param>
public class ModbusPduRequest(ModbusFunctionCodes.FunctionCode functionCode, byte[] content)
{
    public ModbusFunctionCodes.FunctionCode FunctionCode { get; set; } = functionCode;
    public byte[] RequestData { get; set; } = content;
}