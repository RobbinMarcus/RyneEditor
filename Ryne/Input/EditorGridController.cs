using System.Collections.Generic;
using System.Linq;
using Ryne.GameStates;
using Ryne.Scene.Components;
using Ryne.Utility;
using Ryne.Utility.Actions;
using Ryne.Utility.Collections;
using Ryne.Utility.Math;

namespace Ryne.Input
{
    class EditorGridController : Controller
    {
        public float GridAlignment { get; set; }
        public float AngleAlignment { get; set; }
        public float ScaleAlignment { get; set; }

        public bool MovedMouseSinceStart { get; private set; }

        // If true, rotates selected objects around shared pivot
        public bool SharedPivotRotationMode { get; set; }

        private readonly SceneEditor Editor;

        private readonly List<TransformComponent> StartingTransforms;
        private Float4 EditMovementDelta;
        private Float4 EditRotationDelta;
        private Float4 EditScaleDelta;


        private Float4 SharedPivot;

        public EditorGridController(SceneEditor editor)
        {
            Editor = editor;
            StartingTransforms = new List<TransformComponent>();
            MovedMouseSinceStart = false;

            SharedPivotRotationMode = true;

            UpdateAlignmentValues();
        }

        public override void UpdateEntities(float dt)
        {
            if (!base.Changed())
            {
                return;
            }

            MovedMouseSinceStart = true;
            EditMovementDelta += MovementRawDelta * dt;
            EditRotationDelta += RotationDelta * dt;
            EditScaleDelta += ScaleDelta * dt;

            for (var i = 0; i < ControlledEntities.Count; i++)
            {
                var entity = ControlledEntities[i];
                var startingTransform = StartingTransforms[i];
                if (RotationDelta.Length() > Constants.Epsilon)
                {
                    var rotation = startingTransform.Rotation.ToRotator().ToFloat4() + EditRotationDelta;//transform.Rotation.ToRotator().ToFloat4();
                    if (AngleAlignment > 0)
                    {
                        rotation -= RyneMath.Mod(rotation, new Float4(AngleAlignment));
                    }
                    entity.Transform.Rotation = new Rotator(rotation).ToQuaternion();

                    if (SharedPivotRotationMode)
                    {
                        var vector = startingTransform.GetLocation() - SharedPivot;
                        // Unrotate with starting rotation
                        vector = startingTransform.Rotation.Inverse().RotateVector(vector);
                        // Rotate to new rotation
                        vector = entity.Transform.Rotation.RotateVector(vector);
                        entity.Transform.SetLocation(SharedPivot + vector);
                    }
                }

                if (MovementRawDelta.Length() > Constants.Epsilon)
                {
                    var position = startingTransform.Position + EditMovementDelta;
                    if (GridAlignment > 0)
                    {
                        position -= RyneMath.Mod(position, new Float4(GridAlignment));
                    }
                    entity.Transform.Position = position;
                }

                if (ScaleDelta.Length() > Constants.Epsilon)
                {
                    var scale = startingTransform.Scale + EditScaleDelta;
                    if (ScaleAlignment > 0)
                    {
                        scale -= RyneMath.Mod(scale, new Float4(ScaleAlignment));
                    }
                    entity.Transform.Scale = scale;
                }
            }

            OnEntitiesChanged?.Invoke();
        }

        public override bool Changed()
        {
            return base.Changed() && ChangedSinceStart();
        }

        public bool ChangedSinceStart()
        {
            if (ControlledEntities.Empty())
            {
                return false;
            }

            // Take the first entity as starting point
            var transform = ControlledEntity.Transform;
            return (transform.Position - StartingTransforms[0].Position).Length() > Constants.Epsilon
                   || (transform.Rotation.ToFloat3() - StartingTransforms[0].Rotation.ToFloat3()).Length() > Constants.Epsilon
                   || (transform.Scale - StartingTransforms[0].Scale).Length() > Constants.Epsilon;
        }

        public void StartEdit()
        {
            StartingTransforms.Clear();
            foreach (var entity in ControlledEntities)
            {
                StartingTransforms.Add(entity.Transform);
            }

            EditMovementDelta = new Float4(0);
            EditRotationDelta = new Float4(0);
            EditScaleDelta = new Float4(0);

            MovedMouseSinceStart = false;

            SharedPivot = new Float4(0.0f);
            if (SharedPivotRotationMode)
            {
                for (var i = 0; i < ControlledEntities.Count; i++)
                {
                    var startingTransform = StartingTransforms[i];
                    SharedPivot += startingTransform.GetLocation();
                }

                SharedPivot /= ControlledEntities.Count;
            }
        }

        public void EndEdit()
        {
            if (ControlledEntities.Count == 0 || StartingTransforms.Count == 0)
            {
                return;
            }

            if (ControlledEntities.Count > 1)
            {
                Editor.AddUndoAction(new UndoGroupTransformAction(ControlledEntities.Select(x => x.EntityId).ToArray(), StartingTransforms.ToArray()));
            }
            else
            {
                Editor.AddUndoAction(new UndoTransformAction(ControlledEntities[0].EntityId, StartingTransforms[0]));
            }
        }

        public void UpdateAlignmentValues()
        {
            GridAlignment = Global.Config.GridAlignment;
            AngleAlignment = Global.Config.AngleAlignment;
            ScaleAlignment = Global.Config.ScaleAlignment;
        }
    }
}
