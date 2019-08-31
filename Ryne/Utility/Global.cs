using Ryne.GameStates;
using Ryne.Scene;

namespace Ryne.Utility
{
    public class Global
    {
        public static Config Config = new Config();

        // Set from EngineCore
        public static EngineCore EngineCore;
        public static StateManager StateManager;
        public static EntityManager EntityManager;

        // References to globals from bindings
        public static RyneApplication Application => RyneGlobal.GlobalApplication;
        public static RyneResourceManager ResourceManager => RyneGlobal.GlobalResourceManager;
    }
}
