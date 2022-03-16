namespace A2Sender.services
{
    // ConsoleArgumentsService: Singleton Service for parsing, storing, and retreiving console arguments provided 
    //  when running ./sender.sh ... on the command line.
    public static class ConsoleArgumentsService
    {
        private static string? hostAddress;
        private static int? portEmulator;
        private static int? portSender;
        private static int? timeout;
        private static string? fileName;

        public static string GetHostAddress() {
            if (hostAddress == null) throw new Exception("hostAddress not set");
            return hostAddress;
        }
        public static int GetPortEmulator() {
            if (portEmulator == null) throw new Exception("portEmulator not set");
            return (int) portEmulator;
        }
        public static int GetPortSender() {
            if (portSender == null) throw new Exception("portSender not set");
            return (int) portSender;
        }
        public static int GetTimeout() {
            if (timeout == null) throw new Exception("timeout not set");
            return (int) timeout;
        }
        public static string GetFileName() {
            if (fileName == null) throw new Exception("fileName not set");
                return fileName;
        }

        // Tries to parse the args provided by command script and set to static fields.
        public static bool TryParseAndSetArgs(string[] args)
        {
            StackTraceService.ConsoleLog("Reading Input...");
            // validate correct number of arguments and no whitespace
            int numArguments = args.Length;
            if (numArguments != 5) {
                StackTraceService.ConsoleLog("Invalid number of arguments provided.");
                return false;
            }
            for (int i = 0; i < numArguments; ++i) {
                if (string.IsNullOrEmpty(args[i]) || string.IsNullOrWhiteSpace(args[i]) ) {
                    StackTraceService.ConsoleLog($"Argument {(i+1)} + is invalid.");
                    return false;
                }
            }
            
            string hostAddress = args[0];
            int portEmulator;
            int portSender;
            int timeout;
            if (!int.TryParse(args[1], out portEmulator) || portEmulator <= 0) {
                StackTraceService.ConsoleLog("<port_emulator> must an unsigned integer.");
                return false;
            }
            if (!int.TryParse(args[2], out portSender) || portSender <= 0) {
                StackTraceService.ConsoleLog("<port_sender> must an unsigned integer.");
                return false;
            }
            if (!int.TryParse(args[3], out timeout) || timeout <= 0) {
                StackTraceService.ConsoleLog("<timeout> must an unsigned integer.");
                return false;
            }
            string fileName = args[4];
        
            ConsoleArgumentsService.hostAddress = hostAddress;
            ConsoleArgumentsService.portEmulator = portEmulator;
            ConsoleArgumentsService.portSender = portSender;
            ConsoleArgumentsService.timeout = timeout;
            ConsoleArgumentsService.fileName = fileName;
            Console.WriteLine("Reading input successful!");
            return true;
        }
    }
}
