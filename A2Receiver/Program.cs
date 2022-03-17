using A2Receiver.services;
using A2Receiver.utils;

namespace A2Receiver
{
internal class Program
    {
        // Receiver program
        public static void Main(string[] args)
        {
            
            StackTraceService.ConsoleLog("Receiver program Started.");

            // verify/parse command line arguments.
            if (!ConsoleArgumentsService.TryParseAndSetArgs(args))
            {
                StackTraceService.ConsoleLog("Failed to parse console paramater(s).");
                return;
            }

             // try to create log and arrival file files
            if (!FileUtils.TryCreateEmptyFile(ConsoleArgumentsService.GetFileName())) {
                StackTraceService.ConsoleLog("Failed to create empty data file.");
                return;
            }
            if (!FileUtils.TryCreateEmptyFile(FileUtils.arrivalLogFileName)) {
                StackTraceService.ConsoleLog("Failed to create empty arrival.log file.");
                return;
            }

            // intialize the sender client
            SenderService.ConnectUdpClient();

            // start listening indenfitely
            ListenerService.ListenForDataAndEotPackets();
        }
    }
}
