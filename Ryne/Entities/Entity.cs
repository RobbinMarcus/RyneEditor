using System;
using System.Linq;
using System.Text;
using Ryne.Gui;
using Ryne.Scene.Components;
using Ryne.Utility;
using Ryne.Utility.Math;
using MessagePack;

namespace Ryne.Entities
{
    public enum EntityFlag
    {
        Registered,             // Entity registered in EntityManager
        RegisteredBackend,      // Entity registered in backend renderer
        Initialized,            // Entity initialized
        CanUpdate,              // Does this entity need an update call
        Destroyed,              // Entity is no longer in use

        EditorEntityChanged,    // If any of the properties of this entity have been changed for serialization
        EditorNotEditable       // Can this entity not be edited in editor
    }

    public class Entity : IUpdatable, IEquatable<Entity>
    {
        public string Name { get; set; }

        // Unique identifier for this entity to access its components
        [IgnoreMember]
        public int EntityId { get; set; }

        // Identifier for the rendering backend
        [IgnoreMember]
        public int RenderId { get; set; }

        // Flags for every component this entity contains
        public int ComponentMask { get; private set; }

        // Flags for the state of this entity
        [IgnoreMember]
        public int EntityFlags { get; private set; }

        // Events that can be bound
        [IgnoreMember]
        public EventBindings Events { get; protected set; }

        public Entity()
        {
            Name = GetType().Name;
            EntityId = -1;
            RenderId = -1;
            ComponentMask = 0;
            EntityFlags = 0;

            // By default all entities update, change to false?
            SetCanUpdate(true);

            Events = null;

            // TODO: if editor?
            SetChangedInEditor(true);
        }

        public virtual void Initialize()
        {
            EntityFlags = Bitmask.SetBitTo(EntityFlags, (int)EntityFlag.Initialized, 1);
        }

        public virtual void Update(float dt)
        {
        }

        public virtual void Destroy()
        {
            // Components
            if (HasComponent<TransformComponent>()) Transform.SetDefaults();
            if (HasComponent<CollisionComponent>()) Collision.SetDefaults();
            if (HasComponent<MeshComponent>()) Mesh.Destroy();

            SetIsDestroyed(true);
        }

        // Called after deserialization 
        public virtual void PostDeserialize()
        {
            // Call PostDeserialize on all components
            PostDeserializeComponents();

            // Any entity loaded in will not have changes compared to the file, exceptions will need to manually reflag it
            SetChangedInEditor(false);
            SetIsDestroyed(false);
        }

        // TODO: Code generation?
        public virtual void RenderGui(ImGuiWrapper gui)
        {
            // Components
            if (HasComponent<TransformComponent>()) Transform.RenderGui(gui, this);
            if (HasComponent<PhysicsComponent>()) Physics.RenderGui(gui, this);
            if (HasComponent<CollisionComponent>()) Collision.RenderGui(gui, this);
            if (HasComponent<MeshComponent>()) Mesh.RenderGui(gui, this);
        }

        /// <summary>
        /// Make sure to call this for every component type this entity supports
        /// </summary>
        public void AddComponent<T>() where T : struct
        {
            ComponentMask |= Components.GetComponentIndex<T>();
        }

        public bool HasComponent<T>() where T : struct
        {
            return (ComponentMask & Components.GetComponentIndex<T>()) > 0;
        }


        // All components
        // TODO: Code generation?

        [IgnoreMember]
        public ref TransformComponent Transform => ref Global.EntityManager.TransformComponents[EntityId];

        [IgnoreMember]
        public ref PhysicsComponent Physics => ref Global.EntityManager.PhysicsComponents[EntityId];

        [IgnoreMember]
        public ref CollisionComponent Collision => ref Global.EntityManager.CollisionComponents[EntityId];

        [IgnoreMember]
        public ref MeshComponent Mesh => ref Global.EntityManager.MeshComponents[EntityId];

        // Flags
        // TODO: Code generation?

        public bool ContainsFlag(EntityFlag flag)
        {
            return Bitmask.CheckBit(EntityFlags, (int)flag);
        }

        public void SetFlag(EntityFlag flag, bool value)
        {
            EntityFlags = Bitmask.SetBitTo(EntityFlags, (int)flag, value ? 1 : 0);
        }

        [IgnoreMember]
        public bool Registered => ContainsFlag(EntityFlag.Registered);

        [IgnoreMember]
        public bool RegisteredBackend => ContainsFlag(EntityFlag.RegisteredBackend);

        [IgnoreMember]
        public bool Initialized => ContainsFlag(EntityFlag.Initialized);

        [IgnoreMember]
        public bool CanUpdate => ContainsFlag(EntityFlag.CanUpdate);

        [IgnoreMember]
        public bool Destroyed => ContainsFlag(EntityFlag.Destroyed);

        [IgnoreMember]
        public bool ChangedInEditor => ContainsFlag(EntityFlag.EditorEntityChanged);

        public void SetCanUpdate(bool value) => SetFlag(EntityFlag.CanUpdate, value);

        public void SetIsDestroyed(bool value) => SetFlag(EntityFlag.Destroyed, value);

        public void SetChangedInEditor(bool value) => SetFlag(EntityFlag.EditorEntityChanged, value);


        // IEquatable

        public static bool operator ==(Entity obj1, Entity obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            return obj1?.EntityId == obj2?.EntityId;
        }

        public static bool operator !=(Entity obj1, Entity obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EntityId == other.EntityId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = EntityId.GetHashCode();
            return hashCode;
        }


        // Serialize using GetType()
        public string Serialize(bool writeType = true)
        {
            StringBuilder sb = new StringBuilder();
            var bytes = MessagePackSerializer.NonGeneric.Serialize(GetType(), this);
            if (writeType)
            {
                sb.AppendLine(GetType().ToString());
            }
            sb.AppendLine(MessagePackSerializer.ToJson(bytes));
            sb.Append(SerializeComponents());
            return sb.ToString();
        }

        // Creates and adds entity to EntityManager from input text
        public static T Deserialize<T>(string input) where T : Entity
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var entityLine = lines[0];

            var entity = MessagePackSerializer.Deserialize<T>(MessagePackSerializer.FromJson(entityLine));
            Global.EntityManager.AddEntity(entity);

            lines.RemoveAt(0);
            SharedDeserializeLogic(entity, lines.ToArray());
            return entity;
        }

        // Creates and adds entity to EntityManager from input type and text
        public static Entity Deserialize(Type type, string input)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var entityLine = lines[0];

            Entity entity = (Entity)MessagePackSerializer.NonGeneric.Deserialize(type, MessagePackSerializer.FromJson(entityLine));
            Global.EntityManager.AddEntity(entity);

            lines.RemoveAt(0);
            SharedDeserializeLogic(entity, lines.ToArray());
            return entity;
        }

        public static Entity Deserialize(string input)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Type type;
            try
            {
                var typeLine = lines[0];
                type = Type.GetType(typeLine);
            }
            catch (Exception)
            {
                Logger.Error("Could not determine type from string:\n" + lines[0]);
                return null;
            }

            var entityLine = lines[1];
            Entity entity = (Entity)MessagePackSerializer.NonGeneric.Deserialize(type, MessagePackSerializer.FromJson(entityLine));
            Global.EntityManager.AddEntity(entity);

            lines.RemoveAt(0);
            lines.RemoveAt(0);
            SharedDeserializeLogic(entity, lines.ToArray());
            return entity;
        }

        private static void SharedDeserializeLogic(Entity entity, string[] componentLines)
        {
            entity.DeserializeComponents(componentLines);
            entity.PostDeserialize();
        }

        // Creates a string from all components set on this entity
        private string SerializeComponents()
        {
            StringBuilder sb = new StringBuilder();
            if (HasComponent<TransformComponent>()) sb.AppendLine(MessagePackSerializer.ToJson(Transform));
            if (HasComponent<PhysicsComponent>()) sb.AppendLine(MessagePackSerializer.ToJson(Physics));
            if (HasComponent<CollisionComponent>()) sb.AppendLine(MessagePackSerializer.ToJson(Collision));
            if (HasComponent<MeshComponent>()) sb.AppendLine(MessagePackSerializer.ToJson(Mesh));
            return sb.ToString();
        }

        // Restores components in this entity from string, make sure entity is added to EntityManager
        private void DeserializeComponents(string[] componentLines)
        {
            int lineCount = 0;

            if (HasComponent<TransformComponent>()) Transform = MessagePackSerializer.Deserialize<TransformComponent>(MessagePackSerializer.FromJson(componentLines[lineCount++]));
            if (HasComponent<PhysicsComponent>()) Physics = MessagePackSerializer.Deserialize<PhysicsComponent>(MessagePackSerializer.FromJson(componentLines[lineCount++]));
            if (HasComponent<CollisionComponent>()) Collision = MessagePackSerializer.Deserialize<CollisionComponent>(MessagePackSerializer.FromJson(componentLines[lineCount++]));
            if (HasComponent<MeshComponent>()) Mesh = MessagePackSerializer.Deserialize<MeshComponent>(MessagePackSerializer.FromJson(componentLines[lineCount++]));

            // Try set component to default after deserializing to initialize properties that have to be initialized
            Mesh.TrySetDefaults();
        }

        private void PostDeserializeComponents()
        {
            if (HasComponent<TransformComponent>()) Transform.PostDeserialize();
            if (HasComponent<PhysicsComponent>()) Physics.PostDeserialize();
            if (HasComponent<CollisionComponent>()) Collision.PostDeserialize();
            if (HasComponent<MeshComponent>()) Mesh.PostDeserialize();
        }
    }
}
