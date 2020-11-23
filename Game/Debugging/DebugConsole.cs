using DREngine.Game.Input;
using DREngine.Game.UI.Debugging;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.Debugging
{
    public class DebugConsole
    {
        private UIDebugConsole _ui;
        public bool Opened => _ui.Active;

        private int _maxLines = 500;
        private int _lineCounter = 0;

        private GamePlus _game;

        public DebugConsole(GamePlus game, SpriteFont font, float outputHeight, InputActionButton openAction, InputActionButton closeAction, InputActionButton submitAction)
        {
            _game = game;
            _ui = (UIDebugConsole) new UIDebugConsole(game, font, outputHeight).AddToRoot();
            openAction.Pressed += OnOpenPressed;
            closeAction.Pressed += OnClosePressed;
            submitAction.Pressed += OnSubmitPressed;

            Debug.OnLog += OnGlobalLog;
            Debug.OnLogError += OnGlobalLogError;

            _ui.Active = false;
        }

        #region External Control

        public void Open()
        {
            if (Opened) return;
            _ui.SetActive(true);
            _ui.SetFocused();
        }

        public void Close()
        {
            if (!Opened) return;
            _ui.SetActive(false);
        }

        public void Clear()
        {
            _ui.OutputText = "";
        }

        public void PrintToOutput(string text)
        {
            _lineCounter++;
            while (_lineCounter > _maxLines)
            {
                int newLineIndex = _ui.OutputText.IndexOf('\n');
                if (newLineIndex != -1)
                {
                    _ui.OutputText = _ui.OutputText.Substring(newLineIndex + 1);
                }
                _lineCounter--;
            }
            bool bottom = _ui.IsLogAtBottom();
            _ui.OutputText += text + "\n";
            if (bottom)
            {
                _ui.MoveLogToBottom();
            }
        }

        public void PrintErrorToOutput(string text)
        {
            PrintToOutput($"E: {text}");
        }

        #endregion

        private void OnClosePressed(InputActionButton obj)
        {
            Close();
        }

        private void OnOpenPressed(InputActionButton obj)
        {
            Open();
        }

        private void OnSubmitPressed(InputActionButton obj)
        {
            if (Opened)
            {
                string input = _ui.InputText;
                // Ignore empty
                if (input == "") return;

                _ui.InputText = "";
                _ui.MoveLogToBottom();

                try
                {
                    Commands.Run(_game, input);
                }
                catch (InvalidArgumentsException e)
                {
                    PrintErrorToOutput(e.Message);
                }
            }
        }

        private void OnGlobalLogError(string text)
        {
            PrintErrorToOutput(text);
        }

        private void OnGlobalLog(string text)
        {
            PrintToOutput(text);
        }
    }
}
