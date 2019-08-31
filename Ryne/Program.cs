using System;
using Ryne.GameStates;
using Ryne.Utility;

namespace Ryne
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
                EngineCore engine = new EngineCore();
                engine.Initialize("Ryne");

                var stateManager = engine.StateManager;
                var mainMenu = new MainMenu(stateManager);
                stateManager.Add(mainMenu);

                engine.GameLoop();
                engine.Dispose();
#if !DEBUG
            }
            catch (Exception e)
            {
                // Log exception before throwing
                Logger.Error(e.Message + "\n" + e.StackTrace);
                Logger.Flush();
                throw e;
            }
#endif
        }
    }
}