using System;
using System.Collections.Generic;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace DREngine.Game
{
    public class TestSceneManager0 : IGameStarter
    {
        private BaseSceneLoader _scene0;
        private BaseSceneLoader _scene1;

        private GamePlus _game;
        private GraphicsDevice _graphics;

        public void Initialize(GamePlus game)
        {
            _game = game;
            _graphics = game.GraphicsDevice;
            Debug.LogDebug("TestGame Initialize()");

            _scene0 = new BaseSceneLoader(game, "scene0", (game) =>
            {
                new Camera3D(game);
                new TestTriangleObject(game, new Vector3(0, 0, -90), Quaternion.Identity);
                new TestTriangleObject(game, new Vector3(10, 10, -100), Quaternion.Identity);
            });
            _scene1 = new BaseSceneLoader(game, "scene1", (game) =>
            {
                new Camera3D(game, Vector3.Backward * 100, Math.FromEuler(0, 0, 45));
                new TestTriangleObject(game, new Vector3(0, 0, 0), Math.FromEuler(10, 10, -45));
                new TestTriangleObject(game, new Vector3(0, 40, -20), Math.FromEuler(0, -10, 0));
            });
        }

        public void Update(float deltaTime)
        {
            // Test basic camera rotation & FOV stuff

            if (Input.IsPressed(Keys.NumPad0))
            {
                Debug.Log("Going to scene 0");
                _game.SceneManager.LoadScene("scene0");
            }
            if (Input.IsPressed(Keys.NumPad1))
            {
                Debug.Log("Going to scene 1");
                _game.SceneManager.LoadScene("scene1");
            }
        }

        public void Draw()
        {

        }
    }

}
