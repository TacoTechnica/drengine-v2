using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine.Game.Objects;

namespace DREngine.Game.CoreScenes
{
    public class ProjectMainMenuScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__PROJECT_MAIN_MENU__";

        private readonly DRScene _testScene;

        public ProjectMainMenuScene(DRGame game) : base(game, SCENE_NAME)
        {
            // Load a DR Scene as a test. We will change this to be our menu scene later.
            _testScene = new DRScene(game, "TestScene", new ProjectPath(game, "TEST_SCENE.scene"));
        }

        public override void LoadScene()
        {
            _testScene.LoadScene();
        }
    }
}