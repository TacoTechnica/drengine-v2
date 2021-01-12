using System;
using System.Collections.Generic;
using DREngine.Game.VN;
using GameEngine;
using Gdk;
using Gtk;
using Drag = Gtk.Drag;
using Widget = Gtk.Widget;

namespace DREngine.Editor.SubWindows.Resources.VNEditor
{
    public class VNCommandList : ScrolledWindow
    {
        private DREditor _editor;

        public ListBox CommandListBox { get; }

        public int Count { get; private set; }

        private BaseCommandButton _hoverButton;

        private VNDragger _dragger;

        public Action<int, int> OnCommandMove;

        public IEnumerable<BaseCommandButton> Buttons
        {
            get
            {
                foreach (Widget w in CommandListBox.Children)
                {
                    if (w is BaseCommandButton b) yield return b;
                }
            }
        }

        public VNCommandList(DREditor editor)
        {
            _editor = editor;
            _dragger = new VNDragger();
            CommandListBox = new ListBox();
            this.Add(CommandListBox);
            CommandListBox.Show();

            HookupDragging();
        }

        public void Clear()
        {
            foreach (Widget child in CommandListBox.Children)
            {
                CommandListBox.Remove(child);
            }

            Count = 0;
        }

        public void AddCommand(VNCommand command)
        {
            InsertCommand(command, Count);
        }

        public void InsertCommand(VNCommand command, int index)
        {
            BaseCommandButton button = GetNewCommandButton(_editor, command);
            VBox buffer = new VBox();
            button.Initialize(buffer);
            CommandListBox.Insert(button, index);
            //buffer.Show();
            button.Show();
            button.HideBuffer();
            
            Count ++;
        }

        public void ShowBuffer(int bufferIndex)
        {
            _hoverButton?.HideBuffer();
            //Debug.Log($"Trying: {bufferIndex} ? {CommandListBox.Children.Length}");
            if (bufferIndex >= CommandListBox.Children.Length || bufferIndex == -1)
            {
                // Do nothing.
                _hoverButton = null;
            }
            else
            {
                _hoverButton = (BaseCommandButton) CommandListBox.Children[bufferIndex];
                _hoverButton.ShowBuffer();
            }
        }

        public void HideBuffer()
        {
            _hoverButton?.HideBuffer();

            _hoverButton = null;
        }

        private void MoveCommand(int prevIndex, int newIndex)
        {
            Debug.Log("Removing...");
            Widget toRemove = CommandListBox.Children[prevIndex];
            CommandListBox.Remove(toRemove);

            Debug.Log("Re-adding...");

            int placeToAdd = newIndex;
            if (newIndex > prevIndex)
            {
                // Account for the "shift back" after removing.
                placeToAdd -= 1;
            }

            CommandListBox.Insert(toRemove, placeToAdd);

            OnCommandMove?.Invoke(prevIndex, newIndex);
        }

        private BaseCommandButton GetNewCommandButton(DREditor editor, VNCommand command)
        {
            Type type = command.GetType();

            if (Is<DialogCommand>())
            {
                return new DialogueCommandButton(editor);
            }

            return new UnknownCommandButton(editor);

            bool Is<T>()
            {
                return typeof(T) == type;
            }
        }

        private void HookupDragging()
        {
            //Widget handle = button.GetDragHandle();
            Drag.SourceSet(CommandListBox, ModifierType.Button1Mask, null, DragAction.Move | DragAction.Copy);

            int start = -1;
            CommandListBox.DragBegin += (o, args) =>
            {
                var button = _dragger.GetHoveringButton(this);
                if (button == null)
                {
                    Debug.LogWarning("Failed to grab anything to drag...");
                }
                else
                {
                    start = button.Index;
                    Debug.Log($"DRAGGING: {start}");
                    //start = button.Index;
                    _dragger.DragBegin(this);
                }
            };

            CommandListBox.DragEnd += (o, args) =>
            {
                int end = _dragger.DragPlacementPos;
                _dragger.DragEnd(this);
                if (start == -1)
                {
                    Debug.LogWarning("This shouldn't happen on drag!");
                }
                else
                {
                    MoveCommand(start, end);
                }
            };
        }
    }
}
