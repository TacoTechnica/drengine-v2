using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public abstract class VNCommand
    {
        /// <summary>
        ///     A pointer to where we are in the script.
        /// </summary>
        [JsonIgnore] public int CommandIndex;

        public abstract string Type { get; set; }

        public abstract IEnumerator Run(DRGame game);

        #region Global Command List

        private static List<Type> _commandTypes;

        public static List<Type> CommandTypes
        {
            get
            {
                if (_commandTypes == null)
                {
                    _commandTypes = new List<Type>();
                    LoadCommandTypeList(_commandTypes);
                }

                return _commandTypes;
            }
        }

        private static void LoadCommandTypeList(List<Type> commands)
        {
            // ReSharper disable once PossibleNullReferenceException
            foreach (var type in
                Assembly.GetAssembly(typeof(VNCommand)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(VNCommand))))
                commands.Add(type);
        }

        #endregion Management
    }

    public class PrintCommand : VNCommand
    {
        public string Text;
        public override string Type { get; set; } = "Print";

        public override IEnumerator Run(DRGame game)
        {
            Debug.Log($"VN PRINT: {Text}");
            yield break;
        }
    }

    public class LabelCommand : VNCommand
    {
        public string Label;
        public override string Type { get; set; } = "Label";

        public override IEnumerator Run(DRGame game)
        {
            // Do nothing.
            yield break;
        }
    }
}