namespace A2Sender.utils
{
    public static class FileReader
    {
        public static byte[]? ReadFile(string fileName)
        {
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch
            {
                Console.WriteLine("FileReader: ReadFile(): Could not read bytes from file name " + fileName);
                return null;
            }
        }
    }
}