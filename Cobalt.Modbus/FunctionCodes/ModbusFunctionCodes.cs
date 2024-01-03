namespace Cobalt.Modbus.FunctionCodes;

/// <summary>
///     Modbus instruction codes as defined in the standard
/// </summary>
public static class ModbusFunctionCodes
{
    //https://en.wikipedia.org/wiki/Modbus see function code table
    // at the moment we only care amount reading holding register feature. More will be added as we add more features
    public enum FunctionCode : byte
    {
        // Bit Access
        ReadDiscreteInputs = 0x02,
        ReadCoils = 0x01,
        WriteSingleCoil = 0x05,
        WriteMultipleCoils = 0x0F,

        // 16 bit access
        //  - Physical Input Registers
        ReadInputRegister = 0x04,

        //  - Internal registers or physical output registers
        ReadHoldingRegisters = 0x03,
        WriteSingleRegister = 0x06,
        WriteMultipleRegisters = 0x10,
        ReadWriteMultipleRegisters = 0x17,
        MaskWriteRegister = 0x16,
        ReadFifoQueue = 0x18, // First in First Out Queue,

        ErrorReadingHoldingRegisters = 0x83,
        ErrorReadingCoils = ReadCoils + 0x80,

        //ErrorReadingDiscreteInputs = ReadDiscreteInputs + 0x82,
        ErrorReadingInputRegistersOrDiscreteInputs = 0x84,
        ErrorWritingSingleCoil = 0x85,
        ErrorWritingSingleRegister = 0x86,
        ErrorWritingMultipleCoils = 0x8F,
        ErrorWritingMultipleRegisters = 0x90
    }

    public enum ModbusErrors : byte
    {
        IllegalFunction = 0x01,
        IllegalDataAddress = 0x02,
        IllegalDataValue = 0x03,
        ServerDeviceFailure = 0x04,
        Acknowledge = 0x05,
        ServiceDeviceBusy = 0x06,
        MemoryParityError = 0x08
    }
}