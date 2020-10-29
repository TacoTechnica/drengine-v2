using DREngine.Game.Input;
using DREngine.Game.Tween;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class TestTween : IGameRunner
    {

        private ExampleTriangleObject _t1;
        private ExampleTriangleObject _t2;

        public void Initialize(GamePlus game)
        {
            new Camera3D(game, Vector3.Backward * 100);
            _t1 = new ExampleTriangleObject(game, new Vector3(-30, 0, 0), Quaternion.Identity);
            _t2 = new ExampleTriangleObject(game, new Vector3(30, 0, 0), Quaternion.Identity);
        }

        public void Update(float deltaTime)
        {
            if (RawInput.KeyPressed(Keys.A))
            {
                _t1.Tweener.CancelAll();
                _t1.Tweener.TweenPosition(new Vector3(-60, 30, 0), 1f)
                    .SetEaseElasticOut()
                    .SetOvershoot(1f)
                    .SetOnComplete(() =>
                    {
                        Debug.Log("Target Reached!");
                    })
                    .SetOnCancel(() =>
                    {
                        Debug.Log("Tween CANCELLED");
                    });
            }
            if (RawInput.KeyPressed(Keys.D))
            {
                _t1.Tweener.CancelAll();
                _t1.Tweener.TweenPosition(new Vector3(0, 0, 0), 1f).SetEase(EaseType.CircOut).SetDelay(0.5f);
            }
        }

        public void Draw()
        {

        }
    }
}
