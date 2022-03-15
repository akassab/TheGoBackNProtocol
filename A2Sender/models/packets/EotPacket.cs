using A2Sender.enums;

namespace A2Sender.models.packets
{
    // EotPacket: Is a Packet (inheritence).
    public class EotPacket : Packet
    {
        public EotPacket(uint sequenceNumber) : base(TypeEnum.Eot, sequenceNumber) {}
    }
}
