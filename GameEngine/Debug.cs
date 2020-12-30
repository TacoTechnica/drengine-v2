using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameEngine
{
    public static class Debug
    {
        public static bool PrintDebug = true;
        public static bool PrintTrace = true;
        public static bool ShortenPath = true;
        public static int padding = 20;

        public static Action<string> OnLogPrint;
        public static Action<string> OnLogDebug;
        public static Action<string, string> OnLogError;
        public static Action<string> OnLogWarning;

        private static string RootDirectory = "";

        private static object _lock = new object();

        public static void InitRootDirectory()
        {
            string thisFile = new StackTrace(true).GetFrame(0).GetFileName().Replace('\\', '/');
            int lastDir = thisFile.LastIndexOf("/");
            if (lastDir != -1)
            {
                thisFile = thisFile.Substring(0, lastDir);
            }

            RootDirectory = thisFile;
        }

        private static string GetPrint(string log, int traceOffs)
        {
            if (PrintTrace)
            {
                StackTrace strace = new StackTrace(2 + traceOffs, true);
                StackFrame sf = strace.GetFrame(0);
                if (sf != null && sf.GetFileName() != null)
                {
                    string path = sf.GetFileName().Substring(RootDirectory.Length + 1).Replace('\\', '/');
                    if (ShortenPath)
                    {
                        path = string.Join("/", GetShortenedStackPath(path.Split("/")));
                    }

                    path = $"{path}:{sf.GetFileLineNumber()}";
                    return $"{path.PadRight(padding)} {log}";
                }
                return $"??:{log}";
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
            lock (_lock)
            {
                if (PrintDebug) Console.WriteLine($"***({log})***");
                OnLogDebug?.Invoke(log);
            }
        }

        public static void Log(string log)
        {
            LogSilent(log, 1);
            lock (_lock)
            {
                OnLogPrint?.Invoke(log);
            }
        }

        public static void LogError(string log)
        {
            lock (_lock)
            {
                int offs = 1;
                Console.Error.WriteLine(GetPrint(log, offs));
                string stack = new StackTrace(offs, true).ToString();
                Console.Out.WriteLine(stack);
                OnLogError?.Invoke(log, stack);
            }
        }

        public static void LogWarning(string log)
        {
            lock (_lock)
            {
                Console.Error.WriteLine(GetPrint(log, 1));
                OnLogWarning?.Invoke(log);
            }
        }

        /// <summary>
        ///     Debug Logs without alerting anyone.
        /// </summary>
        public static void LogSilent(string log, int offs = 0)
        {
            lock (_lock)
            {
                Console.WriteLine(GetPrint(log, offs));
            }
        }

    }
}
