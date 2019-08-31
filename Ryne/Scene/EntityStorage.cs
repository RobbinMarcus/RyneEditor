using Ryne.Entities;
using Ryne.Scene.Components;
using Ryne.Utility.Collections;

namespace Ryne.Scene
{
    public class EntityStorage
    {
        // All entities
        // TODO: Copies are pass by reference, need to have a (fast) pass by value option
        public DynamicArray<Entity> Entities { get; private set; }
        // Copy of component mask from entity to enable iteration over components from systems
        public DynamicArray<int> ComponentMasks { get; private set; }

        // Components
        public DynamicArray<TransformComponent> TransformComponents { get; private set; }
        public DynamicArray<PhysicsComponent> PhysicsComponents { get; private set; }
        public DynamicArray<CollisionComponent> CollisionComponents { get; private set; }
        public DynamicArray<MeshComponent> MeshComponents { get; private set; }

        public DynamicArray<int> Reusable { get; private set; }

        public EntityStorage()
        {
            Entities = new DynamicArray<Entity>();
            ComponentMasks = new DynamicArray<int>();

            TransformComponents = new DynamicArray<TransformComponent>();
            PhysicsComponents = new DynamicArray<PhysicsComponent>();
            CollisionComponents = new DynamicArray<CollisionComponent>();
            MeshComponents = new DynamicArray<MeshComponent>();

            Reusable = new DynamicArray<int>();
        }

        public EntityStorage(EntityStorage other)
        {
            Initialize();

            other.Entities.CopyTo(Entities);
            other.ComponentMasks.CopyTo(ComponentMasks);

            other.TransformComponents.CopyTo(TransformComponents);
            other.PhysicsComponents.CopyTo(PhysicsComponents);
            other.CollisionComponents.CopyTo(CollisionComponents);
            other.MeshComponents.CopyTo(MeshComponents);

            other.Reusable.CopyTo(Reusable);
        }

        public void Initialize()
        {
            Entities = new DynamicArray<Entity>();
            ComponentMasks = new DynamicArray<int>();

            TransformComponents = new DynamicArray<TransformComponent>();
            PhysicsComponents = new DynamicArray<PhysicsComponent>();
            CollisionComponents = new DynamicArray<CollisionComponent>();
            MeshComponents = new DynamicArray<MeshComponent>();

            Reusable = new DynamicArray<int>();
        }

        /// <summary>
        /// Remove all component data
        /// </summary>
        public void ClearComponents()
        {
            TransformComponents.Clear();
            PhysicsComponents.Clear();
            CollisionComponents.Clear();
            MeshComponents.Clear();
        }

        public void Clear()
        {
            ClearComponents();

            Entities.Clear();
            ComponentMasks.Clear();
            Reusable.Clear();
        }

        /// <summary>
        /// Resizes all component arrays to contain at least maxSize amount of objects
        /// </summary>
        public void SetComponentArraySize(int maxSize)
        {
            int oldSize = TransformComponents.Length;
            if (maxSize > oldSize)
            {
                TransformComponents.SetSize(maxSize);
                PhysicsComponents.SetSize(maxSize);
                CollisionComponents.SetSize(maxSize);
                MeshComponents.SetSize(maxSize);

                for (int i = oldSize; i < maxSize; i++)
                {
                    // Initialize defaults
                    TransformComponents[i].SetDefaults();
                    CollisionComponents[i].SetDefaults();
                    MeshComponents[i].SetDefaults();
                }
            }
        }

        public void DeleteEntity(int entityId)
        {
            Reusable.Add(entityId);
        }
    }
}
