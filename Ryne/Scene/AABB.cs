using System;
using System.Collections.Generic;
using Ryne.Scene.Components;
using Ryne.Utility.Math;

namespace Ryne.Scene
{
    public struct AABB
    {
        public Float4 Min;
        public Float4 Max;

        public AABB(Float4 min, Float4 max)
        {
            Min = min;
            Max = max;
        }

        public AABB(float size)
        {
            Min = new Float4(0.0f);
            Max = new Float4(size);
        }

        public Float3 Center()
        {
            return new Float3((Min + Max) * 0.5f);
        }

        public float Area()
        {
            Float4 diff = Max - Min;
            return (diff.X * diff.Y + diff.Y * diff.Z + diff.Z * diff.X) * 2.0f;
        }

        public Float4 Size()
        {
            return Max - Min;
        }

        public static AABB Empty()
        {
            return new AABB(new Float4(100000.0f), new Float4(-100000.0f));
        }

        public void Expand(AABB bounds)
        {
            Max = RyneMath.Max(Max, bounds.Max);
            Min = RyneMath.Min(Min, bounds.Min);
        }

        public void Expand(Float4 position)
        {
            Max = RyneMath.Max(Max, position);
            Min = RyneMath.Min(Min, position);
        }

        public bool Contains(Float3 position)
        {
            return position.X >= Min.X && position.X <= Max.X
               && position.Y >= Min.Y && position.Y <= Max.Y
               && position.Z >= Min.Z && position.Z <= Max.Z;
        }

        public Float3[] GetVertices()
        {
            Float3[] corners = new Float3[8];

            corners[0] = new Float3(Min);
            corners[4] = new Float3(Max);

            for (int i = 0; i < 4; i++)
            {
                corners[i] = new Float3(Min);
                corners[i + 4] = new Float3(Max);
            }

            Float4 diff = Max - Min;
            corners[1].X += diff.X;
            corners[2].Y += diff.Y;
            corners[3].Z += diff.Z;
            corners[5].X -= diff.X;
            corners[6].Y -= diff.Y;
            corners[7].Z -= diff.Z;

            return corners;
        }

        public AABB Transform(TransformComponent transform)
        {
            AABB result = Empty();

            foreach (var corner in GetVertices())
            {
                result.Expand(transform.ToWorldSpace(new Float4(corner)));
            }

            return result;
        }

        public void DebugVisualization(RyneImGuiWrapper gui, Float3 color)
        {
            var vertices = GetVertices();
            List<Tuple<Float3, Float3>> lines = new List<Tuple<Float3, Float3>>
            {
                new Tuple<Float3, Float3>(vertices[0], vertices[1]), 
                new Tuple<Float3, Float3>(vertices[1], vertices[7]),
                new Tuple<Float3, Float3>(vertices[7], vertices[2]),
                new Tuple<Float3, Float3>(vertices[2], vertices[0]),
                new Tuple<Float3, Float3>(vertices[3], vertices[6]),
                new Tuple<Float3, Float3>(vertices[6], vertices[4]),
                new Tuple<Float3, Float3>(vertices[4], vertices[5]),
                new Tuple<Float3, Float3>(vertices[5], vertices[3]),
                new Tuple<Float3, Float3>(vertices[0], vertices[3]),
                new Tuple<Float3, Float3>(vertices[1], vertices[6]),
                new Tuple<Float3, Float3>(vertices[2], vertices[5]),
                new Tuple<Float3, Float3>(vertices[7], vertices[4])
            };
            foreach (var line in lines)
            {
                gui.DebugAddLine(line.Item1, line.Item2, color);
            }
        }
    }
}
