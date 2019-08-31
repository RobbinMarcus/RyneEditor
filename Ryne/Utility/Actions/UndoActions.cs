using System;
using System.Collections.Generic;
using Ryne.Entities;
using Ryne.Scene.Components;

namespace Ryne.Utility.Actions
{
    public class UndoAction : IAction
    {
        public bool Execute()
        {
            throw new ArgumentException("UndoAction should only ever be used for undoing an action that doesn't have a normal Execute");
        }
        public virtual bool Undo()
        {
            return true;
        }
    }

    public class UndoDeleteAction : UndoAction
    {
        private readonly string SerializedEntity;

        public UndoDeleteAction(string serializedEntity)
        {
            SerializedEntity = serializedEntity;
        }

        public override bool Undo()
        {
            Entity e = Entity.Deserialize(SerializedEntity);
            ref var newEntity = ref Global.EntityManager.Entities[e.EntityId];

            // Flag as changed so it will be saved
            newEntity.SetChangedInEditor(true);
            return true;
        }
    }

    public class UndoTransformAction : UndoAction
    {
        private readonly int EntityId;
        private readonly TransformComponent Transform;

        public UndoTransformAction(int entityId, TransformComponent previousTransform)
        {
            EntityId = entityId;
            Transform = previousTransform;
        }

        public override bool Undo()
        {
            ref Entity entity = ref Global.EntityManager.Entities[EntityId];
            // Set entity to previous transform
            entity.Transform = Transform;
            entity.SetChangedInEditor(true);

            if (entity.RegisteredBackend)
            { 
                Global.StateManager.GetCurrentState().SceneRenderData.UpdateTransform(entity.RenderId, entity.Transform.ToRenderComponent());
            }

            return true;
        }
    }

    public class UndoGroupTransformAction : UndoAction
    {
        private readonly List<UndoTransformAction> TransformActions;

        public UndoGroupTransformAction(int[] entityIds, TransformComponent[] previousTransforms)
        {
            TransformActions = new List<UndoTransformAction>();
            for (int i = 0; i < entityIds.Length; i++)
            {
                UndoTransformAction action = new UndoTransformAction(entityIds[i], previousTransforms[i]);
                TransformActions.Add(action);
            }
        }

        public override bool Undo()
        {
            bool success = true;
            foreach (var action in TransformActions)
            {
                success |= action.Undo();
            }

            return success;
        }
    }
}
