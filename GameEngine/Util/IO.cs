namespace GameEngine.Util
{
    public static class IO
    {
        public static string ReadTextFile(string fpath)
        {
            return System.IO.File.ReadAllText(fpath);
        }
        public static void WriteTextFile(string fpath, string text)
        {
            System.IO.File.WriteAllText(fpath, text);
        }
    }
}
