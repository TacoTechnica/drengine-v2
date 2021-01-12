using System.Timers;
using GameEngine;
using Gdk;
using Gtk;
using Action=System.Action;

namespace DREngine.Editor.SubWindows.Resources.VNEditor
{
    public class VNDragger
    {
        private GUITicker _dragTicker;

        public int DragPlacementPos = -1;

        public VNDragger()
        {
            _dragTicker = new GUITicker(50);
        }

        public void DragBegin(VNCommandList commands)
        {
            _dragTicker.OnTick = () =>
            {
                DragPlacementPos = GetHoverPlacementPos(commands);
                commands.ShowBuffer(DragPlacementPos);
            };
            _dragTicker.Stop();
            _dragTicker.Start();
        }

        public void DragEnd(VNCommandList commands)
        {
            //Debug.Log($"Drag Stopped: {dragPlacementPos}");
            DragPlacementPos = -1;
            _dragTicker.Stop();
            commands.HideBuffer();
        }

        #region Utils

        public BaseCommandButton GetHoveringButton(VNCommandList commands)
        {
            // Get mouse pos
            int mx, my;
            commands.Window.Screen.Display.GetPointer(out mx, out my);
            int winx, winy;
            commands.Window.GetOrigin(out winx, out winy);

            Rectangle listRect = WidgetRect(commands, winx, winy, 8, 16);
            bool inside = listRect.Contains(mx, my);
            if (inside)
            {
                foreach (BaseCommandButton child in commands.Buttons)
                {
                    Rectangle r = WidgetRect(child, winx, winy);
                    if (r.Contains(mx, my))
                    {
                        return child;
                    }
                }
            }

            return null;
        }
        
        private int GetHoverPlacementPos(VNCommandList commands)
        {
            // Get mouse pos
            int mx, my;
            commands.Window.Screen.Display.GetPointer(out mx, out my);
            int winx, winy;
            commands.Window.GetOrigin(out winx, out winy);

            Rectangle listRect = WidgetRect(commands, winx, winy, 8, 16);
            bool inside = listRect.Contains(mx, my);
            if (inside)
            {
                int targetPos = -1;
                // For each child...
                int childIndex = 0;
                foreach (Widget child in commands.Buttons)
                {
                    if (!(child is BaseCommandButton)) continue;
                    Rectangle r = WidgetRect(child, winx, winy);
                    if (r.Contains(mx, my))
                    {
                        int deltaY = my - r.Top;
                        if (deltaY > r.Height / 2)
                        {
                            //Debug.Log($"CHILD {childIndex} LOWER");
                            // Lower half
                            targetPos = childIndex + 1;
                        }
                        else
                        {
                            //Debug.Log($"CHILD {childIndex} UPPER");
                            // Upper half
                            targetPos = childIndex;
                        }
                        break;
                    }

                    ++childIndex;
                }

                if (targetPos == -1)
                {
                    // We found nothing.
                    int deltaY = my - listRect.Top;
                    if (deltaY > 20) // Arbitrary. We want top to only contain the very top smidget.
                    {
                        //Debug.Log("BOTTOM");
                        // Lower half
                        targetPos = commands.Count;
                    }
                    else
                    {
                        //Debug.Log("TOP");
                        // Upper half
                        targetPos = 0;
                    }
                }
                return targetPos;
            }

            return -1;
        }

        private Rectangle WidgetRect(Widget w, int winx, int winy, int pushX = 0, int pushY = 0)
        {
            int wx = w.Allocation.X + winx - pushX;
            int wy = w.Allocation.Y + winy - pushY;
            //w.TranslateCoordinates(w.Toplevel, 0, 0, out int wx, out int wy);
            //wx += winx;
            //wy += winy;
            return new Rectangle(wx, wy, w.AllocatedWidth + pushX*2, w.AllocatedHeight + pushY*2);
        }
        private class GUITicker
        {
            public double IntervalMs
            {
                get => _timer.Interval;
                set => _timer.Interval = value;
            }

            private volatile Action _onTick;

            public Action OnTick
            {
                get => _onTick;
                set
                {
                    lock (_lock)
                    {
                        _onTick = value;
                    }
                }
            }

            private Timer _timer;

            private object _lock = new object();

            private bool _stopped = true;

            public GUITicker(int intervalMs)
            {
                OnTick = null;
                _timer = new Timer(intervalMs);
                _timer.Elapsed += (sender, args) =>
                {
                    lock (_lock)
                    {
                        Application.Invoke((obj, args2) =>
                        {
                            lock (_lock)
                            {
                                if (_stopped)
                                {
                                    return;
                                }

                                OnTick?.Invoke();
                            }
                        });
                    }
                };
            }

            public void Start()
            {
                lock (_lock)
                {
                    _stopped = false;
                    _timer.Start();
                }
            }

            public void Stop()
            {
                lock (_lock)
                {
                    _stopped = true;
                    _timer.Stop();
                }
            }
        }
        #endregion
    }
}