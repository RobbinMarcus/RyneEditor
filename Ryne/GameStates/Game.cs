using Ryne.Entities;
using Ryne.Utility;

namespace Ryne.GameStates
{
    class Game : GameState
    {
        private RyneSceneRenderData SceneData;

        private Player Player;

        // TODO: rework editor support during game state
        private SceneEditor Editor;
        private bool FromEditor;
        private bool GameFocussed;

        public Game(StateManager manager) : base(manager)
        {
            SceneData = new RyneSceneRenderData();
            SceneData.SetRenderer(RyneRendererType.RayTracer);

            FromEditor = false;
            GameFocussed = true;

            // Set render transform to render the game full window
            Global.Application.SetRenderTransform(new Int2(0), new Int2(Global.Config.Width, Global.Config.Height));
        }

        public Game(SceneEditor editor) : base(editor.StateManager)
        {
            Editor = editor;
            FromEditor = true;
            GameFocussed = true;

            SceneData = editor.SceneRenderData;
        }

        public override void Initialize()
        {
            base.Initialize();
            Global.EntityManager.ClearAccelerationStructures();

            Player = new Player();
            Global.EntityManager.AddEntity(Player);
            SceneData.SetRenderCamera(Player.PlayerCamera);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (GameFocussed)
            {
                Global.EntityManager.Update(dt);
            }

            if (FromEditor)
            {
                Editor.Gui.Update(dt);

                if (Global.Application.KeyDown((int) RyneKey.LeftAlt) && Global.Application.KeyPressed((int) RyneKey.S))
                {
                    GameFocussed = !GameFocussed;
                    Player.PlayerController.SetCaptureMouse(GameFocussed);
                }
            }

            if (Global.Application.KeyDown((int)RyneKey.Escape))
            {
                StateManager.ExitCurrentState();
            }
        }

        public override void Draw(float dt)
        {
            SceneData.RenderScene();
            if (FromEditor)
            {
                Editor.Gui.Draw(dt);
            }
        }

        public override void PostFrame()
        {
            SceneData.PostFrame();
        }

        public override void Destroy()
        {
            foreach (var entity in Global.EntityManager.Entities)
            {
                entity.Destroy();
            }
            base.Destroy();
        }
    }
}
