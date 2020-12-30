using System;
using DREngine.Game.Scene;
using GameEngine.Game;

namespace DREngine.Game.CoreScenes
{
    public class ProjectMainMenuScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__PROJECT_MAIN_MENU__";

        private DRScene TestScene;

        public ProjectMainMenuScene(DRGame game) : base(game, SCENE_NAME)
        {
            // Load a DR Scene as a test. We will change this to be our menu scene later.
            TestScene = new DRScene(game, "TestScene", new ProjectPath(game, "TEST_SCENE.scene"), false);
        }

        public override void LoadScene()
        {
            TestScene.LoadScene();
        }
    }
}
