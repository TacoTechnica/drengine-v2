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
        #region Global Command List

        private static List<Type> _commandTypes = null;

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
            foreach (Type type in
                Assembly.GetAssembly(typeof(VNCommand)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(VNCommand))))
            {
                commands.Add(type);
            }
        }

        #endregion

        /// <summary>
        /// A pointer to where we are in the script.
        /// </summary>
        [JsonIgnore] public int CommandIndex;
        public abstract string Type { get; set; }

        public abstract IEnumerator Run(DRGame game);
    }

    public class PrintCommand : VNCommand
    {
        public override string Type { get; set; } = "Print";

        public string Text;

        public override IEnumerator Run(DRGame game)
        {
            Debug.Log(Text);
            yield break;
        }
    }

    public class LabelCommand : VNCommand
    {
        public override string Type { get; set; } = "Label";

        public string Label;

        public override IEnumerator Run(DRGame game)
        {
            // Do nothing.
            yield break;
        }
    }
}
