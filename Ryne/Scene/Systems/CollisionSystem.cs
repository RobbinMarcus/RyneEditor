using Ryne.Entities;
using Ryne.Scene.Components;
using Ryne.Scene.Events;
using Ryne.Utility;
using Ryne.Utility.Math;
using Math = System.Math;

namespace Ryne.Scene.Systems
{
    public class CollisionSystem : BaseSystem
    {
        private EventSystem Events;

        public CollisionSystem(EntityManager manager) : base(manager, Components.Components.GetComponentIndex<CollisionComponent>()
            | Components.Components.GetComponentIndex<TransformComponent>())
        {
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
            // TODO: only handle entities with id > own id?
            foreach (var entity in GetEntitiesWithMask())
            {
                if (entity.Collision.Static)
                {
                    continue;
                }

                ref var transform = ref entity.Transform;

                // If moved, collision detect/resolve
                if ((transform.Position - transform.PreviousPosition).Length() > Constants.Epsilon)
                {
                    ref var collision = ref entity.Collision;

                    // Query StaticBVH
                    var overlappingIds = Global.EntityManager.CollisionBvh.Query(transform, collision);

                    // For each id: Collision resolve
                    foreach (var id in overlappingIds)
                    {
                        if (id == entity.EntityId)
                        {
                            continue;
                        }

                        var otherEntity = Global.EntityManager.Entities[id];
                        var collisionData = CheckIntersection(entity, otherEntity);

                        if (collisionData.Intersecting)
                        {
                            //if (!entity.Collision.Static)
                            //{ 
                                // Move mesh back out of the static mesh
                                transform.Position += new Float4(collisionData.Normal * collisionData.Depth);

                                // Remove velocity in the direction of the collision normal
                                ref var physicsComponent = ref entity.Physics;
                                var counterVelocity = RyneMath.Project(physicsComponent.Velocity, new Float4(collisionData.Normal));
                                physicsComponent.Velocity -= counterVelocity;
                            //}

                            var collisionEvent = new CollisionEvent(entity.EntityId, id, collisionData);
                            Events.PushEvent(collisionEvent);
                        }
                    }
                }
            }
        }


        public static CollisionData CheckIntersection(Entity e1, Entity e2)
        {
            var mask = Components.Components.GetComponentIndex<TransformComponent>() | Components.Components.GetComponentIndex<CollisionComponent>();
            if ((e1.ComponentMask & mask) != mask || (e2.ComponentMask & mask) != mask)
            {
                Logger.Error("CheckIntersection with entities that don't have the necessary components");
                return new CollisionData();
            }

            return CheckIntersection(e1.Collision, e2.Collision, e1.Transform, e2.Transform);
        }

        // Check intersection with another collision component
        public static CollisionData CheckIntersection(CollisionComponent c1, CollisionComponent c2, TransformComponent t1, TransformComponent t2)
        {
            // To avoid duplicate code, we sort the shapes by enum order and only compare with equal/higher enums
            bool inverseLookup = c2.Shape < c1.Shape;
            var shape = inverseLookup ? c2.Shape : c1.Shape;
            var shape2 = inverseLookup ? c1.Shape : c2.Shape;

            CollisionData result = new CollisionData();
            switch (shape)
            {
                case RyneCollisionShape.CollisionShapeAABB:
                {
                    AABB aabb = inverseLookup ? c2.ExtractAABB(t2) : c1.ExtractAABB(t1);
                    switch (shape2)
                    {
                        case RyneCollisionShape.CollisionShapeAABB:
                            result = AABBAABBIntersection(aabb, c2.ExtractAABB(t2));
                            break;
                        case RyneCollisionShape.CollisionShapeCube:
                            Cube cube = inverseLookup ? c1.ExtractCube(t1) : c2.ExtractCube(t2);
                            result = AABBCubeIntersection(aabb, cube);
                            break;
                        case RyneCollisionShape.CollisionShapeSphere:
                            Sphere sphere = inverseLookup ? c1.ExtractSphere(t1) : c2.ExtractSphere(t2);
                            result = SphereAABBIntersection(sphere, aabb);
                            result.Invert();
                            break;
                    }
                    break;
                }

                case RyneCollisionShape.CollisionShapeCube:
                {
                    Cube cube = inverseLookup ? c2.ExtractCube(t2) : c1.ExtractCube(t1);
                    switch (shape2)
                    {
                        case RyneCollisionShape.CollisionShapeCube:
                            result = CubeCubeIntersection(cube, c2.ExtractCube(t2));
                            break;
                        case RyneCollisionShape.CollisionShapeSphere:
                            Sphere sphere = inverseLookup ? c1.ExtractSphere(t1) : c2.ExtractSphere(t2);
                            result = SphereCubeIntersection(sphere, cube);
                            result.Invert();
                            break;
                    }
                    break;
                }

                case RyneCollisionShape.CollisionShapeSphere:
                    switch (shape2)
                    {
                        case RyneCollisionShape.CollisionShapeSphere:
                            result = SphereSphereIntersection(c1.ExtractSphere(t1), c2.ExtractSphere(t2));
                            break;
                    }
                    break;

                default:
                    Logger.Error("CheckIntersection: Collisionshape not found");
                    result.Intersecting = false;
                    break;
            }

            if (inverseLookup)
            {
                result.Invert();
            }

            return result;
        }

        /// Intersection functions

        // Intersection between spheres in world space
        public static CollisionData SphereSphereIntersection(Sphere s1, Sphere s2)
        {
            CollisionData result;
            result.Normal = s1.Position - s2.Position;
            result.Intersecting = result.Normal.Length() < s1.Radius + s2.Radius;
            result.Normal = result.Normal.Normalize();

            var p1 = s1.Position - result.Normal * s1.Radius;
            var p2 = s2.Position + result.Normal * s2.Radius;
            var diff = p2 - p1;
            result.Depth = diff.Length();

            return result;
        }

        // Intersection between two AABB's in world space
        public static CollisionData AABBAABBIntersection(AABB a1, AABB a2)
        {
            var b1 = new SatBodyData();
            b1.CreateFrom(a1);
            var b2 = new SatBodyData();
            b2.CreateFrom(a2);

            var sat = new SeparatingAxisTheorem();
            return sat.Test(b1, b2);
        }

        public static CollisionData CubeCubeIntersection(Cube c1, Cube c2)
        {
            var b1 = new SatBodyData();
            b1.CreateFrom(c1);
            var b2 = new SatBodyData();
            b2.CreateFrom(c2);

            var sat = new SeparatingAxisTheorem();
            return sat.Test(b1, b2);
        }

        // Intersection between Sphere and AABB in world space
        public static CollisionData SphereAABBIntersection(Sphere s, AABB a)
        {
            // TODO?
            CollisionData result;
            Float3 center = s.Position;
            Float3 closestPos = new Float3(
                Math.Max(a.Min.X, Math.Min(center.X, a.Max.X)),
                Math.Max(a.Min.Y, Math.Min(center.Y, a.Max.Y)),
                Math.Max(a.Min.Z, Math.Min(center.Z, a.Max.Z)));

            Float3 diff = center - closestPos;
            if (a.Contains(center))
            {
                // TODO: depth
                result.Depth = s.Radius;
                result.Intersecting = true;
            }
            else
            {
                result.Depth = s.Radius - diff.Length();
                result.Intersecting = result.Depth > 0.0f;
            }

            result.Normal = diff.Normalize();

            return result;
        }

        public static CollisionData SphereCubeIntersection(Sphere s, Cube c)
        {
            CollisionData result;
            Float4 center = new Float4(s.Position) - c.Center;
            center = c.Rotation.RotateVector(center);
            Float3 closestPos = new Float3(
                Math.Max(-c.Size.X, Math.Min(center.X, c.Size.X)),
                Math.Max(-c.Size.Y, Math.Min(center.Y, c.Size.Y)),
                Math.Max(-c.Size.Z, Math.Min(center.Z, c.Size.Z)));

            Float3 diff = new Float3(center) - closestPos;
            result.Intersecting = false;
            result.Depth = diff.Length();
            if (result.Depth < s.Radius)
            {
                result.Intersecting = true;
            }

            result.Normal = diff.Normalize();

            return result;
        }

        public static CollisionData AABBCubeIntersection(AABB a, Cube c)
        {
            var b1 = new SatBodyData();
            b1.CreateFrom(a);
            var b2 = new SatBodyData();
            b2.CreateFrom(c);

            var sat = new SeparatingAxisTheorem();
            return sat.Test(b1, b2);
        }


        /// Overlap functions

        // Returns true if AABB's overlap
        public static bool AABBAABBOverlap(AABB a1, AABB a2)
        {
            return a1.Max.Z > a2.Min.X
                && a1.Min.Z < a2.Max.X
                && a1.Max.Y > a2.Min.Y
                && a1.Min.Y < a2.Max.Y
                && a1.Max.Z > a2.Min.Z
                && a1.Min.Z < a2.Max.Z;
        }
    }

    public struct CollisionData
    {
        public Float3 Normal;
        public float Depth;
        public bool Intersecting;

        public void Invert()
        {
            Normal = new Float3(-Normal.X, -Normal.Y, -Normal.Z);
        }
    }
}
