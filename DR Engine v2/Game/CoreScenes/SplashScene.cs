using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Core;
using Color = Microsoft.Xna.Framework.Color;
using Layout = GameEngine.Game.UI.Layout;

using Path = GameEngine.Game.Path;

namespace DREngine.Game.CoreScenes
{
    /// <summary>
    /// The
    /// </summary>
    public class SplashScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__SPLASH_SCREEN__";
        private const string PROJECT_YAML_NAME = "project.yaml";
        private const string PROJECT_ICON_NAME = "icon.png";

        private UIHandler _ui;
        private DRGame _game;

        private string _projectDirectories;

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
                _ui.SetFailText($"Failed to find a directory for projects at {_projectDirectories}. Please make sure one exists!");
            }
        }

        private bool UpdateProjectList()
        {
            _ui.SetFailText("");
            _ui.ClearProjectOptions();

            if (!Directory.Exists(_projectDirectories)) return false;

            int count = 0;


            // Go through all directories and check inside for project.yaml
            foreach (string dir in Directory.GetDirectories(_projectDirectories))
            {
                Path projectPath = new Path(System.IO.Path.Combine(new [] {dir, PROJECT_YAML_NAME}));
                Path iconPath = new Path(System.IO.Path.Combine(new [] {dir, PROJECT_ICON_NAME}));
                Debug.Log($"TEST: {projectPath}");
                if (File.Exists(projectPath))
                {
                    // We have a project
                    try
                    {
                        ProjectData dat = ProjectData.ReadFromFile(_game.GraphicsDevice, projectPath, false);

                        string projectName = dat.Name;
                        string projectAuthor = dat.Author;

                        Sprite icon = null;
                        if (File.Exists(iconPath))
                        {
                            // We have an icon
                            icon = new Sprite(_game, iconPath);
                        }

                        _ui.AddProjectOption(projectName, projectAuthor, icon, () =>
                        {
                            LoadProjectFromPath(projectPath);
                        });
                        ++count;
                    }
                    catch (YamlException e)
                    {
                        Debug.LogDebug($"Failed to read project file at {projectPath}. YAML Output: {e.Message}");
                    }
                }
            }
            // FOR TESTING ONLY
            if (count != 0)
            {
                for (int i = 0; i < 10; ++i)
                {
                    _ui.AddProjectOption($"EMPTY PROJ {i}", "mee", null, () => { });
                }
            }

            if (count == 0)
            {
                _ui.SetFailText($"No projects found in {_projectDirectories}. Make sure all project folders are stored here, one folder per project within this path.");
            }
            return true;
        }

        private void LoadProjectFromPath(string projectPath)
        {
            _game.LoadProject(projectPath);
        }

        #region UI Handling

        class UIHandler : GameObject
        {
            private UIProjectList _projectList;
            private UIText _failText;
            private UIColoredRect _background;
            private UIText _header;
            private UIText _version;

            public UIHandler(DRGame game) : base(game)
            {
                Font font = game.ProjectResources.GetResource<Font>(game.GameProjectData.OverridableResources.MenuFont.GetFullPath(game));

                // Tinted Background
                Color dark = Color.Black;
                Color background = Color.Lerp(Color.DarkOliveGreen, dark, 0.6f);
                Color tinted = Color.Lerp(background, dark, 0.9f);
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
                    .WithoutWordWrap()
                    .WithHAlign(UIText.TextHAlignMode.Center)
                    .WithLayout(Layout.CenteredLayout())
                    .AddToRoot();

                _version = (UIText) new UIText(game, font, Program.Version, Color.White)
                    .WithHAlign(UIText.TextHAlignMode.Right)
                    .WithVAlign(UIText.TextVAlignMode.Bottom)
                    .WithoutWordWrap()
                    .WithLayout(Layout.CornerLayout(Layout.BottomRight, 0, 0).OffsetBy(-4, -4))
                    .AddToRoot();



            }

            public void SetFailText(string text)
            {
                _failText.Text = text;
                _failText.Active = (text != "");

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
        class UIProjectList : UIComponent
        {
            public Action<string> OnProjectPick = null;

            private MenuList _menu;
            private Font _font;

            private UIVerticalLayout _layout;

            private UIScrollView _scrollView;

            public UIProjectList(DRGame game, Font font, float childHeight, float spacing) : base(game)
            {
                _font = font;

                _menu = new MenuList(game, game.MenuControls.Select, game.MenuControls.MouseSelect, game.MenuControls.MoveDown, game.MenuControls.MoveUp);

                UIComponent background = new UIColoredRect(game, Color.Black, false, this);
                new UIColoredRect(game, Color.YellowGreen, true, this).CopyLayoutFrom(background);

                _layout = (UIVerticalLayout) new UIVerticalLayout(game, childHeight, spacing)
                    .PadLeft(6f)
                    .PadRight(6f)
                    .PadTop(6f)
                    .PadBottom(12f);

                float sliderWidth = 12;
                float sliderPad = 4;

                UISlider verticalSlider = (UISlider) new UISlider(game)
                    .WithLayout(Layout.SideStretchLayout(Layout.Right, sliderWidth, sliderPad, sliderPad));
                _scrollView = (UIScrollView) new UIScrollViewMasked(game, _layout, verticalSlider)
                    .WithContentLayout(Layout.SideStretchLayout(Layout.Top).WithMargin(new Margin(0, 0, sliderWidth + sliderPad*2, 0)))
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
                if (item is UIComponent itemui)
                {
                    _scrollView.FitRectInView(itemui.LayoutRect, _layout.Padding);
                }
            }

            public void ClearProjectOptions()
            {
                List<IMenuItem> toDestroy = new List<IMenuItem>(_menu.Children);
                foreach (IMenuItem item in toDestroy)
                {
                    if (item is UIComponent itemUI)
                    {
                        itemUI.DestroyImmediate();
                    }
                    _menu.RemoveChild(item);
                }
            }

            public void AddProjectOption(string projectName, string projectAuthor, Sprite icon, Action OnPress)
            {
                var button = new UIProjectOptionButton(_game, _font, projectName, projectAuthor, icon, _layout.ChildCount);
                _layout.AddChild(button);
                _menu.AddChild(button);

                button.Pressed += OnPress;
            }

            protected override void Draw(UIScreen screen, Rect targetRect) { }

            class UIProjectOptionButton : UIMenuButtonBase
            {
                private Color _deselectTextColor = Color.White;
                private Color _deselectBackgroundColor = Color.DarkSlateBlue;
                private Color _selectTextColor = Color.Black;
                private Color _selectBackgroundColor = Color.Lerp(Color.DarkSlateBlue, Color.Cornsilk, 0.5f);
                private Color _pressTextColor = Color.LightGoldenrodYellow;
                private Color _pressBackgroundColor = Color.LightSlateGray;

                private UIText _nameText;
                private UIText _authorText;
                private UIColoredRect _background;

                public UIProjectOptionButton(GamePlus game, Font font, string name, string author, Sprite icon, int index, UIComponent parent = null) : base(game, parent)
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
                    Color tinted = Color.Lerp(backgroundColor, Color.Black, 0.2f);
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
