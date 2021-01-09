using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DREngine.Game.CoreScenes
{
    /// <summary>
    ///     The
    /// </summary>
    public class SplashScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__SPLASH_SCREEN__";
        private const string PROJECT_FILE_NAME = "project.json";
        private const string PROJECT_ICON_NAME = "icon.png";
        private readonly DRGame _game;

        private readonly string _projectDirectories;

        private UIHandler _ui;

        public SplashScene(DRGame game, string projectDirectories) : base(game, SCENE_NAME)
        {
            _game = game;
            _projectDirectories = new EnginePath(projectDirectories);
        }

        public override void LoadScene()
        {
            _ui = new UIHandler(_game);
            if (!UpdateProjectList())
            {
                _ui.ClearProjectOptions();
                _ui.SetFailText(
                    $"Failed to find a directory for projects at {_projectDirectories}. Please make sure one exists!");
            }
        }

        private bool UpdateProjectList()
        {
            _ui.SetFailText("");
            _ui.ClearProjectOptions();

            if (!Directory.Exists(_projectDirectories)) return false;

            var count = 0;


            // Go through all directories and check inside for project.json
            foreach (var dir in Directory.GetDirectories(_projectDirectories))
            {
                var projectPath = new GameEngine.Game.Path(Path.Combine(new[] {dir, PROJECT_FILE_NAME}));
                var iconPath = new GameEngine.Game.Path(Path.Combine(new[] {dir, PROJECT_ICON_NAME}));
                Debug.Log($"TEST: {projectPath}");
                if (File.Exists(projectPath))
                    // We have a project
                    try
                    {
                        var dat = ProjectData.LoadFromFile(projectPath, false);

                        var projectName = dat.Name;
                        var projectAuthor = dat.Author;

                        Sprite icon = null;
                        if (File.Exists(iconPath))
                            // We have an icon
                            icon = new Sprite(_game, iconPath);

                        _ui.AddProjectOption(projectName, projectAuthor, icon,
                            () => { LoadProjectFromPath(projectPath); });
                        ++count;
                    }
                    catch (JsonException e)
                    {
                        Debug.LogDebug($"Failed to read project file at {projectPath}. JSON Output: {e.Message}");
                    }
            }

            // FOR TESTING ONLY
            if (count != 0)
                for (var i = 0; i < 10; ++i)
                    _ui.AddProjectOption($"EMPTY PROJ {i}", "mee", null, () => { });

            if (count == 0)
                _ui.SetFailText(
                    $"No projects found in {_projectDirectories}. Make sure all project folders are stored here, one folder per project within this path.");
            return true;
        }

        private void LoadProjectFromPath(string projectPath)
        {
            _game.LoadProject(projectPath);
        }

        #region UI Handling

        private class UIHandler : GameObject
        {
            private readonly UIColoredRect _background;
            private readonly UIText _failText;
            private readonly UIText _header;
            private readonly UIProjectList _projectList;
            private readonly UIText _version;

            public UIHandler(DRGame game) : base(game)
            {
                var font = game.ResourceLoader.GetResource<Font>(game.GameData.OverridableResources.MenuFont
                    .GetFullPath(game));

                // Tinted Background
                var dark = Color.Black;
                var background = Color.Lerp(Color.DarkOliveGreen, dark, 0.6f);
                var tinted = Color.Lerp(background, dark, 0.9f);
                _background = (UIColoredRect) new UIColoredRect(game, background, tinted, tinted, background)
                    .AddToRoot();

                // Header
                _header = (UIText) new UIText(game, font, "DR Engine v2", Color.White)
                    .WithHAlign(UIText.TextHAlignMode.Center)
                    .WithVAlign(UIText.TextVAlignMode.Middle)
                    .WithLayout(Layout.SideStretchLayout(Layout.Top, 64))
                    .AddToRoot();

                // Project list & text
                _projectList = (UIProjectList) new UIProjectList(game, font, 64, 10f)
                    .WithLayout(Layout.FullscreenLayout(128, 64 + 64 + 12, 128, 64))
                    .AddToRoot();
                _failText = (UIText) new UIText(game, font, "", Color.White)
                    .WithHAlign(UIText.TextHAlignMode.Center)
                    .WithLayout(Layout.CustomLayout(0, 0.5f, 1, 0.5f))
                    .AddToRoot();

                _version = (UIText) new UIText(game, font, Program.Version, Color.White)
                    .WithHAlign(UIText.TextHAlignMode.Right)
                    .WithVAlign(UIText.TextVAlignMode.Bottom)
                    .WithoutWordWrap()
                    .WithLayout(Layout.CornerLayout(Layout.BottomRight).OffsetBy(-4, -4))
                    .AddToRoot();
            }

            public void SetFailText(string text)
            {
                _failText.Text = text;
                _failText.Active = text != "";

                _projectList.Active = !_failText.Active;
            }

            public void AddProjectOption(string projectName, string projectAuthor, Sprite icon, Action onPress)
            {
                _projectList.AddProjectOption(projectName, projectAuthor, icon, onPress);
            }

            public void ClearProjectOptions()
            {
                _projectList.ClearProjectOptions();
            }

            public override void OnDestroy()
            {
                Debug.Log("BOOM");
                _projectList.DestroyImmediate();
                _background.DestroyImmediate();
                _failText.DestroyImmediate();
                _header.DestroyImmediate();
                _version.DestroyImmediate();
            }
        }

        /// <summary>
        ///     Picks a project from a list of options, displaying some info.
        /// </summary>
        private class UIProjectList : UIComponent
        {
            private readonly Font _font;

            private readonly UIVerticalLayout _layout;

            private readonly MenuList _menu;

            private readonly UIScrollView _scrollView;

            public UIProjectList(DRGame game, Font font, float childHeight, float spacing) : base(game)
            {
                _font = font;

                _menu = new MenuList(game, game.MenuControls.Select, game.MenuControls.MouseSelect,
                    game.MenuControls.MoveDown, game.MenuControls.MoveUp);

                UIComponent background = new UIColoredRect(game, Color.Black, false, this);
                new UIColoredRect(game, Color.YellowGreen, true, this).CopyLayoutFrom(background);

                _layout = new UIVerticalLayout(game, childHeight, spacing)
                    .PadLeft(6f)
                    .PadRight(6f)
                    .PadTop(6f)
                    .PadBottom(12f);

                float sliderWidth = 12;
                float sliderPad = 4;

                var verticalSlider = (UISlider) new UISlider(game)
                    .WithLayout(Layout.SideStretchLayout(Layout.Right, sliderWidth, sliderPad, sliderPad));
                _scrollView = (UIScrollView) new UIScrollViewMasked(game, _layout, verticalSlider)
                    .WithContentLayout(Layout.SideStretchLayout(Layout.Top)
                        .WithMargin(new Margin(0, 0, sliderWidth + sliderPad * 2, 0)))
                    .WithLayout(Layout.FullscreenLayout());
                AddChild(_scrollView);
                AddChild(verticalSlider);

                // Background
                background.CopyLayoutFrom(_scrollView);

                // When we select something, focus our scroll rect to it.
                _menu.ItemSelected += OnMenuSelected;
            }

            [SuppressMessage("ReSharper", "DelegateSubtraction")]
            public override void DestroyImmediate()
            {
                _menu.Destroy();
                _menu.ItemSelected -= OnMenuSelected;
                base.DestroyImmediate();
            }

            private void OnMenuSelected(IMenuItem item)
            {
                if (item is UIComponent itemui) _scrollView.FitRectInView(itemui.LayoutRect, _layout.Padding);
            }

            public void ClearProjectOptions()
            {
                var toDestroy = new List<IMenuItem>(_menu.Children);
                foreach (var item in toDestroy)
                {
                    if (item is UIComponent itemUI) itemUI.DestroyImmediate();
                    _menu.RemoveChild(item);
                }
            }

            public void AddProjectOption(string projectName, string projectAuthor, Sprite icon, Action onPress)
            {
                var button =
                    new UIProjectOptionButton(Game, _font, projectName, projectAuthor, icon, _layout.ChildCount);
                _layout.AddChild(button);
                _menu.AddChild(button);

                button.Pressed += onPress;
            }

            protected override void Draw(UIScreen screen, Rect targetRect)
            {
            }

            private class UIProjectOptionButton : UIMenuButtonBase
            {
                private readonly UIText _authorText;
                private readonly UIColoredRect _background;
                private readonly Color _deselectBackgroundColor = Color.DarkSlateBlue;
                private readonly Color _deselectTextColor = Color.White;

                private readonly UIText _nameText;
                private readonly Color _pressBackgroundColor = Color.LightSlateGray;
                private readonly Color _pressTextColor = Color.LightGoldenrodYellow;
                private readonly Color _selectBackgroundColor = Color.Lerp(Color.DarkSlateBlue, Color.Cornsilk, 0.5f);
                private readonly Color _selectTextColor = Color.Black;

                public UIProjectOptionButton(GamePlus game, Font font, string name, string author, Sprite icon,
                    int index, UIComponent parent = null) : base(game, parent)
                {
                    _background = (UIColoredRect) new UIColoredRect(game, _deselectBackgroundColor, false, this)
                        .WithLayout(Layout.FullscreenLayout());

                    float padX = 8,
                        padY = 8;
                    float dx = padX,
                        dy = padY;

                    float spriteMaxWidth = 64;

                    // Icon
                    new UISprite(game, icon, this) {PreserveAspect = true}
                        .WithLayout(Layout.SideStretchLayout(Layout.Left, spriteMaxWidth, padY, padX));

                    dx += spriteMaxWidth + padX;

                    // Position text
                    _nameText = (UIText) new UIText(game, font, name, _deselectTextColor, this)
                        .WithoutWordWrap()
                        .WithLayout(Layout.CornerLayout(Layout.TopLeft).OffsetBy(dx, dy));
                    dy += 32;
                    _authorText = (UIText) new UIText(game, font, "by " + author, _deselectTextColor, this)
                        .WithoutWordWrap()
                        .WithLayout(Layout.CornerLayout(Layout.TopLeft).OffsetBy(dx, dy));

                    SetVisualState(_deselectTextColor, _deselectBackgroundColor);

                    // Tween in for fun
                    Tweener.TweenPositionXDelta(-500, 0.5f).Immediate().SetEaseExpoOut().SetDelay(0.1f * index);
                }

                protected override void Draw(UIScreen screen, Rect targetRect)
                {
                    // Handled by children.
                }

                protected override void OnSelectVisual()
                {
                    SetVisualState(_selectTextColor, _selectBackgroundColor);
                }

                protected override void OnDeselectVisual()
                {
                    SetVisualState(_deselectTextColor, _deselectBackgroundColor);
                }

                protected override void OnPressVisual()
                {
                    SetVisualState(_pressTextColor, _pressBackgroundColor);
                }

                protected override void OnDepressVisual()
                {
                    //SetVisualState(_selectTextColor, _selectBackgroundColor);
                }

                private void SetVisualState(Color textColor, Color backgroundColor)
                {
                    // Shade the bottom part for fun.
                    var tinted = Color.Lerp(backgroundColor, Color.Black, 0.2f);
                    _background.Tweener.CancelAll();
                    _background.Tweener.TweenBackgroundColor(backgroundColor, backgroundColor, tinted, tinted, 0.3f)
                        .SetEaseExpoOut();
                    // Text
                    _nameText.Tweener.CancelAll();
                    _nameText.Tweener.TweenTextColor(textColor, 0.3f).SetEaseCircOut();
                    _authorText.Tweener.CancelAll();
                    _authorText.Tweener.TweenTextColor(textColor, 0.3f).SetEaseCircOut();
                }
            }
        }

        #endregion
    }
}