namespace A2Receiver.services
{
    // Singleton Service for parsing, storing, and retreiving console arguments provided 
    //  when running ./receiver.sh ... on the command line.
    public static class ConsoleArgumentsService
    {
        private static string? hostName = null;
        private static int? portEmulator = null;
        private static int? portReceiver = null;
        private static string? fileName = null;

        public static string GetHostName() {
            if (hostName == null) throw new Exception("hostName not set");
            return hostName;
        }

        public static int GetPortEmulator() {
            if (portEmulator == null) throw new Exception("portEmulator not set");
            return (int) portEmulator;
        }

        public static int GetPortReceiver() {
            if (portReceiver == null) throw new Exception("portReceiver not set");
            return (int) portReceiver;
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
            if (numArguments != 4) {
                StackTraceService.ConsoleLog("Invalid number of arguments provided.");
                return false;
            }
            for (int i = 0; i < numArguments; ++i) {
                if (string.IsNullOrEmpty(args[i]) || string.IsNullOrWhiteSpace(args[i]) ) {
                    StackTraceService.ConsoleLog($"Argument {(i+1)} + is invalid.");
                    return false;
                }
            }

            string hostName = args[0];
            int portEmulator;
            int portReceiver;
            string fileName = args[3];
            if (!int.TryParse(args[1], out portEmulator) || portEmulator <= 0) {
                StackTraceService.ConsoleLog("<port_emulator> must an unsigned integer.");
                return false;
            }
            if (!int.TryParse(args[2], out portReceiver) || portReceiver <= 0) {
                StackTraceService.ConsoleLog("<port_receiver> must an unsigned integer.");
                return false;
            }
            
            ConsoleArgumentsService.hostName = hostName;
            ConsoleArgumentsService.portEmulator = portEmulator;
            ConsoleArgumentsService.portReceiver = portReceiver;
            ConsoleArgumentsService.fileName = fileName;

            StackTraceService.ConsoleLog("Reading input successful!");
            return true;
        }
    }
}
