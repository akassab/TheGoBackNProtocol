using A2Sender.services;
using A2Sender.utils;
namespace A2Sender
{
    // Beginning of the program.
    internal class Program
        {
            // Sender program
            public static void Main(string[] args)
            {
                StackTraceService.ConsoleLog("Sender program started."); 

                // verify/parse command line arguments.
                if (!ConsoleArgumentsService.TryParseAndSetArgs(args))
                {
                     StackTraceService.ConsoleLog("Failed to parse console paramater(s).");
                     return;
                }

                // try to create log files
                if (!FileUtils.TryCreateEmptyLogFiles()) {
                    StackTraceService.ConsoleLog("Failed to create log files.");
                    return;
                }

                // start new thread to listen for acks indefinitely
                new Thread(() => ListenerService.ListenForSackPackets()).Start();

                // send all the data and eot packet(s)
                SenderService.SendDataAndEotPackets();
            }
        }
}
