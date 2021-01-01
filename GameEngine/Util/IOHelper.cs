namespace GameEngine.Util
{
    public static class IOHelper
    {
        public static string ReadTextFile(string fpath)
        {
            string result = System.IO.File.ReadAllText(fpath);
            Debug.LogSilent($"[IO] Read from to {fpath}, size {result.Length}");
            return result;
        }
        public static void WriteTextFile(string fpath, string text)
        {
            Debug.LogSilent($"[IO] Wrote to {fpath}, size {text.Length}");
            System.IO.File.WriteAllText(fpath, text);
        }
    }
}
