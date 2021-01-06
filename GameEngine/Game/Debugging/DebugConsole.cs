using System;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI.Debugging;

namespace GameEngine.Game.Debugging
{
    public class DebugConsole
    {
        private readonly GamePlus _game;
        private int _lineCounter;

        private readonly int _maxLines = 500;
        private readonly UIDebugConsole _ui;
        public Action OnClosed;

        public Action OnOpened;

        public DebugConsole(GamePlus game, Font font, float outputHeight, InputActionButton openAction,
            InputActionButton closeAction, InputActionButton submitAction)
        {
            _game = game;
            _ui = (UIDebugConsole) new UIDebugConsole(game, font, outputHeight).AddToRoot(true);
            openAction.Pressed += OnOpenPressed;
            closeAction.Pressed += OnClosePressed;
            submitAction.Pressed += OnSubmitPressed;

            Debug.OnLogPrint += OnGlobalLog;
            Debug.OnLogError += OnGlobalLogError;

            _ui.Active = false;
        }

        public bool Opened => _ui.Active;

        public Font Font => _ui.Font;

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
                var input = _ui.InputText;
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
                    PrintErrorToOutput(e.Message, e.StackTrace);
                }
            }
        }

        private void OnGlobalLogError(string text, string trace)
        {
            PrintErrorToOutput(text, trace);
        }

        private void OnGlobalLog(string text)
        {
            PrintToOutput(text);
        }

        #region External Control

        public void Open()
        {
            if (Opened) return;
            _ui.SetActive(true);
            _ui.SetFocused();
            OnOpened?.Invoke();
        }

        public void Close()
        {
            if (!Opened) return;
            _ui.SetActive(false);
            OnClosed?.Invoke();
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
                var newLineIndex = _ui.OutputText.IndexOf('\n');
                if (newLineIndex != -1) _ui.OutputText = _ui.OutputText.Substring(newLineIndex + 1);
                _lineCounter--;
            }

            var bottom = _ui.IsLogAtBottom();
            _ui.OutputText += text + "\n";
            if (bottom) _ui.MoveLogToBottom();
        }

        public void PrintErrorToOutput(string text, string trace)
        {
            PrintToOutput($"E: {text}");
        }

        #endregion
    }
}