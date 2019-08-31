using System.Collections.Generic;
using System.Linq;
using Ryne.Entities;
using Ryne.GameStates;
using Ryne.Scene.AccelerationStructures;
using Ryne.Scene.Components;
using Ryne.Scene.Systems;
using Ryne.Utility;
using Ryne.Utility.Collections;

namespace Ryne.Scene
{
    // @optimization: add entities post frame using ToAdd list
    public class EntityManager : IUpdatable
    {
        public EntityStorage Data { get; private set; }

        public List<BaseSystem> Systems { get; }

        // Bounding volume hierarchy for fast spatial queries
        public CompactBvh CollisionBvh { get; private set; }

        public DynamicArray<Entity> Entities => Data.Entities;
        public DynamicArray<int> ComponentMasks => Data.ComponentMasks;


        // Components
        public DynamicArray<TransformComponent> TransformComponents => Data.TransformComponents;
        public DynamicArray<PhysicsComponent> PhysicsComponents => Data.PhysicsComponents;
        public DynamicArray<CollisionComponent> CollisionComponents => Data.CollisionComponents;
        public DynamicArray<MeshComponent> MeshComponents => Data.MeshComponents;


        // Entities added this frame that need initialization
        private readonly List<int> AddedEntities;

        public EntityManager()
        {
            Systems = new List<BaseSystem>();
            Data = new EntityStorage();
            Data.Initialize();

            CollisionBvh = new CompactBvh();

            AddedEntities = new List<int>();
        }


        public void Initialize()
        {
            AddSystem(new PhysicsSystem(this));
            AddSystem(new CollisionSystem(this));
            AddSystem(new EventSystem(this));

            foreach (var system in Systems)
            {
                system.Initialize();
            }
        }

        public void Update(float dt)
        {
            foreach (var system in Systems)
            {
                system.Update(dt);
            }

            // TODO: if changed
            UpdateCollisionBvh();

            foreach (var entity in Entities)
            {
                if (!entity.Initialized || !entity.CanUpdate)
                {
                    continue;
                }

                entity.Update(dt);
            }

            // TODO: any events fired from entities that aren't immediately executed will wait till next frame
        }

        public void PostFrame()
        {
            foreach (var entityId in AddedEntities)
            {
                var entity = Entities[entityId];

                entity.SetFlag(EntityFlag.Registered, true);
                entity.Initialize();

                Global.StateManager.RegisterEntity(entity);
            }

            AddedEntities.Clear();
        }

        public void UpdateCollisionBvh()
        {
            List<RyneAABB> bounds = new List<RyneAABB>();
            List<int> entityIds = new List<int>();

            foreach (var entity in Entities.Where(x => x.HasComponent<CollisionComponent>()))
            {
                if (!entity.Initialized)
                {
                    continue;
                }

                var entityBounds = entity.Collision.EncapsulatingAABB(entity.Transform);

                bounds.Add(new RyneAABB(entityBounds.Min, entityBounds.Max));
                entityIds.Add(entity.EntityId);
            }
            //Parallel.For(0, Global.EntityManager.Entities.Length, i =>
            //{
            //    AABB aabb = new AABB(new Float4(0.0f), new Float4(1.0f));
            //    bounds[i] = aabb.Transform(Global.EntityManager.TransformComponents[i]);
            //});

            if (entityIds.Count < 1)
            {
                return;
            }

            RyneBoundingVolumeHierarchy bvh = new RyneBoundingVolumeHierarchy();
            bvh.Create(bounds, entityIds);

            CollisionBvh = new CompactBvh { CompactedNodes = bvh.Nodes.ToArray() };
        }

        // TODO: need an initialization stage
        public void AddSystem(BaseSystem system)
        {
            Systems.Add(system);
        }

        /// <summary>
        /// Adds entity to the manager
        /// </summary>
        /// <returns>EntityId</returns>
        public int AddEntity(Entity entity, bool needsInitialize = true)
        {
            int entityId;
            if (Data.Reusable.Length > 0)
            {
                entityId = Data.Reusable[Data.Reusable.Length - 1];
                Entities[entityId] = entity;
                ComponentMasks[entityId] = entity.ComponentMask;

                Data.Reusable.RemoveLast();
            }
            else
            {
                Entities.Add(entity);
                ComponentMasks.Add(entity.ComponentMask);

                entityId = Entities.Length - 1;

                // Make sure we store components up to entityId
                ResizeComponents(entityId + 1);
            }

            Entities[entityId].EntityId = entityId;

            if (needsInitialize)
            {
                AddedEntities.Add(entityId);
            }

            // Register with matching systems
            foreach (var system in Systems)
            {
                var mask = system.ComponentMask & entity.ComponentMask;
                if (mask == system.ComponentMask)
                {
                    system.RegisterEntity(entity);
                }
            }

            return entityId;
        }


        public void DeleteEntity(ref Entity entity)
        {
            if (!entity.Initialized)
            {
                Logger.Error($"Can't delete entity {entity.Name}: Entity not initialized");
                return;
            }

            if (entity.EntityId < 0)
            {
                Logger.Error($"Can't delete entity {entity.Name}: entityId < 0");
                return;
            }

            if (entity.Destroyed)
            {
                Logger.Warning($"Calling DeleteEntity on Destroyed entity: {entity.Name}");
                return;
            }

            if (entity.Registered)
            {
                Global.StateManager.UnregisterEntity(entity);
                entity.SetFlag(EntityFlag.Registered, false);
            }

            entity.Destroy();
            Data.DeleteEntity(entity.EntityId);
        }

        // Clears all entities, components and acceleration structures
        public void Clear()
        {
            // TODO: clear loaded models backend?
            ClearEntities();
            ClearComponents();
            ClearAccelerationStructures();
        }

        public void ClearEntities()
        {
            Entities.Clear();
            Data.Reusable.Clear();
        }

        public void ClearComponents()
        {
            Data.ClearComponents();
        }

        public void ClearAccelerationStructures()
        {
            CollisionBvh = CompactBvh.Empty;
        }

        // Resizes all component arrays to contain at least maxSize amount of objects
        public void ResizeComponents(int maxSize)
        {
            Data.SetComponentArraySize(maxSize);
        }

        // TODO: create changelist to update GPU scene data
        public void RestoreEntities(EntityStorage data)
        {
            Data = new EntityStorage(data);
        }

        public IEnumerable<Entity> GetEntitiesWithMask(int mask)
        {
            foreach (var entity in Entities.Where(x => (x.ComponentMask & mask) == mask))
            {
                yield return entity;
            }
        }

        public T GetSystemOfType<T>() where T : BaseSystem
        {
            foreach (var system in Systems)
            {
                if (system.GetType() == typeof(T))
                {
                    return (T)system;
                }
            }

            return null;
        }
    }
}
