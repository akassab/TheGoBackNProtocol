using A2Sender.services;

namespace A2Sender.models
{
    public class PacketStatus {

        public bool acknowledged;
        public readonly DateTime dateTimeSent;

        public PacketStatus () {
            this.acknowledged = false;
            this.dateTimeSent = DateTime.UtcNow;
        }

        public bool IsExpired() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                throw new Exception("PacketStatus: IsExpired(): Cannot check if sent packet is expired if console parameters is not set..");
            }
            return (DateTime.UtcNow - this.dateTimeSent).TotalMilliseconds < ConsoleParametersService.GetTimeOut();
        }
    }
}
