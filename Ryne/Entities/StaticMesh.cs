using Ryne.Scene;
using Ryne.Scene.Components;
using Ryne.Utility.Math;

namespace Ryne.Entities
{
    public sealed class StaticMesh : Entity
    {
        public StaticMesh()
        {
            AddComponent<TransformComponent>();
            AddComponent<MeshComponent>();
            AddComponent<CollisionComponent>();
        }

        public override void Initialize()
        {
            base.Initialize();
            Collision.Shape = RyneCollisionShape.CollisionShapeAABB;
            Collision.SetCube(new Cube(new Float4(0.0f), new Float4(0.5f), Quaternion.Default));
            Collision.Static = true;
        }
    }
}
