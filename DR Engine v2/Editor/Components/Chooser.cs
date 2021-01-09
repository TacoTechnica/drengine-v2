using System;
using System.Collections.Generic;
using Gtk;
using Action = System.Action;

#pragma warning disable 693

namespace DREngine.Editor.Components
{
    /// <summary>
    /// Synchronously makes a choice.
    /// There might be different ways of doing this, like through a dropdown or a choice list window.
    /// </summary>

    public interface IChooser
    {

        public void MakeChoice<T>(IEnumerable<T> options, Action<T> onChoice, Func<T, string> toString = null);

        public void MakeChoiceOrDefault<T>(IEnumerable<T> options, Action<T> onChoice, Action onDefault, string defaultName, Func<T, string> toString = null);

        public void Cancel();
    }

    public class DropdownChooser : IChooser
    {
        private Menu _menu;

        private void Append(Menu menu, string name, Action onClick)
        {
            var item = new MenuItem(name);
            item.Activated += (sender, args) => { onClick?.Invoke(); };
            menu.Append(item);
            item.Show();
        }

        private Menu MakeMenu<T>(IEnumerable<T> options, Func<T, string> toString, Action<T> onChoice)
        {
            if (toString == null)
            {
                toString = arg => arg.ToString();
            }
            Menu result = new Menu();

            foreach (T option in options)
            {
                Append(result, toString(option), () =>
                {
                    onChoice.Invoke(option);
                });
            }

            return result;
        }

        public void MakeChoice<T>(IEnumerable<T> options, Action<T> onChoice, Func<T, string> toString = null)
        {
            _menu = MakeMenu(options, toString, onChoice);
            _menu.Popup();
        }

        public void MakeChoiceOrDefault<T>(IEnumerable<T> options, Action<T> onChoice, Action onDefault, string defaultName,
            Func<T, string> toString = null)
        {
            _menu = MakeMenu(options, toString, onChoice);
            Append(_menu, defaultName, onDefault);
            _menu.Popup();
        }

        public void Cancel()
        {
            if (_menu != null)
            {
                _menu.Popdown();
                _menu.Dispose();
                _menu = null;
            }
        }
    }
}
