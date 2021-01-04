using GameEngine.Game;

namespace GameEngine.Util
{
    public static class IOHelper
    {
        public static string ReadTextFile(Path fpath)
        {
            string result = System.IO.File.ReadAllText(fpath);
            Debug.LogSilent($"[IO] Read from to {fpath.GetShortName()}, size {result.Length}");
            return result;
        }
        public static void WriteTextFile(Path fpath, string text)
        {
            Debug.LogSilent($"[IO] Wrote to {fpath.GetShortName()}, size {text.Length}");
            System.IO.File.WriteAllText(fpath, text);
        }
    }
}
