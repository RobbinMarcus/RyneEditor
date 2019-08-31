using Ryne.Entities;
using Ryne.Scene.Components;

namespace Ryne.Scene.Systems
{
    class PhysicsSystem : BaseSystem
    {
        private EventSystem Events;

        //private float FloorZ;

        public PhysicsSystem(EntityManager manager) : base(manager, 
            Components.Components.GetComponentIndex<PhysicsComponent>()
            | Components.Components.GetComponentIndex<TransformComponent>())
        {
            //FloorZ = 0.0f;
        }

        public override void RegisterEntity(Entity entity)
        {
        }

        public override void Initialize()
        {
            Events = EntityManager.GetSystemOfType<EventSystem>();
        }

        public override void Update(float dt)
        {
            // TODO:
            // This is a very simple physics system that just applies acceleration for now
            // It still needs
            // - Angular momentum
            // - Proper friction model
            foreach (var entity in GetEntitiesWithMask())
            {
                ref var transform = ref entity.Transform;
                ref var physics = ref entity.Physics;

                transform.PreviousPosition = transform.Position;

                // Add forces to the component
                AddForces(ref physics);

                physics.Velocity += physics.Acceleration * dt;
                transform.Position += physics.Velocity * dt;

                // Clear forces
                physics.Acceleration = new Float4(0.0f);

                // Apply friction
                physics.Velocity *= 0.99f;
            }
        }

        private void AddForces(ref PhysicsComponent physics)
        {
            // TODO: Only apply gravity when not on a surface
            if (!physics.OnSurface)
            {
                physics.Acceleration += new Float4(0, 0, -10, 0);
            }
        }
    }
}
