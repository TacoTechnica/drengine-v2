using System;
using System.Collections.Generic;
using System.Reflection;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class GenericEnumWidget : FieldWidget<Enum>
    {
        private ComboBoxText _chooser;

        private readonly Dictionary<string, int> _nameToValueMap = new Dictionary<string, int>();

        private Type _type;

        protected override Enum Data
        {
            get
            {
                var name = _chooser.ActiveText;
                if (!_nameToValueMap.ContainsKey(name))
                    throw new InvalidOperationException($"Invalid enum name: {name}");

                return (Enum) Enum.ToObject(_type, _nameToValueMap[name]);
            }
            set
            {
                TreeIter result;
                if (!GetIter(out result, value.ToString()))
                    throw new InvalidOperationException($"Invalid enum name: {value}");
                _chooser.SetActiveIter(result);
            }
        }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            if (field is FieldInfo finfo)
            {
                _type = finfo.FieldType;
            } else if (field is PropertyInfo pinfo)
            {
                _type = pinfo.PropertyType;
            }

            _nameToValueMap.Clear();

            _chooser = new ComboBoxText();


            foreach (int value in Enum.GetValues(_type))
            {
                var name = Enum.GetName(_type, value);
                _nameToValueMap[name] = value;
                _chooser.AppendText(name);
            }

            //_chooser.FinishEditing();
            content.PackStart(_chooser, true, true, 16);
            _chooser.Show();

            //_chooser.ActiveId = entries[0];
            TreeIter first;
            _chooser.Model.GetIterFirst(out first);
            _chooser.SetActiveIter(first);
            /*if (!_chooser.SetActiveId(entries[0]))
            {
                throw new InvalidOperationException($"Failed to set chooser starting value to {entries[0]}. This is bad.");
            }
            */

            _chooser.Changed += (sender, args) => { OnModify(); };
        }

        private bool GetIter(out TreeIter iter, string name)
        {
            _chooser.Model.GetIterFirst(out iter);

            while (true)
            {
                var val = (string) _chooser.Model.GetValue(iter, 0);

                if (val == name) return true;

                if (!_chooser.Model.IterNext(ref iter)) return false;
            }
        }
    }
}