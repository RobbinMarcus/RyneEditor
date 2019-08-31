using System.Collections.Generic;
using Ryne.Entities;
using Ryne.Utility;

namespace Ryne.Scene.Systems
{
    public abstract class BaseSystem : IUpdatable
    {
        protected EntityManager EntityManager;
        public int ComponentMask;

        protected BaseSystem(EntityManager manager, int componentMask)
        {
            EntityManager = manager;
            ComponentMask = componentMask;
        }

        public abstract void RegisterEntity(Entity entity);

        protected IEnumerable<Entity> GetEntitiesWithMask()
        {
            foreach (var entity in EntityManager.Entities)
            {
                if (entity.Destroyed || !entity.Initialized)
                {
                    continue;
                }

                if ((entity.ComponentMask & ComponentMask) != ComponentMask)
                {
                    continue;
                }

                yield return entity;
            }
        }

        public abstract void Initialize();

        public abstract void Update(float dt);
    }
}
