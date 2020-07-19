using Ryne.Entities;
using Ryne.Scene.Components;

namespace Ryne.Scene.Systems
{
    class PhysicsSystem : BaseSystem
    {
        private EventSystem Events;

        private readonly bool UseGravity;
        private readonly bool UseFriction;

        public PhysicsSystem(EntityManager manager) : base(manager, 
            Components.Components.GetComponentIndex<PhysicsComponent>()
            | Components.Components.GetComponentIndex<TransformComponent>())
        {
            UseGravity = true;
            UseFriction = true;
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

                if (UseFriction)
                {
                    physics.Velocity *= 0.99f;
                }
            }
        }

        private void AddForces(ref PhysicsComponent physics)
        {
            if (UseGravity && !physics.OnSurface)
            {
                physics.Acceleration += new Float4(0, 0, -10, 0);
            }
        }
    }
}
