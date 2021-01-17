using System;
using System.Reflection;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.VN;
using DREngine.ResourceLoading;
using GameEngine;
using Gtk;
using Type = System.Type;

namespace DREngine.Editor.SubWindows.Resources.VNEditor
{
    public class VNResourceWindow : ResourceWindow<VNScript>
    {

        private DREditor _editor;

        private VNCommandList _commands;
        private VNCommandPicker _picker;

        private Box _fieldBoxContainer;

        public VNResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            var vertPane = new HPaned();
            vertPane.WideHandle = true; // Separate left and right side
            vertPane.Position = 140; // How wide the left side starts

            _picker = new VNCommandPicker(_editor);
            _commands = new VNCommandList(_editor);

            _picker.NewCommandPlaced += (type, index) =>
            {
                if (index != -1)
                {
                    InsertNewCommand(index, type);
                }
            };

            _picker.LinkDragTo(_commands);

            _commands.OnCommandMove += (from, to) =>
            {
                VNCommand toRemove = CurrentResource.Commands[from];
                CurrentResource.Commands.RemoveAt(from);
                int toAddTo = to;
                if (toAddTo > from)
                {
                    toAddTo -= 1;
                }
                CurrentResource.Commands.Insert(toAddTo, toRemove);

                MarkDirty();
            };

            HBox rightSide = new HBox();
            VBox rightmostEdit = new VBox();
            _fieldBoxContainer = new VBox();
            rightmostEdit.Add(_fieldBoxContainer);
            _fieldBoxContainer.Show();

            rightSide.PackStart(_commands, true, true, 16);
            _commands.Show();
            rightSide.PackStart(rightmostEdit, true, true, 16);
            rightmostEdit.Show();

            vertPane.Pack1(_picker, false, false);
            _picker.Show();
            vertPane.Pack2(rightSide, true, false);
            rightSide.Show();

            container.PackStart(vertPane, true, true, 16);
            vertPane.Show();
            
            RequestMinSize(480, 300);
        }

        protected override void OnOpen(VNScript resource, Box container)
        {
            _commands.Clear();
            foreach (VNCommand command in resource.Commands)
            {
                _commands.AddCommand(command);
            }
            ClearCommandFields();
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            throw exception;
        }

        protected override void OnClose()
        {
            // Nothing.
        }

        private void InsertNewCommand(int index, Type type)
        {
            MarkDirty();
            Debug.Log($"NEW COMMAND: {index}: {type}");
            ConstructorInfo con = type.GetConstructor(System.Type.EmptyTypes);
            if (con == null)
            {
                throw new InvalidOperationException($"Command of type {type} does not contain default (empty) constructor!\n" +
                                                    "It must have one provided to be created from the VN Editor.");
            }

            VNCommand command = (VNCommand)Activator.CreateInstance(type);
            if (command == null)
            {
                throw new InvalidOperationException($"Failed to create VNCommand from type {type}. Make sure it extends VNCommand and has an empty constructor!");
            }

            // Update data
            CurrentResource.Commands.Insert(index, command);
            // Update visual
            _commands.InsertCommand(command, index);
        }

        private void ClearCommandFields()
        {
            // Remove children
            foreach (Widget child in _fieldBoxContainer.Children)
            {
                _fieldBoxContainer.Remove(child);
                child.Dispose();
            }
        }
        private void OpenCommandFields(VNCommand command)
        {
            ClearCommandFields();
            // Create fieldbox
            FieldBox b = new FieldBox(_editor, command.GetType(), true);
            b.LoadTarget(command);

            _fieldBoxContainer.PackStart(b, true, true, 16);
            b.Show();

            b.Modified += () =>
            {
                MarkDirty();
            };
        }
    }
}
