using A2Sender.services;
using A2Sender.utils;
namespace A2Sender
{
    // Beginning of the program.
    internal class Program
        {
            public static void Main(string[] args)
            {
                StackTraceService.ConsoleLog("Program started..."); 

                // verify/parse command line arguments.

                // Try to parse console arguments
                if (!ConsoleArgumentsService.TryParseAndSetArgs(args))
                {
                     StackTraceService.ConsoleLog("Failed to parse console paramater(s).");
                     return;
                }
                // Try to create log files
                if (!FileUtils.TryCreateEmptyLogFiles()) {
                    StackTraceService.ConsoleLog("Failed to create log files.");
                    return;
                }


                // start new thread to listen for acks indefinitely
                ListenerService.ListenForSackPackets();
                    
                    
                // try to send packets
                if (SenderService.TrySendDataAndEotPackets()) {
                    StackTraceService.ConsoleLog("Sent all packets!");
                }
       
            }
        }
}
