using A2Sender.enums;

namespace A2Sender.models.packets
{
    public class SackPacket : Packet
    {
        public SackPacket(uint sequenceNumber) : base(TypeEnum.Sack, sequenceNumber) {}
    }
}
