using A2Sender.services;

namespace A2Sender.models
{
    public class SentPacketInfo {

        public bool acknowledged;
        public readonly DateTime dateTimeSent;

        public SentPacketInfo () {
            this.acknowledged = false;
            this.dateTimeSent = DateTime.UtcNow;
        }

        public bool IsExpired() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                throw new Exception("SentPacketInfo: IsExpired(): Cannot check if sent packet is expired if console parameters is not set..");
            }
            return (DateTime.UtcNow - this.dateTimeSent).TotalMilliseconds < ConsoleParametersService.GetTimeOut();
        }
    }
}
