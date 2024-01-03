using Cobalt.Modbus.Device;

namespace Cobalt.Modbus.ProtocolDataUnit;

/// <summary>
///     Modbus Protocol Data Unit Request Builder
/// </summary>
public interface IModbusPdu
{
    /// <summary>
    ///     Build a request to perform over TCP
    /// </summary>
    /// <param name="accessor">The modbus accessor</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">A function cannot be performed with the given parameters</exception>
    /// <exception cref="ArgumentOutOfRangeException">A invalid function was given</exception>
    ModbusPduRequest BuildPduRequest(ModbusAccessor accessor);

    /// <summary>
    ///     Read the pdu response and return the read holding register values
    /// </summary>
    /// <param name="pduResponse"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Thrown when a modbus error is triggered on read</exception>
    /// <exception cref="InvalidOperationException">Thrown when an PDU response is not received</exception>
    ModbusPduResponseData ReadResponse(byte[] pduResponse);
}