using A2Sender.services;

namespace A2Sender
{
    // Beginning of the program.
    internal class Program
        {
            public static void Main(string[] args)
            {
                StackTraceService.ConsoleLog("Program started..."); 

                // verify/parse command line arguments.
                if (ConsoleArgumentsService.TryParseAndSetArgs(args))
                {
                    // start new thread to listen for acks indefinitely
                    ListenerService.ListenForSackPackets();
                    
                    // try to send packets
                    if (SenderService.TrySendDataAndEotPackets()) {
                        StackTraceService.ConsoleLog("Sent all packets!");
                    }
                    else {
                        //  todo:  IF THIS FAILS WE SHOULD CANCEL THE THREAD ****************
                        StackTraceService.ConsoleLog("Failed to send packets.");
                    }
                }
                else {
                    StackTraceService.ConsoleLog("Failed to parse console paramater(s).");
                }
            }
        }
}
