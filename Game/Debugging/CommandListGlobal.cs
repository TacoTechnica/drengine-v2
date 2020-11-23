// ReSharper disable UnusedType.Global
// ReSharper disable ArrangeTypeModifiers

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DREngine.Game.Debugging.CommandListGlobal
{
    class Help : Command
    {
        public Help () : base(
            "help", "Lists all commands or gets help from specific command.",
            new Arg<string>("command", "", 0, false)
        ) {}

        protected override void Call ( ArgParser parser )
        {
            string command = parser.Get<string>();

            if (command == "") {
                // List all commands
                Log("");
                Log("Command List:");
                Log("=========================");
                List<Command> commands = new List<Command>(Commands.AllCommands);
                var comparer = Comparer<Command>.Create(
                    ( c1, c2 ) => c1.Name.CompareTo(c2.Name));
                commands.Sort(comparer);
                foreach ( Command c in commands) {
                    Log($"    {c.Name.PadRight(10)} : {c.Description}");
                }
                Log("=========================");
            } else {
                // We have a command!
                Command c = Commands.Get(command);
                if (c == null) {
                    LogError($"Command does not exist: {command}");
                } else {
                    Log("");
                    Log("=========================");
                    Log($"{c.Name}");
                    Log("=========================");
                    Log($"        {c.Description}");
                    Log("");
                    Log($"Usage:  {c.GetHelpRepresentation()}");
                    Log("=========================");
                }
            }
        }
    }

    class Temp : Command
    {
        enum TempEnum
        {
            Level0,
            Level1
        }

        public Temp () : base(
            "temp", "Temporary demonstration command.",
                new Arg<string>("stringA"),
                new Arg<int>("num1", 24, 3),
                new Arg<string>("stringB", "hi", 2),
                new Arg<TempEnum>("choice")
        ) {}

        protected override void Call ( ArgParser args )
        {
            string stringA = args.Get<string>();     // First arg
            int num1 = args.Get<int>();                // Second (optional) arg
            string stringB = args.Get<string>();     // Third arg
            TempEnum en = args.Get<TempEnum>();   // Fourth arg.

            Log("YOU RAN THE COMMAND!");
            Log($"stringA = {stringA}, num1 = {num1}, stringB = {stringB}, choice = {en}");
            //string[] extra = args.Get<string[]>(); // "Args" args.
        }
    }

    class Clear : Command
    {
        public Clear () : base(
            "clear", "Clears the console.")
        { }

        protected override void Call ( ArgParser parser )
        {
            DebugConsole.Clear();
        }
    }

    class Close : Command
    {
        public Close () : base(
            "close", "Closes the console. Pressing Escape also works."
        ) { }

        protected override void Call ( ArgParser parser )
        {
            DebugConsole.Close();
        }
    }


    class Print : Command
    {
        public Print () : base(
            "print", "Print to the console. Calls Debug.Log",
            new Arg<string>("text")
        ) { }

        protected override void Call ( ArgParser parser )
        {
            string text = parser.Get<string>();
            Debug.Log(text);
        }
    }

    class Scene : Command
    {
        public Scene () : base(
            "scene", "Switch scene or list all scenes.",
            new Arg<string>("scenePath", "", 0, false)
        ) { }

        private static HashSet<string> scenesBuilt = null;

        protected override void Call ( ArgParser parser )
        {
            string scene = parser.Get<string>();

            LogError("Not implemented.");
        }


    }

    class Reload : Command
    {
        public Reload () : base(
            "reload", "Reloads the current scene."
        ) { }

        protected override void Call ( ArgParser parser )
        {
            LogError("Not implemented.");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    class Restart : Command
    {
        public Restart () : base(
            "restart", "Restarts the game."
        ) { }

        protected override void Call ( ArgParser parser )
        {
            LogError("Not implemented.");
        }
    }
}
