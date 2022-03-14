using A2Sender.enums;

namespace A2Sender.models.packets
{
    public class EotPacket : Packet
    {
        public EotPacket() : base(TypeEnum.Eot) {}
    }
}
