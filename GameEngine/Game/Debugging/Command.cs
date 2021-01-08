using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GameEngine.Game.Debugging
{
    #region Args & parsing them

    public abstract class ArgBase
    {
        public int MinArgCountToUseDefault { get; protected set; }
        public bool HasDefault { get; protected set; }

        public virtual bool IsArray => false;

        protected TV GetConverted<TV>(object ob)
        {
            try
            {
                return (TV) Convert.ChangeType(ob, typeof(TV));
            }
            catch (Exception)
            {
                throw new InvalidArgumentsException(
                    $"Tried to convert the following object to type {typeof(TV)} and failed: {ob}. This is probably an internal problem, contact the dev!");
                //return default(T);
            }
        }

        public abstract object ParseUnit(string unit, string[] unitPlusRemainder);

        public TV ParseUnit<TV>(string unit, string[] unitPlusRemainder)
        {
            return GetConverted<TV>(ParseUnit(unit, unitPlusRemainder));
        }

        public abstract TV GetDefault<TV>();

        public abstract string GetHelpRepresentation();
    }

    /// <summary>
    ///     A single arg.
    /// </summary>
    public class Arg<T> : ArgBase
    {
        private readonly Dictionary<string, T> _enumValues;

        private readonly string _name;

        private readonly bool _showDefault;

        // Regular Constructor
        public Arg(string name)
        {
            _name = name;

            _showDefault = true;
            HasDefault = false;
            // If enum, take action.
            if (IsInstanceOf<T>(typeof(Enum)))
            {
                _enumValues = new Dictionary<string, T>();
                foreach (T v in Enum.GetValues(typeof(T))) _enumValues.Add(v.ToString()?.ToLower() ?? throw new InvalidOperationException("How did we get here?"), v);
            }
            else
            {
                // Make sure as an extra precaution that we only use (non enum) types we can handle
                if (!IsInstancesOf<T>(typeof(string), typeof(float), typeof(int), typeof(double), typeof(long)))
                    throw new InvalidSetupException(
                        $"Arguments are not programmed to parse the following type: {typeof(T)}. This is either not implemented intentionally or by accident somehow.");
            }
        }

        // Constructor with default value
        public Arg(string name, T defaultValue, int minArgCountToUseDefault, bool showDefault = true) : this(name)
        {
            HasDefault = true;
            Default = defaultValue;
            MinArgCountToUseDefault = minArgCountToUseDefault;
            _showDefault = showDefault;
        }

        public T Default { get; }

        // This is important cause if it is, it will stop parsing further variables and end here as it is a params.
        public override bool IsArray => IsInstanceOf<T>(typeof(Array));
        private bool IsEnum => _enumValues != null;

        /// <summary>
        ///     Return the "help" command representation of this argument.
        ///     For instance, in a "dialogue" command it looks like this:
        ///     dialogue
        /// </summary>
        public override string GetHelpRepresentation()
        {
            if (HasDefault)
            {
                if (_showDefault) return "<" + _name + "=" + Default + ">";
                return "<" + _name + ">";
            }

            return "[" + _name + "]";
        }

        private bool IsInstanceOf<TV>(Type t)
        {
            return typeof(TV) == t || typeof(TV).IsSubclassOf(t);
        }

        private bool IsInstancesOf<TV>(params Type[] types)
        {
            foreach (var t in types)
                if (IsInstanceOf<TV>(t))
                    return true;
            return false;
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ParseErrorCheck(bool good, object value, string type)
        {
            if (!good)
                throw new InvalidArgumentsException(
                    $"Failed to parse the following argument into type {type}: {value}.");
        }

        private TV ParseUnitUtil<TV>(string unit, string[] unitPlusRemainder)
        {
            // If enum, check from our cached enum dictionary.
            if (IsEnum)
            {
                unit = unit.ToLower();
                if (!_enumValues.ContainsKey(unit))
                {
                    var res = new StringBuilder();
                    foreach (var type in _enumValues.Keys)
                    {
                        res.Append(type);
                        res.Append("|");
                    }

                    res.Remove(res.Length - 1, 1); // Remove the last "|"
                    throw new InvalidArgumentsException($"Invalid argument found: {unit}. Accepted values are: {res}");
                }

                return GetConverted<TV>(_enumValues[unit]);
            }

            // Do number parsing.
            if (IsInstanceOf<TV>(typeof(float)))
            {
                ParseErrorCheck(float.TryParse(unit, out _), unit, "float");
                return GetConverted<TV>(float.Parse(unit));
            }

            if (IsInstanceOf<TV>(typeof(double)))
            {
                ParseErrorCheck(double.TryParse(unit, out _), unit, "double");
                return GetConverted<TV>(double.Parse(unit));
            }

            if (IsInstanceOf<TV>(typeof(int)))
            {
                ParseErrorCheck(int.TryParse(unit, out _), unit, "int");
                return GetConverted<TV>(int.Parse(unit));
            }

            if (IsInstanceOf<TV>(typeof(long)))
            {
                ParseErrorCheck(long.TryParse(unit, out _), unit, "long");
                return GetConverted<TV>(long.Parse(unit));
            }

            // Now do string parsing.
            if (IsInstanceOf<TV>(typeof(string)))
            {
                // Remove quotes
                if (unit.Length >= 2)
                    if (unit[0] == '\"' && unit[unit.Length - 1] == '\"')
                        unit = unit.Substring(1, unit.Length - 2);
                return GetConverted<TV>(unit);
            }

            // For arrays, parse them uh individually.
            if (IsInstanceOf<TV>(typeof(Array)))
            {
                // Get the type of the individual array generic by creating a dummy. Not a smart idea but whatever.
                var dummy = GetConverted<Array>(Activator.CreateInstance<TV>());
                var subType = dummy.GetType().GetElementType();

                // Call the generic method with reflection. TODO: This is kinda bad but whatever.
                var thisMethod = typeof(Arg<T>).GetMethod("ParseUnitUtil");
                if (thisMethod == null || subType == null)
                {
                    throw new InvalidOperationException($"Failed to get command for array type: {typeof(T)}");
                }
                var thisMethodRef = thisMethod.MakeGenericMethod(subType);

                var result = new List<object>();

                var remainingUnits = new string[unitPlusRemainder.Length];
                unitPlusRemainder.CopyTo(remainingUnits, 0);
                foreach (var subUnit in unitPlusRemainder)
                {
                    // Sanity check
                    if (remainingUnits.Length == 0 || subUnit != remainingUnits[0])
                    {
                        Debug.LogError("SANITY CHECK FAILED!");
                        Debug.LogError($"This shouldn't happen: {remainingUnits.Length}");
                        Debug.LogError(
                            $"This shouldn't happen: {subUnit} != {(remainingUnits.Length != 0 ? remainingUnits[0] : null)}");
                        break;
                    }

                    // Parse the first value and add to our array.
                    var subValue = thisMethodRef.Invoke(this, new object[] {subUnit, remainingUnits});
                    result.Add(subValue);

                    Debug.Log($"MID ARRAY DEBUG: Sub: {subUnit} = {subValue}");

                    // Pop off the first unit
                    var copy = new string[remainingUnits.Length - 1];
                    remainingUnits.CopyTo(copy, 1);
                    remainingUnits = copy;
                }

                return GetConverted<TV>(result.ToArray());
            }

            throw new InvalidSetupException(
                $"Arguments are not programmed to parse the following type: {typeof(T)}. This is either not implemented intentionally or by accident somehow.");
        }

        public override object ParseUnit(string unit, string[] unitPlusRemainder)
        {
            return ParseUnitUtil<T>(unit, unitPlusRemainder);
        }

        public virtual bool CheckValidUnit(string arg, out string errorMsg)
        {
            errorMsg = "";
            return true;
        }

        public override TV GetDefault<TV>()
        {
            return GetConverted<TV>(Default);
        }
    }

    /// <summary>
    ///     This parses args from a command line and spits out the arguments.
    ///     If it fails, it returns an exception with a resulting message.
    /// </summary>
    public class ArgParser
    {
        private int _argCounter;
        private string[] _argUnits;
        private int _unitCounter;

        public ArgParser(params ArgBase[] args)
        {
            Args = args;
            _argCounter = 0;
            _unitCounter = 0;
        }

        public ArgBase[] Args { get; }

        public void LoadArgs(string line)
        {
            var units = SplitLineIntoKeywords(line);
            // Discard the first element since, well, it will always be the name of the command.
            if (units.Count != 0) units.RemoveAt(0);
            _argUnits = units.ToArray();
            _argCounter = 0;
            _unitCounter = 0;
        }

        // Get the next argument.
        public T Get<T>()
        {
            if (_argCounter >= Args.Length)
                throw new InvalidSetupException("You tried grabbing more arguments than you had... Bad move.");
            if (_argUnits.Length > Args.Length)
                throw new InvalidArgumentsException(
                    $"Too many arguments provided ({_argUnits.Length})! The maximum is {Args.Length}.");

            // Current values from arrays
            var arg = Args[_argCounter];
            ++_argCounter;
            if (arg.IsArray) _argCounter = Args.Length;

            // If this can be default and we don't have enough (unit) args provided to use this arg, use the default value instead of reading from our arg list.
            var givenArgs = _argUnits.Length;
            if (arg.HasDefault && arg.MinArgCountToUseDefault >= givenArgs) return arg.GetDefault<T>();


            if (_unitCounter >= _argUnits.Length)
                throw new InvalidArgumentsException($"Not enough arguments supplied: You supplied {_argUnits.Length}");

            var unit = _argUnits[_unitCounter];
            var unitPlusRemaining = new string[_argUnits.Length - _unitCounter];
            Array.Copy(_argUnits, _unitCounter, unitPlusRemaining, 0, unitPlusRemaining.Length);
            //argUnits.CopyTo(unitPlusRemaining, unitCounter);

            ++_unitCounter;

            // If our type is not valid, try um handling the defaults.

            return arg.ParseUnit<T>(unit, unitPlusRemaining);
        }

        // Given a single line as a string, parse it into a list of keywords
        public static List<string> SplitLineIntoKeywords(string line)
        {
            var result = new List<string>();
            // By default, it's just spaces. But sometimes we want to count quotes. So do it manually.
            var lastKword = "";
            var openQuote = false;
            var prevChar = '\0';
            foreach (var c in line)
            {
                // We found a quote, update our "quote" state.
                if (c == '\"') openQuote = !openQuote;
                if (prevChar == '\\')
                {
                    if (c == '#' || c == '"') // We escaped this pound sign, so ignore the escaping backslash
                        lastKword = lastKword.Substring(0, lastKword.Length - 1);
                }
                else
                {
                    if (c == '#') // Bail! Everything beyond this is part of a comment, so ignore.
                        break;
                }

                if (c == ' ' && !openQuote)
                {
                    // If it's empty, just ignore.
                    if (lastKword.Length != 0) // Remove trailing whitespace
                        result.Add(lastKword.Trim());
                    lastKword = "";
                }
                else
                {
                    lastKword += c;
                }

                prevChar = c;
            }

            // Add the remainder
            if (lastKword.Length != 0) result.Add(lastKword.Trim());
            return result;
        }
    }

    #endregion

    #region The Command

    public abstract class Command
    {
        private GamePlus _game;
        private readonly ArgParser _parser;

        public Command(string name, string description, params ArgBase[] args)
        {
            Name = name;
            Description = description;
            _parser = new ArgParser(args);
        }

        public string Name { get; }
        public string Description { get; }

        protected DebugConsole DebugConsole => _game.DebugConsole;

        public void Run(GamePlus game, string line)
        {
            _game = game;
            _parser.LoadArgs(line);
            Call(game, _parser);
        }

        public string GetHelpRepresentation()
        {
            var sb = new StringBuilder(Name);
            foreach (var arg in _parser.Args)
            {
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
        private static Dictionary<string, Command> _commandSheet = new Dictionary<string, Command>();

        public static IEnumerable<Command> AllCommands
        {
            get
            {
                foreach (var c in _commandSheet.Values) yield return c;
            }
        }

        public static void AddCommandsFromNamespace(params string[] commandListNamespaces)
        {
            var namespaces = new HashSet<string>(commandListNamespaces);

            var typeList = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in typeList)
            {
                if (!namespaces.Contains(type.Namespace)) continue;
                try
                {
                    if (!type.IsSubclassOf(typeof(Command))) continue;
                    var command = (Command) Activator.CreateInstance(type);
                    if (command == null)
                    {
                        throw new InvalidOperationException($"Command {type} initialized to null. Oof.");
                    }
                    AddCommand(command);
                }
                catch (MissingMethodException)
                {
                    // It's a method here, so we ignore.
                }
            }
        }

        public static void AddCommand(Command command)
        {
            if (_commandSheet.ContainsKey(command.Name))
                throw new InvalidSetupException($"Duplicate commands of name {command.Name} found!");
            _commandSheet[command.Name] = command;
        }

        /// <summary>
        ///     Init but you pass it classes instead of namespaces by string. Makes it more type-safe.
        /// </summary>
        public static void AddCommandsFromNamespace(params Type[] oneTypeFromEachNamespaces)
        {
            var namespaces = oneTypeFromEachNamespaces.ToList().ConvertAll(type => type.Namespace).ToArray();
            AddCommandsFromNamespace(namespaces);
        }

        public static void Run(GamePlus game, string line)
        {
            var c = GetCommand(line);
            if (c != null)
                try
                {
                    c.Run(game, line);
                }
                catch (InvalidArgumentsException ae)
                {
                    throw new InvalidArgumentsException($"{ae.Message}\nUsage: {c.GetHelpRepresentation()}", ae);
                }
        }

        private static Command GetCommand(string line)
        {
            if (_commandSheet == null)
                throw new InvalidSetupException(
                    "You forgot to initialize the command catalogue thing! Call Commands.Init() somewhere at the start of the game to initialize this.");

            if (line.Length != 0)
            {
                var command = line;
                var firstSpace = line.IndexOf(' ');
                if (firstSpace != -1) command = line.Substring(0, firstSpace);

                if (!_commandSheet.ContainsKey(command))
                    throw new InvalidArgumentsException($"Command {command} does not exist.");

                return _commandSheet[command];
            }

            return null;
        }

        public static Command Get(string name)
        {
            return _commandSheet.ContainsKey(name) ? _commandSheet[name] : null;
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
        public InvalidArgumentsException(string message)
            : base(message)
        {
        }

        public InvalidArgumentsException(string message, Exception inner)
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
        public InvalidSetupException(string message)
            : base(message)
        {
        }

        public InvalidSetupException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    #endregion
}