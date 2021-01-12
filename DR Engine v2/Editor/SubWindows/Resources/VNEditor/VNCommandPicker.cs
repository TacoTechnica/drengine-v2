using System;
using System.Threading.Tasks;
using System.Timers;
using DREngine.Game.VN;
using Gdk;
using Gtk;
using Action = System.Action;
using Debug = GameEngine.Debug;
using Drag = Gtk.Drag;

namespace DREngine.Editor.SubWindows.Resources.VNEditor
{
    public class VNCommandPicker : ToolPalette
    {

        public Action<Type, int> NewCommandPlaced;

        private VNDragger _dragger;
        
        public VNCommandPicker(DREditor editor)
        {
            _dragger = new VNDragger();
            AddGroup("Dialog");
            AddButton(editor.Icons.UnknownFile, "Dialog", typeof(DialogCommand));
            AddGroup("Control");
            AddButton(editor.Icons.UnknownFile, "Label", typeof(LabelCommand));
            AddGroup("Misc");
            AddButton(editor.Icons.UnknownFile, "Print", typeof(PrintCommand));
        }

        private ToolItemGroup _cachedLastGroup = null;

        private void AddGroup(string name)
        {
            _cachedLastGroup = new ToolItemGroup(name);
            this.Add(_cachedLastGroup);
            _cachedLastGroup.Show();
        }

        private void AddButton(Pixbuf icon, string label, Type type)
        {
            ToolButtonVN b = new ToolButtonVN(icon, label, type);
            if (_cachedLastGroup == null) throw new InvalidOperationException("Didn't create group before making button, this is entirely the devs fault.");

            _cachedLastGroup.Insert(b, (int) _cachedLastGroup.NItems);
            b.Show();
        }

        public void LinkDragTo(VNCommandList commands)
        {
            foreach (Widget w in this.Children)
            {
                if (w is ToolItemGroup group)
                {
                    foreach (Widget child in group.Children)
                    {
                        if (child is ToolButtonVN vnb)
                        {
                            vnb.HookupDrag(commands);
                            vnb.DragBegin += (o, args) =>
                            {
                                _dragger.DragBegin(commands);
                            };
                            vnb.DragEnd += (o, args) =>
                            {
                                int toAdd = _dragger.DragPlacementPos;
                                _dragger.DragEnd(commands);
                                NewCommandPlaced?.Invoke(vnb.CommandType, toAdd);
                            };
                        }
                    }
                }
            }
        }
        

        #region Helper Classes

        private class ToolButtonVN : ToolButton
        {
            public Type CommandType;

            public ToolButtonVN(Pixbuf icon, string label, Type type) : base(new Image(icon), label)
            {
                CommandType = type;
            }

            public void HookupDrag(VNCommandList commands)
            {
                this.UseDragWindow = true;
                Drag.SourceSet(this, ModifierType.Button1Mask, null, DragAction.Move);
            }
        }

        #endregion
    }
}
