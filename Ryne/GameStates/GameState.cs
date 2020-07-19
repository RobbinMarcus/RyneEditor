using Ryne.Entities;
using Ryne.Scene.Components;
using Ryne.Utility;

namespace Ryne.GameStates
{
    public class GameState : IDrawable
    {
        public StateManager StateManager { get; }
        public bool IsInitialized { get; private set; }

        public RyneSceneRenderData SceneRenderData { get; protected set; }

        public RyneNotifyFunction OnExitCallback;

        public GameState(StateManager manager)
        {
            StateManager = manager;
            IsInitialized = false;

            OnExitCallback = null;
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public virtual void Update(float dt)
        {
        }

        public virtual void Draw(float dt)
        {
        }

        public virtual void PostFrame()
        {
        }

        public virtual void RegisterEntity(Entity entity)
        {
            if (entity.RegisteredBackend)
            {
                Logger.Warning($"Already registered entity: {entity.Name}");
                return;
            }

            if (!entity.HasComponent<TransformComponent>())
            {
                Logger.Warning($"Tried to register entity without transform: {entity.Name}");
                return;
            }

            // Special case for lights, which don't need a MeshComponent
            if (entity is AreaLight light)
            {
                entity.RenderId = SceneRenderData.RegisterLight(light.ToRenderLight(), entity.Transform.ToRenderComponent());
                entity.SetFlag(EntityFlag.RegisteredBackend, true);
                return;
            }

            if (!entity.HasComponent<MeshComponent>())
            {
                return;
            }

            if (!entity.Mesh.Loaded)
            {
                entity.Mesh.OnMeshLoadedCallback += () => RegisterEntity(entity);
                return;
            }

            entity.RenderId = SceneRenderData.RegisterEntity(entity.Transform.ToRenderComponent(), entity.Mesh.ObjectType, entity.Mesh.GeometryIndex);
            entity.SetFlag(EntityFlag.RegisteredBackend, true);
            entity.Mesh.OnRegistered(entity);
        }

        public virtual void UnregisterEntity(Entity entity)
        {
            if (entity.RegisteredBackend)
            {
                if (SceneRenderData is null)
                {
                    Logger.Error($"Can't Unregister entity {entity.Name} from backend: SceneRenderData is null");
                }
                SceneRenderData.UnregisterEntity(entity.RenderId);
                entity.SetFlag(EntityFlag.RegisteredBackend, false);
            }
        }

        public virtual void Destroy()
        {
            OnExitCallback?.Invoke();
        }
    }
}
