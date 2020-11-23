using DREngine.Game.Input;
using DREngine.Game.UI.Debugging;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.Debugging
{
    public class DebugConsole
    {
        private UIDebugConsole _ui;
        public bool Opened => _ui.Active;

        private DebugConsoleExecutor _executor;

        private int _maxLines = 500;
        private int _lineCounter = 0;

        public DebugConsole(GamePlus game, SpriteFont font, float outputHeight, InputActionButton openAction, InputActionButton closeAction, InputActionButton submitAction)
        {
            _ui = (UIDebugConsole) new UIDebugConsole(game, font, outputHeight).AddToRoot();
            openAction.Pressed += OnOpenPressed;
            closeAction.Pressed += OnClosePressed;
            submitAction.Pressed += OnSubmitPressed;

            Debug.OnLog += OnGlobalLog;
            Debug.OnLogError += OnGlobalLogError;

            _ui.Active = false;

            _executor = new DebugConsoleExecutor();
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

                _executor.ExecuteInput(input);
            }
        }

        private void OnGlobalLogError(string text)
        {
            PrintToOutput($"E: {text}");
        }

        private void OnGlobalLog(string text)
        {
            PrintToOutput(text);
        }
    }
}
