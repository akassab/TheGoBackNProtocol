using A2Sender.services;

namespace A2Sender
{
internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program started..."); 

            // verify/parse command line arguments.
            if (ConsoleParametersService.TryParseAndSetArgs(args))
            {
                // start new thread to listen for acks indefinitely
                ListenerService.ListenForAck();
                
                // try to send packets
                if (SenderService.TrySendPackets()) {
                    Console.WriteLine("Sent all packets!");
                }
                else {
                    //  todo:  IF THIS FAILS WE SHOULD CANCEL THE THREAD ****************
                    Console.WriteLine("Failed to send packets.");
                }

            }
            else {
                Console.WriteLine("Failed to parse console paramater(s).");
            }
        }
    }
}
