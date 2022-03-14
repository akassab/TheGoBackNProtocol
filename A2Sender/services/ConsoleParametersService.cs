namespace A2Sender.services
{
    public static class ConsoleParametersService
    {
        private static string? hostAddress;
        private static int? portEmulator;
        private static int? portSender;
        private static int? timeout;
        private static string? fileName;

        public static bool IsConsoleParametersSet() {
            return hostAddress != null && portEmulator != null && portSender != null && fileName != null;
        }

        public static void SetConsoleParameters(string hostAddress, int portEmulator, int portSender, int timeout, string fileName) {
            // Set console parameters
            if (ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: SetConsoleParameters(): Console Parameters already set.");
            }
            ConsoleParametersService.hostAddress = hostAddress;
            ConsoleParametersService.portEmulator = portEmulator;
            ConsoleParametersService.portSender = portSender;
            ConsoleParametersService.timeout = timeout;
            ConsoleParametersService.fileName = fileName;
        }

        public static int GetTimeOut() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: GetTimeOut(): Console Parameters not set.");
            }
            return (int) ConsoleParametersService.timeout;
        }

        public static int GetPortSender() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: GetSenderPort(): Console Parameters not set.");
            }
            return (int) ConsoleParametersService.portSender;
        }

        public static int GetPortEmulator() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: GetPortEmulator(): Console Parameters not set.");
            }
            return (int) ConsoleParametersService.portEmulator;
        }

        public static string GetFileName() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: GetFileName(): Console Parameters not set.");
            }
            return (string) ConsoleParametersService.fileName;
        }

        public static string GetHostAddress() {
            if (!ConsoleParametersService.IsConsoleParametersSet()) {
                // This should never execute
                throw new Exception("ConsoleParametersService: GetHostAddress(): Console Parameters not set.");
            }
            return (string) ConsoleParametersService.hostAddress;
        }

        // Will try to parse the arguments provided with the ./server.sh script.
        //  returns whether parsing succeeded or not.
        public static bool TryParseAndSetArgs(string[] args)
        {
            Console.WriteLine("Reading input...");
            int numArguments = args.Length;
            if (numArguments != 5) {
                Console.WriteLine("Invalid number of arguments provided.");
                return false;
            }

            // validate no empty or whitespace
            for (int i = 0; i < numArguments; ++i) {
                if (string.IsNullOrEmpty(args[i]) || string.IsNullOrWhiteSpace(args[i]) ) {
                    Console.WriteLine("Argument " + (i+1) + "is invalid");
                    return false;
                }
            }
            // get all fields and validate types
            string hostAddress = args[0];
            int portEmulator;
            if (!int.TryParse(args[1], out portEmulator) || portEmulator <= 0) {
                Console.WriteLine("<port_emulator> must an unsigned integer.");
                return false;
            }
            int portSender;
            if (!int.TryParse(args[2], out portSender) || portSender <= 0) {
                Console.WriteLine("<port_sender> must an unsigned integer.");
                return false;
            }
            int timeout;
            if (!int.TryParse(args[3], out timeout) || timeout <= 0) {
                Console.WriteLine("<timeout> must an unsigned integer.");
                return false;
            }
            string fileName = args[4];
        
            // Initialize data class
            ConsoleParametersService.SetConsoleParameters(hostAddress, portEmulator, portSender, timeout, fileName);
            Console.WriteLine("Reading input successful!");
            return true;
        }
    }
}
