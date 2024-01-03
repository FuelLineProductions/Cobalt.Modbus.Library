# Modbus Communication protocol implementation in dotnet

## Functionality implemented using the following documentation: 
- https://modbus.org/docs/Modbus_Application_Protocol_V1_1b3.pdf
- https://modbus.org/docs/Modbus_Messaging_Implementation_Guide_V1_0b.pdf

# Feature List
- Modbus TCP
- Build PDU Requests
- Read PDU Responses
- Read Holding Registers
- Read Coils
- Read Discrete Inputs
- Write Single Coil
- Write Multiple Coils
- Write Single Register
- Write Multiple Registers
- Read Input Registers
- Arithmetic DataType Conversion with byte order configuration

## Project Contents

- Library (Cobalt.Modbus)
- Background worker service (Cobalt.Modbus.Reader)
- Web User interface to setup and access a device (Cobalt.Modbus.WebUI)

## Simulators Tested
- https://sourceforge.net/projects/pymodslave/ pymodslave

## Devices Tested
TODO
