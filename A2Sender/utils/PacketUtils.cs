using A2Sender.enums;
using A2Sender.models.packets;
using A2Sender.services;
using System.Net;
using System.Text;
namespace A2Sender.utils
{
    // PacketUtils: A singleton class containing utilities to create packets, encode/decode packets.
    public static class PacketUtils
    {
        // Factory(...): Factory method for creating a Packet of type Data/Sack/Eot. Returns null when it can't.
        public static Packet? Factory(TypeEnum type, uint sequenceNumber, uint length = 0, string data = "") {
            switch (type) {
                case TypeEnum.Data:
                    return new DataPacket(sequenceNumber, length, data);
                case TypeEnum.Sack:
                    return new SackPacket(sequenceNumber);
                case TypeEnum.Eot:
                    return new EotPacket(sequenceNumber);
                default:
                    return null;
            }
        }

        // Encode(packet): encodes a packet into an array of bytes.
        static public byte[] Encode(Packet packet) {
            // Adjust byte orders for integer fields
            byte[] typeLittenEndian = BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.type));
            byte[] typeBigEndian = new byte[4];
            Array.Copy(typeLittenEndian, 4, typeBigEndian, 0, 4);
            byte[] sequenceNumberLittleEndian =  BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.sequenceNumber));
            byte[] sequenceNumberBigEndian = new byte[4];
            Array.Copy(sequenceNumberLittleEndian, 4, sequenceNumberBigEndian, 0, 4);
            byte[] lengthLittleEndian = BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.length));
            byte[] lengthBigEndian = new byte[4];
            Array.Copy(lengthLittleEndian, 4, lengthBigEndian, 0, 4);
            // Encode string data
            var encodedData = Encoding.ASCII.GetBytes(packet.data);
            // Flatten
            var x =  new byte[][] {
               typeBigEndian,
               sequenceNumberBigEndian,
               lengthBigEndian,
               encodedData
            }.SelectMany(b => b).ToArray();
            return x;
        }

        // TryDecode(...): Tries to decode an array of bytes into a Packet. Returns true on success and false otherwise.
        static public bool TryDecode(byte[] packetBytes, out Packet? packet) {
            // Verify min number of bytes
            int nBytes = packetBytes.Length;
            if (!(nBytes > 12)) {
                StackTraceService.ConsoleLog($"Could not decode a packet with {nBytes} number of bytes.");
                packet = null;
                return false;
            }

            try {
                // Decode the type of packet
                byte[] typeBytes = new Byte[4];
                Array.Copy(packetBytes, 0, typeBytes, 0, 4);
                TypeEnum? type = TypeEnumUtils.UIntToTypeEnum(BitConverter.ToUInt32(typeBytes));
                if (type == null) {
                    StackTraceService.ConsoleLog("Could not determine type of the packet.");
                    packet = null;
                    return false;
                }
                // Decode the sequence number
                byte[] sequenceBytes = new Byte[4];
                Array.Copy(packetBytes, 4, sequenceBytes, 0, 4);
                uint sequenceNumber = BitConverter.ToUInt32(sequenceBytes);
                // Decode the length
                byte[] lengthBytes = new Byte[4];
                Array.Copy(packetBytes, 8, lengthBytes, 0, 4);
                uint length = BitConverter.ToUInt32(lengthBytes);
                // Decode the string data
                string data = Encoding.ASCII.GetString(packetBytes, 12, nBytes - 12);

                // Create the packet
                packet = Factory((TypeEnum) type, sequenceNumber, length, data); 
                return true;
            }
            catch (Exception e) {
                 StackTraceService.ConsoleLog("Could not decode a packet for some reason.");
                 packet = null;
                 return false;
            }
        }
    }
}
