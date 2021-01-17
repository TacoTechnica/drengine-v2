using System.Collections.Generic;
using DREngine.Game.Scene;
using DREngine.Game.VN;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game.Objects;

namespace DREngine.Game.CoreScenes
{
    public class ProjectMainMenuScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__PROJECT_MAIN_MENU__";

        private readonly DRScene _testScene;

        private DRGame _game;

        public ProjectMainMenuScene(DRGame game) : base(game, SCENE_NAME)
        {
            _game = game;
            // Load a DR Scene as a test. We will change this to be our menu scene later.
            //_testScene = new DRScene(game, "TestScene", new ProjectPath(game, "TEST_SCENE.scene"));
        }

        public override void LoadScene()
        {
            var script = new VNScript(_game);

            script.Commands = new List<VNCommand>(new VNCommand[]
            {
                new LabelCommand {Label = "STARTO"},
                new DialogCommand {Name = "Arin", Text = "At age 6 I was born without a face."},
                new PrintCommand {Text = "1 poopity scoop"},
                new DialogCommand {Name = "Obama", Text = "Hello. I am 44th president of the United States Barack Obama."},
                new PrintCommand {Text = "2 scoopity poop"},
                new LabelCommand {Label = "ENDO"}
            });

            Debug.Log("STARTING NEW SCRIPT");
            _game.VNRunner.CallScript(script);
            
            //_testScene.LoadSceneRaw();
        }
    }
}