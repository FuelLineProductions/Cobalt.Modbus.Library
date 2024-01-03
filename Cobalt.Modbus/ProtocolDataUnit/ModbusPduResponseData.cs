using System.Text;
using Cobalt.Modbus.FunctionCodes;

namespace Cobalt.Modbus.ProtocolDataUnit;

/// <summary>
///     Raw response for reading multiple holding registers
/// </summary>
/// <param name="functionCode">The function code. This should 0x03 (read holding registers)</param>
/// <param name="count">Total amount of bytes</param>
/// <param name="contentResponse">Raw content response. This should be converted to the expected type</param>
public class ModbusPduResponseData(ModbusFunctionCodes.FunctionCode functionCode, byte[] contentResponse)
{
    public ModbusFunctionCodes.FunctionCode FunctionCode = functionCode;
    public byte[] Content { get; set; } = contentResponse;
    public bool? BoolValue { get; set; }
    public byte? ByteValue { get; set; }
    public ushort? UshortValue { get; set; }
    public short? ShortValue { get; set; }
    public uint? UintValue { get; set; }
    public int? IntValue { get; set; }
    public float? FloatValue { get; set; }
    public double? DoubleValue { get; set; }
    public long? LongValue { get; set; }
    public ulong? UlongValue { get; set; }
    public string? StringValue { get; set; }

    /// <summary>
    ///     Converts the received content to the best matched data type, if there is a data type lower, it will also convert to
    ///     that
    /// </summary>
    /// <param name="byteFlip">Whether to reverse the byte order, default is true</param>
    public void ConvertContentToType(bool byteFlip = true)
    {
        if (byteFlip) Content = contentResponse.Reverse().ToArray();
        switch (Content.Length)
        {
            case 1:
            {
                var byteContent = Content[0];
                ByteConverter(byteContent);
                break;
            }
            case 2:
            {
                var byteContent = Content[0];
                ByteConverter(byteContent);

                var twoBytes = new[] { Content[0], Content[1] };
                TwoByteConverter(twoBytes);

                break;
            }
            case 4:
            {
                var byteContent = Content[0];
                ByteConverter(byteContent);

                var twoBytes = new[] { Content[0], Content[1] };
                TwoByteConverter(twoBytes);

                var fourBytes = new[] { Content[0], Content[1], Content[2], Content[3] };
                FourByteConverter(fourBytes);

                break;
            }
            case 8:
            {
                var byteContent = Content[0];
                ByteConverter(byteContent);

                var twoBytes = new[] { Content[0], Content[1] };
                TwoByteConverter(twoBytes);

                var fourBytes = new[] { Content[0], Content[1], Content[2], Content[3] };
                FourByteConverter(fourBytes);

                var eightBytes = Content.Take(8).ToArray();
                EightByteConverter(eightBytes);

                break;
            }
            default:
            {
                var stringConverted = Encoding.UTF8.GetString(Content);
                StringValue = stringConverted;
                break;
            }
        }
    }

    private void ByteConverter(byte byteContent)
    {
        ByteValue = byteContent;
        if (byteContent.Equals(1) || byteContent.Equals(0)) BoolValue = byteContent == 1;
    }

    private void TwoByteConverter(byte[] bytes)
    {
        var uConverted = BitConverter.ToUInt16(bytes);
        UshortValue = uConverted;
        var converted = BitConverter.ToInt16(bytes);
        ShortValue = converted;
    }

    private void FourByteConverter(byte[] bytes)
    {
        var uConverted = BitConverter.ToUInt32(bytes);
        UintValue = uConverted;
        var converted = BitConverter.ToInt32(bytes);
        IntValue = converted;
        var floatConverted = BitConverter.ToSingle(bytes);
        FloatValue = floatConverted;
    }

    private void EightByteConverter(byte[] bytes)
    {
        var doubleConverted = BitConverter.ToDouble(bytes);
        DoubleValue = doubleConverted;
        var longConverted = BitConverter.ToInt64(bytes);
        LongValue = longConverted;
        var ulongConverted = BitConverter.ToUInt64(bytes);
        UlongValue = ulongConverted;
    }
}