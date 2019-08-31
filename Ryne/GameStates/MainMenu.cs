using Ryne.Gui;
using Ryne.Utility;
using System;

namespace Ryne.GameStates
{
    class MainMenu : GameState
    {
        private bool Done;
        private float Timer;
        private readonly RyneTexture AlphaTexture;

        public MainMenu(StateManager manager) : base(manager)
        {
            // Setting timer to 0 will show the splash until the scene is loaded
            Timer = 0.0f;

            AlphaTexture = new RyneTexture();
            Done = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            AlphaTexture.Filename = "Textures/RyneEngineAlpha.jpg";
            AlphaTexture.Type = RyneTextureType.AlbedoTexture;
            if (!AlphaTexture.Load())
            {
                Logger.Warning("Could not load into texture");
            }
        }

        public override void Update(float dt)
        {
            if (Done)
            {
                StateManager.ExitCurrentState();
                return;
            }

            Timer -= dt;
            if (Timer < 0)
            {
                var sceneEditor = new SceneEditor(StateManager);
                sceneEditor.OnExitCallback += () => Done = true;
                StateManager.Add(sceneEditor);
            }
        }

        public override void Draw(float dt)
        {
            Global.Application.RenderTextureFullscreen(AlphaTexture, false);
        }
    }
}
