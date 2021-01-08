using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine.Game;
using GameEngine.Game.Debugging;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    /// <summary>
    /// A test game class that we use for our tests.
    /// </summary>
    public class TestGame : GamePlus
    {
        public Font TestFont => DebugConsole.Font;

        private Color BACKGROUND => Color.SlateBlue;

        private IGameTester _tester;

        public TestGame() : base("Game Engine Test", "Content", true)
        {

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 120f);

            Commands.AddCommand(new TestSceneCommand());
        }

        #region Extra help with testing + simple background

        protected override void Update(GameTime gameTime)
        {
            if (RawInput.KeyPressed(Keys.F1))
            {
                Debug.LogDebug("Toggling Debug Collider Drawing");
                DebugDrawColliders = !DebugDrawColliders;
            }
            base.Update(gameTime);
            _tester?.Update(this.DeltaTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BACKGROUND);
            _tester?.Draw();
            base.Draw(gameTime);
        }

        public void SetTester(IGameTester tester)
        {
            if (_tester != tester)
            {
                _tester = tester;
                _tester.Initialize(this);
            }
        }

        class TestSceneCommand : Command
        {
            private static Dictionary<string, Type> _scenes; 
            public TestSceneCommand() : base("testscene", "Goes to a test scene", 
                new Arg<string>("sceneName", null, 0, false))
            {
                if (_scenes == null)
                {
                    _scenes = new Dictionary<string, Type>();
                    // Get all testcommands
                    var typeList = Assembly.GetExecutingAssembly().GetTypes();
                    foreach (var type in typeList)
                    {
                        if (typeof(IGameTester).IsAssignableFrom(type))
                        {
                            string name = type.Name;
                            _scenes[name] = type;
                        }
                    }
                }
            }

            protected override void Call(GamePlus game, ArgParser parser)
            {
                string name = parser.Get<string>();
                if (name == null)
                {
                    Log("SCENES:");
                    Log("###################");
                    foreach (string listed in _scenes.Keys)
                    {
                        Log($"    {listed}");
                    }
                    Log("###################");
                    return;
                }
                if (!_scenes.ContainsKey(name))
                {
                    LogError($"Invalid scene: {name}. Run without argument for a full list.");
                    return;
                }

                Type type = _scenes[name];
                IGameTester tester = (IGameTester) Activator.CreateInstance(type);
                if (tester == null)
                {
                    throw new InvalidOperationException($"IGameRunner {type} initialized to null. Oof.");
                }

                TestScene scene = new TestScene((TestGame)game, "TEST: " + type.Name, tester);
                game.SceneManager.LoadScene(scene);
            }
        }

        class TestScene : ISceneLoader
        {
            private string _name;
            private IGameTester _tester;
            private TestGame _game;
            public TestScene(TestGame game, string name, IGameTester tester)
            {
                _game = game;
                _name = name;
                _tester = tester;
            }
            public IEnumerable<string> GetNames()
            {
                yield return _name;
            }

            public void LoadScene()
            {
                _game.SetTester(_tester);
            }
        }

        #endregion

        public static void Main(string[] args) {
            Debug.InitRootDirectory();
            using (var game = new TestGame()) {
                game.Run();
            }
        }
    }

}
