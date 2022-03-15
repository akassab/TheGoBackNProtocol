using A2Sender.enums;

namespace A2Sender.models.packets
{
    // DataPacket: Is a Packet (inheritence).
    public class DataPacket : Packet
    {
        public DataPacket(uint sequenceNumber, uint length, string data) : base(TypeEnum.Data, sequenceNumber, length, data) {}
    }
}
