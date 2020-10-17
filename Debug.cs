using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DREngine
{
    public static class Debug
    {
        public static bool PrintDebug = true;
        public static bool PrintTrace = true;
        public static bool ShortenPath = true;
        public static int padding = 20;

        private static string GetPrint(string log)
        {
            if (PrintTrace)
            {
                StackTrace strace = new StackTrace(2, true);
                StackFrame sf = strace.GetFrame(0);
                string path = sf.GetFileName().Substring(Program.RootDirectory.Length + 1).Replace('\\', '/');
                if (ShortenPath)
                {
                    path = string.Join("/", GetShortenedStackPath(path.Split("/")));
                }

                path = $"{path}:{sf.GetFileLineNumber()}";
                return $"{path.PadRight(padding)} {log}";
            }
            return log;
        }

        private static IEnumerable<string> GetShortenedStackPath(string[] path)
        {
            foreach (string folder in path.SkipLast(1))
            {
                if (folder.Length <= 1) yield return folder;
                else yield return folder.Substring(0, 1);
            }
            yield return path.Last();
        }

        public static void LogDebug(string log)
        {
            if (PrintDebug) Console.WriteLine($"***({log})***");
        }

        public static void Log(string log)
        {
            Console.WriteLine(GetPrint(log));
        }

        public static void LogError(string log)
        {
            Console.Error.WriteLine(GetPrint(log));
        }
    }
}
