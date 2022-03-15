using A2Sender.services;

namespace A2Sender.models
{
    // PacketStatus: Contains information about sent packets - whether they have been acknowledged or not and the
    //  time at which it was sent.
    //  - Used in a dictionary in the WindowService.
    public class PacketStatus {

        // Whether the packet has been acknowledged already or not.
        public bool acknowledged;
        // The DateTime the packet was sent (aprox).
        public readonly DateTime dateTimeSent;

        // Constructor.
        public PacketStatus () {
            this.acknowledged = false;
            this.dateTimeSent = DateTime.UtcNow;
        }

        // IsExpired(): Determines if the packet has expired or not.
        public bool IsExpired() {
            return (DateTime.UtcNow - this.dateTimeSent).TotalMilliseconds < ConsoleArgumentsService.GetTimeout();
        }
    }
}
