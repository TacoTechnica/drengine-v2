using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DREngine.Game.Resources;
using DREngine.ResourceLoading;
using GameEngine.Game.Resources;
using Debug = GameEngine.Debug;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class GameResourceField<T> : AbstractFieldGameResource<T> where T : class, IGameResource
    {
        private DREditor _editor;

        private bool _loaded = false;

        public GameResourceField(DREditor editor) : base(editor, typeof(T))
        {
            _editor = editor;
        }

        protected override T ResourceData { get; set; }

        protected override string ResourceToString(T resource)
        {
            return resource.Path.GetShortName();
        }

        protected override void OnPathSelected(string path)
        {
            ProjectPath ppath = new ProjectPath(_editor, path);
            if (_editor.ResourceLoader.ResourceLoaded(ppath))
            {
                // If we're loaded, point to that resource.
                _loaded = true;
                ResourceData = _editor.ResourceLoader.GetResource<T>(ppath);
            }
            else
            {
                // Avoid loading. Might lead to problems but hopefully not.
                _loaded = false;
                ResourceData = NewEmptyResourceObject();
                ResourceData.Path = ppath;
            }
            // Trigger modification
            Data = ResourceData;
            OnModify();
        }

        protected override IEnumerable<string> GetExtraResults(Type t, string path)
        {
            yield break;
        }

        protected override bool AcceptPath(ProjectPath path)
        {
            return File.Exists(path);
        }

        protected abstract T NewEmptyResourceObject();
    }

    public class SpriteResourceField : GameResourceField<DRSprite>
    {
        public SpriteResourceField(DREditor editor) : base(editor)
        {
        }

        protected override DRSprite NewEmptyResourceObject()
        {
            return new DRSprite();
        }
    }
    public class AudioResourceField : GameResourceField<AudioClip>
    {
        public AudioResourceField(DREditor editor) : base(editor)
        {
        }

        protected override AudioClip NewEmptyResourceObject()
        {
            return new AudioClip();
        }
    }
    public class FontResourceField : GameResourceField<Font>
    {
        public FontResourceField(DREditor editor) : base(editor)
        {
        }

        protected override Font NewEmptyResourceObject()
        {
            return new Font();
        }
    }
}