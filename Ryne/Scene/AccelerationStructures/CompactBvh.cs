using System.Collections.Generic;
using Ryne.Scene.Components;
using Ryne.Scene.Systems;
using Ryne.Utility;

namespace Ryne.Scene.AccelerationStructures
{
    public struct CompactBvh
    {
        public static CompactBvh Empty => new CompactBvh();

        // Layout of these nodes can be determined from the Query algorithm below.
        public Float4[] CompactedNodes { get; set; }

        public int[] Query(TransformComponent transform, CollisionComponent collision)
        {
            List<int> result = new List<int>();

            if (CompactedNodes == null || CompactedNodes.Length < 1)
            {
                Logger.Warning("Queried empty CompactBvh");
                return result.ToArray();
            }

            AABB objectAABB = collision.EncapsulatingAABB(transform);

            int[] stack = new int[20];
            int nodeaddr = 0;
            int stackPtr = 0;

            while (stackPtr > -1)
            {
                // Check each child node for overlap.
                Float4 box1 = CompactedNodes[nodeaddr + 0]; // float4 [minx, maxx, miny, maxy]
                Float4 box2 = CompactedNodes[nodeaddr + 1]; // float4 [minz, maxz, child1, 0]
                Float4 box3 = CompactedNodes[nodeaddr + 2]; // float4 [minx, maxx, miny, maxy]
                Float4 box4 = CompactedNodes[nodeaddr + 3]; // float4 [minz, maxz, child2, 0]
                AABB aabb1 = new AABB
                {
                    Min = new Float4(box1.X, box1.Y, box1.Z, 0.0f),
                    Max = new Float4(box1.W, box2.X, box2.Y, 0.0f)
                };
                AABB aabb2 = new AABB
                {
                    Min = new Float4(box2.Z, box2.W, box3.X, 0.0f),
                    Max = new Float4(box3.Y, box3.Z, box3.W, 0.0f)
                };
                int child1 = Helpers.FloatAsInt(box4.X);
                int child2 = Helpers.FloatAsInt(box4.Y);
                bool overlap1 = CollisionSystem.AABBAABBOverlap(objectAABB, aabb1);
                bool overlap2 = CollisionSystem.AABBAABBOverlap(objectAABB, aabb2);

                // Determine next node
                if (!overlap1 && !overlap2)
                {
                    nodeaddr = stack[stackPtr--];
                }
                else
                {
                    nodeaddr = overlap1 ? child1 : child2;

                    // Traverse 1 push 2 to stack
                    if (overlap1 && overlap2)
                    {
                        stack[++stackPtr] = child2;
                    }
                }

                // On leaf break
                while (nodeaddr < 0)
                {
                    int objectId = -nodeaddr - 2; // Leaves are stored as ~id -> ~1 == -2 -> --2 -2 = 0
                    result.Add(objectId);
                    nodeaddr = stack[stackPtr--];
                }
            }

            return result.ToArray();
        }
    }
}
