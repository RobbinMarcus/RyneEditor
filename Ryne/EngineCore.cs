using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;
using Ryne.GameStates;
using Ryne.Scene;
using Ryne.Utility;
using Ryne.Utility.Collections;

namespace Ryne
{
    public class EngineCore : RyneEngineCore
    {
        private RyneApplication Application;
        public StateManager StateManager { get; }
        public EntityManager EntityManager { get; }

        // TODO: does not belong here
        public CircularArray<float> FrameTimes { get; private set; }

        public EngineCore()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            // Allow default resolver to write to private members in data for serializing private setters
            MessagePackSerializer.SetDefaultResolver(ContractlessStandardResolverAllowPrivate.Instance);

            StateManager = new StateManager();
            EntityManager = new EntityManager();

            FrameTimes = new CircularArray<float>(100);
        }

        public void Initialize(string title)
        {
            Logger.Initialize();

            CheckDirectories();

            Config config = Global.Config;
            RyneApplicationConfig appConfig = new RyneApplicationConfig
            {
                Width = config.Width,
                Height = config.Height,
                BorderlessFullscreen = config.BorderlessFullscreen,
                Fullscreen = config.Fullscreen,
                IsFixedTimeStep = config.UseFixedTimeStep,
                NoRendering = config.NoRendering
            };

            base.Initialize(title, Global.Config.WorkingDirectory, appConfig, Logger.EngineLog);
            Application = RyneGlobal.GlobalApplication;
            Int2 screenSize = Application.GetScreenSize();
            if (config.Width != screenSize.X || config.Height != screenSize.Y)
            {
                config.Width = screenSize.X;
                config.Height = screenSize.Y;
            }

            // Store globals
            Global.EngineCore = this;
            Global.StateManager = StateManager;
            Global.EntityManager = EntityManager;

            // Initialize managers
            StateManager.Initialize();
            EntityManager.Initialize();
            TimerManager.Initialize();
        }

        public void Update(float dt)
        {
            if (!Global.Config.NoRendering)
            {
                Application.Update(dt);
            }

            TimerManager.StartFrame();

            StateManager.Update(dt);

            if (!Global.Config.NoRendering)
            {
                Application.StartFrame();
                StateManager.Draw(dt);
                Application.EndFrame();
            }

            StateManager.PostFrame();

            TimerManager.EndFrame();
        }

        public void GameLoop()
        {
            Stopwatch timer = new Stopwatch();
            float updateTime = 0.0f;

            bool isFixedTimeStep = Global.Config.UseFixedTimeStep;
            float fixedTimeStep = Global.Config.FixedTimeStep;

            while(true)
            {
                if (!StateManager.IsRunning)
                {
                    break;
                }

                bool needsUpdate = !isFixedTimeStep || updateTime > fixedTimeStep - Constants.Epsilon;
                if (needsUpdate)
                {
                    float dt = isFixedTimeStep ? fixedTimeStep : updateTime;
                    Update(dt);
                    updateTime -= dt;
                }

                timer.Stop();
                float ms = (float) timer.Elapsed.TotalMilliseconds;
                if (needsUpdate)
                {
                    FrameTimes.Add(ms);
                }
                updateTime += ms * 0.001f;
                timer.Reset();
                timer.Start();
            }
        }

        private void CheckDirectories()
        {
            var contentDir = new DirectoryInfo(Global.Config.WorkingDirectory);

            CheckDirectory(contentDir.FullName);
            CheckDirectory(contentDir.FullName + "VoxelModels");
            CheckDirectory(contentDir.FullName + "Scenes");
            CheckDirectory(contentDir.FullName + "Models");
            CheckDirectory(contentDir.FullName + "Backgrounds");
            CheckDirectory(contentDir.FullName + "Textures");
        }

        private void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
