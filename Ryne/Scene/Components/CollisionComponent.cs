using System.Runtime.InteropServices;
using Ryne.Entities;
using Ryne.Gui;
using Ryne.Scene.Systems;
using Ryne.Utility;
using Ryne.Utility.Math;
using MessagePack;

namespace Ryne.Scene.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CollisionComponent
    {
        // These are quite obscure in order to optimize them for memory access.
        // Fetch the component properties through functions below like "ExtractAABB"
        public Float4 Data1;
        public Float4 Data2;
        public RyneCollisionShape Shape;

        public bool Static;

        // TODO DEBUG
        [IgnoreMember]
        private bool DebugVisualization;

        public void SetDefaults()
        {
            Data1 = new Float4(0.0f);
            Data2 = new Float4(0.0f);
            Shape = RyneCollisionShape.CollisionShapeNone;
        }

        public AABB EncapsulatingAABB(TransformComponent transform)
        {
            AABB result = AABB.Empty();

            switch (Shape)
            {
                case RyneCollisionShape.CollisionShapeAABB:
                    result = ExtractAABB(transform);
                    break;
                case RyneCollisionShape.CollisionShapeSphere:
                    Sphere sphere = ExtractSphere(transform);
                    result.Min = new Float4(sphere.Position - new Float3(sphere.Radius), 0.0f);
                    result.Max = new Float4(sphere.Position + new Float3(sphere.Radius), 0.0f);
                    break;
                case RyneCollisionShape.CollisionShapeCube:
                    Cube cube = ExtractCube(transform);
                    foreach (var vertex in cube.GetVertices())
                    {
                        result.Min = RyneMath.Min(result.Min, new Float4(vertex));
                        result.Max = RyneMath.Max(result.Max, new Float4(vertex));
                    }
                    break;
                default:
                    Logger.Error("EncapsulatingAABB for unsupported shape");
                    break;
            }

            return result;
        }

        public bool Contains(TransformComponent transform, Float3 position)
        {
            switch (Shape)
            {
                case RyneCollisionShape.CollisionShapeAABB: return ExtractAABB(transform).Contains(position);
                case RyneCollisionShape.CollisionShapeSphere: return ExtractSphere(transform).Contains(position);
                case RyneCollisionShape.CollisionShapeCube: return ExtractCube(transform).Contains(position);
                default: Logger.Error("Contains for unsupported shape"); break;
            }

            return false;
        }

        public AABB ExtractAABB(TransformComponent transform)
        {
            CheckShape(RyneCollisionShape.CollisionShapeAABB);

            AABB result = new AABB
            {
                Min = transform.GetLocation() - Data1,
                Max = transform.GetLocation() + Data2
            };
            return result;
        }

        public Sphere ExtractSphere(TransformComponent transform)
        {
            CheckShape(RyneCollisionShape.CollisionShapeSphere);

            Sphere result = new Sphere(new Float3(transform.GetLocation()), Data1.W);
            return result;
        }

        public Cube ExtractCube(TransformComponent transform)
        {
            CheckShape(RyneCollisionShape.CollisionShapeCube);

            Cube result = new Cube
            {
                Center = transform.ToWorldSpace(new Float4(0.5f)),//)transform.Position + transform.Scale * 0.5f,
                Size = transform.Scale * 0.5f,
                Rotation = transform.Rotation
            };
            return result;
        }

        public void SetAABB(AABB aabb)
        {
            Shape = RyneCollisionShape.CollisionShapeAABB;
            Data1 = aabb.Min;
            Data2 = aabb.Max;
        }

        public void SetSphere(Sphere sphere)
        {
            Shape = RyneCollisionShape.CollisionShapeSphere;
            Data1 = sphere.Data;
        }

        public void SetCube(Cube cube)
        {
            Shape = RyneCollisionShape.CollisionShapeCube;
            Data1 = cube.Center;
            Data2 = cube.Size;
        }

        public void RenderGui(ImGuiWrapper gui, Entity owner)
        {
            if (gui.CollapsingHeader("Collision component", true))
            {
                if (gui is SceneEditorGui sceneGui)
                {
                    if (gui.InputCheckBox("Render debug visualization", ref DebugVisualization))
                    {
                        sceneGui.RenderDebugVisualization = true;
                        //DebugVisualization = !DebugVisualization;
                    }

                    if (sceneGui.RenderDebugVisualization && DebugVisualization)
                    {
                        Global.EntityManager.UpdateCollisionBvh();

                        switch (Shape)
                        {
                            case RyneCollisionShape.CollisionShapeAABB:
                                var aabb = ExtractAABB(owner.Transform);
                                aabb.DebugVisualization(gui, new Float3(1, 0, 0));
                                break;
                            case RyneCollisionShape.CollisionShapeCube:
                                var cube = ExtractCube(owner.Transform);
                                cube.DebugVisualization(gui, new Float3(1, 0, 0));
                                break;
                        }

                        foreach (var id in Global.EntityManager.CollisionBvh.Query(owner.Transform, this))
                        {
                            if (id == owner.EntityId)
                                continue;

                            var collisionData = CollisionSystem.CheckIntersection(owner, Global.EntityManager.Entities[id]);
                            if (collisionData.Intersecting)
                            {
                                var start = new Float3(owner.Transform.GetLocation());
                                var end = start + collisionData.Normal * collisionData.Depth;
                                gui.DebugAddLine(start, end, new Float3(0, 1, 0));
                            }
                        }
                    }
                }
            }
        }

        public void PostDeserialize()
        {

        }

        public RyneCollisionComponent ToRenderComponent()
        {
            RyneCollisionComponent result = new RyneCollisionComponent
            {
                Data1 = Data1,
                Data2 = Data2,
                Shape = Shape
            };
            return result;
        }

        private void CheckShape(RyneCollisionShape expected)
        {
            if (Shape != expected)
            {
                Logger.Error($"Expected shape {expected} but collision shape is {Shape}");
            }
        }
    }
}
