using A2Receiver.enums;

namespace A2Receiver.models.packets
{
    // Is a Packet (inheritence).
    public class DataPacket : Packet
    {
        public DataPacket(uint sequenceNumber, uint length, string data) : base(TypeEnum.Data, sequenceNumber, length, data) {}
    }
}
