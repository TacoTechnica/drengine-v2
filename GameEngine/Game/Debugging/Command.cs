using System.Diagnostics;
using System.Linq;

namespace GameEngine.Game.Debugging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;

    #region Args & parsing them

    public abstract class ArgBase
    {
        public int MinArgCountToUseDefault { get; protected set; }
        public bool HasDefault { get; protected set; }

        public virtual bool IsArray { get { return false; } }
        protected V GetConverted<V> ( object ob )
        {
            try {
                return (V) Convert.ChangeType(ob, typeof(V));
            } catch ( Exception ) {
                throw new InvalidArgumentsException($"Tried to convert the following object to type {typeof(V)} and failed: {ob}. This is probably an internal problem, contact the dev!");
                //return default(T);
            }
        }
        public abstract object ParseUnit ( string unit, string[] unitPlusRemainder );
        public V ParseUnit<V>(string unit, string[] unitPlusRemainder)
        {
            return GetConverted<V>(ParseUnit(unit, unitPlusRemainder));
        }

        public abstract V GetDefault<V> ();

        public abstract string GetHelpRepresentation ();
    }

    /// <summary>
    ///     A single arg.
    /// </summary>
    public class Arg<T> : ArgBase
    {
        public T Default { get; private set; }

        // This is important cause if it is, it will stop parsing further variables and end here as it is a params.
        public override bool IsArray { get { return IsInstanceOf<T>(typeof(Array)); } }

        private Dictionary<string, T> _enumValues = null;
        private bool _isEnum { get { return _enumValues != null; } }

        private string _name = "";

        private bool _showDefault;

        // Regular Constructor
        public Arg(string name)
        {
            this._name = name;

            _showDefault = true;
            HasDefault = false;
            // If enum, take action.
            if ( IsInstanceOf<T>(typeof(Enum)) ) {
                _enumValues = new Dictionary<string, T>();
                foreach ( T v in Enum.GetValues(typeof(T)) ) {
                    _enumValues.Add(v.ToString().ToLower(), v);
                }
            } else {
                // Make sure as an extra precaution that we only use (non enum) types we can handle
                if ( !IsInstancesOf<T>(typeof(string), typeof(float), typeof(int), typeof(double), typeof(long))) {
                    throw new InvalidSetupException($"Arguments are not programmed to parse the following type: {typeof(T)}. This is either not implemented intentionally or by accident somehow.");
                }
            }
        }

        // Constructor with default value
        public Arg (string name, T defaultValue, int minArgCountToUseDefault, bool showDefault = true ) : this(name)
        {
            HasDefault = true;
            Default = defaultValue;
            MinArgCountToUseDefault = minArgCountToUseDefault;
            _showDefault = showDefault;
        }

        /// <summary>
        ///     Return the "help" command representation of this argument.
        ///     For instance, in a "dialogue" command it looks like this:
        ///         dialogue <name = ""> [text]
        ///     name is optional and defaults to "", while text is non optional.
        /// </summary>
        public override string GetHelpRepresentation()
        {
            if (HasDefault) {
                if ( _showDefault ) {
                    return "<" + _name + "=" + Default + ">";
                }
                return "<" + _name + ">";
            }
            return "[" + _name + "]";
        }

        private bool IsInstanceOf<V>(Type t)
        {
            return typeof(V) == t || typeof(V).IsSubclassOf(t);
        }

        private bool IsInstancesOf<V> ( params Type[] types ) {
            foreach(Type t in types) {
                if (IsInstanceOf<V>(t)) {
                    return true;
                }
            }
            return false;
        }

        private void ParseErrorCheck(bool good, object value, string type)
        {
            if (!good)
                throw new InvalidArgumentsException($"Failed to parse the following argument into type {type}: {value}.");
        }

        private V ParseUnitUtil<V>(string unit, string[] unitPlusRemainder)
        {
            // If enum, check from our cached enum dictionary.
            if ( _isEnum ) {
                unit = unit.ToLower();
                if ( !_enumValues.ContainsKey(unit) ) {
                    StringBuilder res = new StringBuilder();
                    foreach ( string type in _enumValues.Keys ) {
                        res.Append(type);
                        res.Append("|");
                    }
                    res.Remove(res.Length - 1, 1); // Remove the last "|"
                    throw new InvalidArgumentsException($"Invalid argument found: {unit}. Accepted values are: {res}");
                }
                return GetConverted<V>(_enumValues[unit]);
            }

            // Do number parsing.
            if ( IsInstanceOf<V>(typeof(float)) ) {
                ParseErrorCheck(float.TryParse(unit, out _), unit, "float");
                return GetConverted<V>(float.Parse(unit));
            }
            if ( IsInstanceOf<V>(typeof(double)) ) {
                ParseErrorCheck(double.TryParse(unit, out _), unit, "double");
                return GetConverted<V>(double.Parse(unit));
            }
            if ( IsInstanceOf<V>(typeof(int)) ) {
                ParseErrorCheck(int.TryParse(unit, out _), unit, "int");
                return GetConverted<V>(int.Parse(unit));
            }
            if ( IsInstanceOf<V>(typeof(long)) ) {
                ParseErrorCheck(long.TryParse(unit, out _), unit, "long");
                return GetConverted<V>(long.Parse(unit));
            }

            // Now do string parsing.
            if ( IsInstanceOf<V>(typeof(string)) ) {
                // Remove quotes
                if ( unit.Length >= 2 ) {
                    if ( unit[0] == '\"' && unit[unit.Length - 1] == '\"' ) {
                        unit = unit.Substring(1, unit.Length - 2);
                    }
                }
                return GetConverted<V>(unit);
            }

            // For arrays, parse them uh individually.
            if (IsInstanceOf<V>(typeof(Array))) {
                // Get the type of the individual array generic by creating a dummy. Not a smart idea but whatever.
                Array dummy = GetConverted<Array>(Activator.CreateInstance<V>());
                Type subType = dummy.GetType().GetElementType();

                // Call the generic method with reflection. TODO: This is kinda bad but whatever.
                var thisMethod = typeof(Arg<T>).GetMethod("ParseUnitUtil");
                var thisMethodRef = thisMethod.MakeGenericMethod(subType);

                List<object> result = new List<object>();

                string[] remainingUnits = new string[unitPlusRemainder.Length];
                unitPlusRemainder.CopyTo(remainingUnits, 0);
                foreach(string subUnit in unitPlusRemainder) {

                    // Sanity check
                    if (remainingUnits.Length == 0 || subUnit != remainingUnits[0]) {
                        Debug.LogError("SANITY CHECK FAILED!");
                        Debug.LogError($"This shouldn't happen: {remainingUnits.Length}");
                        Debug.LogError($"This shouldn't happen: {subUnit} != {(remainingUnits.Length != 0? remainingUnits[0] : null)}");
                        break;
                    }

                    // Parse the first value and add to our array.
                    object subValue = thisMethodRef.Invoke(this, new object[] { subUnit, remainingUnits });
                    result.Add(subValue);

                    Debug.Log($"MID ARRAY DEBUG: Sub: {subUnit} = {subValue}");

                    // Pop off the first unit
                    string[] copy = new string[remainingUnits.Length - 1];
                    remainingUnits.CopyTo(copy, 1);
                    remainingUnits = copy;
                }

                return GetConverted<V>( result.ToArray() );
            }
            throw new InvalidSetupException($"Arguments are not programmed to parse the following type: {typeof(T)}. This is either not implemented intentionally or by accident somehow.");
        }

        public override object ParseUnit ( string unit, string[] unitPlusRemainder )
        {
            return ParseUnitUtil<T>(unit, unitPlusRemainder);
        }

        public virtual bool CheckValidUnit ( string arg, out string errorMsg )
        {
            errorMsg = "";
            return true;
        }

        public override V GetDefault<V> ()
        {
            return GetConverted<V>(Default);
        }
    }

    /// <summary>
    ///     This parses args from a command line and spits out the arguments.
    ///     If it fails, it returns an exception with a resulting message.
    /// </summary>
    public class ArgParser
    {
        public ArgBase[] Args { get; private set; }
        int argCounter;
        int unitCounter;
        string[] argUnits;

        public ArgParser ( params ArgBase[] args )
        {
            this.Args = args;
            argCounter = 0;
            unitCounter = 0;
        }

        public void LoadArgs ( string line )
        {
            List<string> units = SplitLineIntoKeywords(line);
            // Discard the first element since, well, it will always be the name of the command.
            if ( units.Count != 0 ) {
                units.RemoveAt(0);
            }
            argUnits = units.ToArray();
            argCounter = 0;
            unitCounter = 0;
        }

        // Get the next argument.
        public T Get<T> ()
        {

            if ( argCounter >= Args.Length ) {
                throw new InvalidSetupException($"You tried grabbing more arguments than you had... Bad move.");
            }
            if ( argUnits.Length > Args.Length ) {
                throw new InvalidArgumentsException($"Too many arguments provided ({argUnits.Length})! The maximum is {Args.Length}.");
            }

            // Current values from arrays
            ArgBase arg = Args[argCounter];
            ++argCounter;
            if ( arg.IsArray ) {
                argCounter = Args.Length;
            }

            // If this can be default and we don't have enough (unit) args provided to use this arg, use the default value instead of reading from our arg list.
            int givenArgs = argUnits.Length;
            if ( arg.HasDefault && arg.MinArgCountToUseDefault >= givenArgs ) {
                return arg.GetDefault<T>();
            }


            if ( unitCounter >= argUnits.Length ) {
                throw new InvalidArgumentsException($"Not enough arguments supplied: You supplied {argUnits.Length}");
            }

            string unit = argUnits[unitCounter];
            string[] unitPlusRemaining = new string[argUnits.Length - unitCounter];
            Array.Copy(argUnits, unitCounter, unitPlusRemaining, 0, unitPlusRemaining.Length);
            //argUnits.CopyTo(unitPlusRemaining, unitCounter);

            ++unitCounter;

            // If our type is not valid, try um handling the defaults.

            return arg.ParseUnit<T>(unit, unitPlusRemaining);
        }

        // Given a single line as a string, parse it into a list of keywords
        public static List<string> SplitLineIntoKeywords ( string line )
        {
            List<string> result = new List<string>();
            // By default, it's just spaces. But sometimes we want to count quotes. So do it manually.
            string last_kword = "";
            bool open_quote = false;
            char prev_char = '\0';
            foreach ( char c in line ) {
                // We found a quote, update our "quote" state.
                if ( c == '\"' ) {
                    open_quote = !open_quote;
                }
                if ( prev_char == '\\' ) {
                    if ( c == '#' || c == '"' ) {
                        // We escaped this pound sign, so ignore the escaping backslash
                        last_kword = last_kword.Substring(0, last_kword.Length - 1);
                    }
                } else {
                    if ( c == '#' ) {
                        // Bail! Everything beyond this is part of a comment, so ignore.
                        break;
                    }
                }
                if ( c == ' ' && !open_quote ) {
                    // If it's empty, just ignore.
                    if ( last_kword.Length != 0 ) {
                        // Remove trailing whitespace
                        result.Add(last_kword.Trim());
                    }
                    last_kword = "";
                } else {
                    last_kword += c;
                }
                prev_char = c;
            }
            // Add the remainder
            if ( last_kword.Length != 0 ) {
                result.Add(last_kword.Trim());
            }
            return result;
        }
    }

    #endregion

    #region The Command

    public abstract class Command
    {
        ArgParser parser;

        public string Name { get; private set; }
        public string Description { get; private set; }

        private GamePlus _game;

        protected DebugConsole DebugConsole => _game.DebugConsole;

    public Command(string name, string description, params ArgBase[] args)
        {
            Name = name;
            Description = description;
            parser = new ArgParser(args);
        }

        public void Run(GamePlus game, string line)
        {
            _game = game;
            parser.LoadArgs(line);
            Call(game, parser);
        }

        public string GetHelpRepresentation()
        {
            StringBuilder sb = new StringBuilder(Name);
            foreach(ArgBase arg in parser.Args) {
                sb.Append(" ");
                sb.Append(arg.GetHelpRepresentation());
            }
            return sb.ToString();
        }

        protected void Log(object message)
        {
            DebugConsole.PrintToOutput(message.ToString());
        }

        protected void LogError(object message)
        {
            DebugConsole.PrintErrorToOutput(message.ToString(), new StackTrace(1, true).ToString());
        }

        protected abstract void Call(GamePlus game, ArgParser parser);
    }

    #endregion

    #region Reading & Accessing commands

    public static class Commands
    {
        private static Dictionary<string, Command> commandSheet = null;

        public static void Init(params string[] commandListNamespaces )
        {
            HashSet<string> namespaces = new HashSet<string>(commandListNamespaces);
            // We already initialized, so ignore so we don't do more work.
            if ( commandSheet != null ) return;

            commandSheet = new Dictionary<string, Command>();

            Type[] typeList = Assembly.GetExecutingAssembly().GetTypes();
            foreach ( Type type in typeList ) {
                if ( !namespaces.Contains(type.Namespace) ) {
                    continue;
                }
                try {
                    if (!type.IsSubclassOf(typeof(Command))) {
                        continue;
                    }
                    Command command = (Command) Activator.CreateInstance(type);
                    if ( commandSheet.ContainsKey(command.Name) ) {
                        throw new InvalidSetupException($"Duplicate commands of name {command.Name} found!");
                    }
                    commandSheet[command.Name] = command;
                } catch ( MissingMethodException ) {
                    // It's a method here, so we ignore.
                }
            }
        }

        /// <summary>
        /// Init but you pass it classes instead of namespaces by string. Makes it more type-safe.
        /// </summary>
        public static void Init(params Type[] oneTypeFromEachNamespaces)
        {
            string[] namespaces = oneTypeFromEachNamespaces.ToList().ConvertAll(type => type.Namespace).ToArray();
            Init(namespaces);
        }

        public static void Run(GamePlus game, string line)
        {
            Command c = GetCommand(line);
            if (c != null)
            {
                try
                {
                    c.Run(game, line);
                }
                catch (InvalidArgumentsException ae)
                {
                    throw new InvalidArgumentsException($"{ae.Message}\nUsage: {c.GetHelpRepresentation()}", ae);;
                }
            }
        }

        private static Command GetCommand(string line)
        {
            if (commandSheet == null) {
                throw new InvalidSetupException("You forgot to initialize the command catalogue thing! Call Commands.Init() somewhere at the start of the game to initialize this.");
            }

            if (line.Length != 0)
            {
                string command = line;
                int firstSpace = line.IndexOf(' ');
                if (firstSpace != -1)
                {
                    command = line.Substring(0, firstSpace);
                }

                if (!commandSheet.ContainsKey(command))
                {
                    throw new InvalidArgumentsException($"Command {command} does not exist.");
                }

                return commandSheet[command];
            }
            return null;

        }

        public static IEnumerable<Command> AllCommands
        {
            get {
                foreach(Command c in commandSheet.Values) {
                    yield return c;
                }
            }
        }

        public static Command Get(string name)
        {
            return (commandSheet.ContainsKey(name) ? commandSheet[name] : null);
        }
    }

    #endregion

    #region Custom Exceptions

    /// <summary>
    ///     Runtime any time.
    ///     Tells you your console arguments are bad. >:(
    /// </summary>
    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException ( string message )
            : base(message)
        {
        }
        public InvalidArgumentsException ( string message, Exception inner )
            : base(message, inner)
        {
        }
    }

    /// <summary>
    ///     Runtime at the very beginning.
    ///     Tells you your command is poorly made. >:((
    /// </summary>
    public class InvalidSetupException : Exception
    {
        public InvalidSetupException ( string message )
            : base(message)
        {
        }
        public InvalidSetupException ( string message, Exception inner )
            : base(message, inner)
        {
        }
    }

    #endregion

}
