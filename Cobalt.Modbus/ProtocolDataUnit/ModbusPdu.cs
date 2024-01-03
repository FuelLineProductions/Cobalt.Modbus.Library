using Cobalt.Modbus.Device;
using FunctionCode = Cobalt.Modbus.FunctionCodes.ModbusFunctionCodes.FunctionCode;
using ModbusError = Cobalt.Modbus.FunctionCodes.ModbusFunctionCodes.ModbusErrors;

namespace Cobalt.Modbus.ProtocolDataUnit;

/// <summary>
///     Modbus Protocol Data Unit Request Builder
/// </summary>
public class ModbusPdu : IModbusPdu
{
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <inheritdoc />
    public ModbusPduRequest BuildPduRequest(ModbusAccessor accessor)
    {
        // Coils
        // Function code 1 Byte 0x01
        // Starting Address 2 Bytes 0x0000 to 0xFFFF
        // Quantity of coils 2 Bytes 1 to 2000 (0x7D0)

        // Read holding register
        // Function code 1 Byte 0x03
        // Starting Address 2 Bytes 0x0000 to 0xFFFF
        // Quantity of Registers 2 Bytes 1 to 125 (0x7D)


        // address must be between 0 and 255. Total request must not exceed 255
        if (accessor.StartingRegisterAddress > 255 ||
            accessor.StartingRegisterAddress + accessor.CountOrValue > 255)
            throw new InvalidOperationException();

        byte[] singleWriterBytes = [];
        byte[] multipleCoilData = [];
        byte[] multipleWriteRegisters = [];
        // Function validation checks
        switch (accessor.FunctionCode)
        {
            case FunctionCode.ReadCoils:
            case FunctionCode.ReadDiscreteInputs:
            {
                if (accessor.CountOrValue is < 1 or > 2000)
                    throw new InvalidOperationException(
                        $"{nameof(accessor.FunctionCode)} span is out of {nameof(accessor.FunctionCode)} address range");

                break;
            }
            case FunctionCode.ReadHoldingRegisters:
            {
                // count must be between 1 and 125
                if (accessor.CountOrValue is < 1 or > 125)
                    throw new InvalidOperationException("Holding register span is out of register range");

                break;
            }
            case FunctionCode.WriteSingleCoil:
            {
                if (accessor.CountOrValue is < 1 or > 2000)
                    throw new InvalidOperationException(
                        $"{nameof(accessor.FunctionCode)} write address is out of {nameof(accessor.FunctionCode)} address range");

                singleWriterBytes = accessor.CountOrValue switch
                {
                    > 0xFF00 => throw new InvalidOperationException(
                        $"Write single coil, value {accessor.CountOrValue} to write is out of range expected a value between 0 and 255"),
                    // Can only be on or off
                    // On
                    >= 1 => new byte[] { 0xff, 0x00 },
                    // Off
                    _ => new byte[] { 0x00, 0x00 }
                };

                break;
            }
            case FunctionCode.WriteMultipleCoils:
            {
                if (accessor.StartingRegisterAddress > 0xFFFF || accessor.CountOrValue is < 1 or > 45063)
                    throw new InvalidOperationException(
                        $"{accessor.FunctionCode} write address or write value is out of {accessor.FunctionCode} range");

                // n = Quantity of Outputs / 8, if the remainder is different of 0 => N = N+1
                var rem = Math.DivRem((int)accessor.CountOrValue, 8);
                var n = rem.Quotient;
                if (rem.Quotient == 0) n += 1;

                var byteCount = (byte)n;
                var outputsValue = (byte)(n * 0x01);
                multipleCoilData = new[]
                    {
                        (byte)accessor.FunctionCode,
                        (byte)(accessor.StartingRegisterAddress >> 8),
                        (byte)(accessor.StartingRegisterAddress & 0xff),
                        (byte)(accessor.CountOrValue >> 8),
                        (byte)(accessor.CountOrValue & 0xff),
                        byteCount,
                        outputsValue
                    }
                    ;

                break;
            }
            case FunctionCode.ReadInputRegister:
            {
                if (accessor.StartingRegisterAddress > 0xFFFF || accessor.CountOrValue is < 1 or > 0x007D)
                    throw new InvalidOperationException(
                        $"{accessor.FunctionCode} write address or write value is out of {accessor.FunctionCode} range");
                break;
            }
            case FunctionCode.WriteSingleRegister:
            {
                if (accessor.StartingRegisterAddress is > 65535 or < 1 ||
                    accessor.CountOrValue is > 65535 or < 1)
                    throw new InvalidOperationException(
                        $"{accessor.FunctionCode} write address or write value is out of {accessor.FunctionCode} range");

                singleWriterBytes = BitConverter.GetBytes(accessor.CountOrValue).Reverse().ToArray();
                break;
            }
            case FunctionCode.WriteMultipleRegisters:
            {
                if (accessor.StartingRegisterAddress is > 0xFFFF or < 1 ||
                    accessor.CountOrValue is > 0x007B or < 1 || !accessor.WriteValue.HasValue)
                    throw new InvalidOperationException(
                        $"{accessor.FunctionCode} write address or write value is out of {accessor.FunctionCode} range");

                var byteCount = (byte)(accessor.CountOrValue * 2);

                multipleWriteRegisters = new[]
                {
                    (byte)accessor.FunctionCode,
                    (byte)(accessor.StartingRegisterAddress >> 8),
                    (byte)(accessor.StartingRegisterAddress & 0xff),
                    (byte)(accessor.CountOrValue >> 8),
                    (byte)(accessor.CountOrValue & 0xff),
                    byteCount,
                    (byte)(accessor.WriteValue.Value >> 8),
                    (byte)(accessor.WriteValue.Value & 0xff)
                };
                break;
            }
            case FunctionCode.ReadWriteMultipleRegisters:
                throw new NotImplementedException();
            case FunctionCode.MaskWriteRegister:
                throw new NotImplementedException();
            case FunctionCode.ReadFifoQueue:
                throw new NotImplementedException();
            case FunctionCode.ErrorReadingHoldingRegisters:
            case FunctionCode.ErrorReadingCoils:
            case FunctionCode.ErrorReadingInputRegistersOrDiscreteInputs:
            case FunctionCode.ErrorWritingSingleCoil:
            case FunctionCode.ErrorWritingSingleRegister:
            case FunctionCode.ErrorWritingMultipleCoils:
            case FunctionCode.ErrorWritingMultipleRegisters:
                throw new InvalidOperationException("Cannot perform a function on error code");

            default:
                throw new ArgumentOutOfRangeException(nameof(accessor.FunctionCode), accessor.FunctionCode,
                    "No valid function code provided");
        }


        // Total Bytes in array 1(function code), 2 ushort(starting address), 2 ushort(registerSpan)

        var request = new byte[5];
        // if multiple coil then already built, else use the generic
        if (multipleCoilData.Length != 0)
        {
            request = multipleCoilData;
        }
        // if multiple registers then already built, else use the generic
        else if (multipleWriteRegisters.Length != 0)
        {
            request = multipleWriteRegisters;
        }
        else
        {
            request[0] = (byte)accessor.FunctionCode;
            request[1] = (byte)(accessor.StartingRegisterAddress >> 8);
            request[2] = (byte)(accessor.StartingRegisterAddress & 0xff);

            // Check for write value. This works for single writes only
            if (singleWriterBytes.Length == 2)
            {
                request[3] = singleWriterBytes[0];
                request[4] = singleWriterBytes[1];
            }
            else
            {
                request[3] = (byte)(accessor.CountOrValue >> 8);
                request[4] = (byte)(accessor.CountOrValue & 0xff);
            }
        }

        return new ModbusPduRequest(accessor.FunctionCode, request);
    }

    /// <inheritdoc />
    public ModbusPduResponseData ReadResponse(byte[] pduResponse)
    {
        if (pduResponse.Length == 0) throw new InvalidOperationException("Failed to get pdu response from sensor");

        // function code is position 0, as a single byte
        var functionCode = pduResponse[0];
        // byte count or error code is position 1, as a single byte
        var byteCount = pduResponse[1];
        // function code is error code and known exception is present
        if (functionCode is (byte)FunctionCode.ErrorReadingHoldingRegisters or (byte)FunctionCode.ErrorReadingCoils
                or (byte)FunctionCode.ErrorReadingInputRegistersOrDiscreteInputs
                or (byte)FunctionCode.ErrorWritingSingleCoil
                or (byte)FunctionCode.ErrorWritingSingleRegister
                or (byte)FunctionCode.ErrorWritingMultipleCoils
                or (byte)FunctionCode.ErrorWritingMultipleRegisters &&
            byteCount is (byte)ModbusError.IllegalDataValue or (byte)ModbusError.ServerDeviceFailure
                or (byte)ModbusError.IllegalFunction or (byte)ModbusError.IllegalDataAddress
           )
            throw new InvalidDataException($"Modbus error: {byteCount} :{
                Enum.GetName((ModbusError)byteCount)}");

        // Write is different output to the readers
        var response = functionCode is (byte)FunctionCode.WriteSingleCoil or (byte)FunctionCode.WriteSingleRegister
            or (byte)FunctionCode.WriteMultipleCoils
            or (byte)FunctionCode.WriteMultipleRegisters
            ? [pduResponse[3], pduResponse[4]]
            :
            // Skip first two positions, take rest of the array this is the case for most readers
            pduResponse.Skip(2).Take(byteCount).ToArray();


        // Generate a new holding register response
        var readings = new ModbusPduResponseData((FunctionCode)functionCode,
            response);
        // Convert to data types;
        readings.ConvertContentToType();
        return readings;
    }
}