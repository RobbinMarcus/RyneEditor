using System;
using System.Collections.Generic;
using Ryne.Utility;
using Ryne.Utility.Math;

namespace Ryne.Scene
{
    public struct Cube
    {
        public Float4 Center;
        public Float4 Size;
        public Quaternion Rotation;

        public Cube(Float4 center, Float4 size, Quaternion rotation)
        {
            Center = center;
            Size = size;
            Rotation = rotation;
        }

        public Cube(float size)
        {
            Center = new Float4(0.0f);
            Size = new Float4(size);
            Rotation = Quaternion.Default;
        }

        public float Area()
        {
            Float4 diff = Size;
            return (diff.X * diff.Y + diff.Y * diff.Z + diff.Z * diff.X) * 2.0f;
        }

        public static Cube Empty()
        {
            return new Cube(new Float4(0.0f), new Float4(0.0f), Quaternion.Default);
        }

        public bool Contains(Float3 position)
        {
            Logger.Error("Cube contains");
            return false;
        }

        public Float3[] GetVertices()
        {
            Float3[] corners = new Float3[8];
            Float4 doubleSize = Size * 2;

            corners[0] = new Float3(Center - Size);
            corners[4] = new Float3(Center + Size);

            for (int i = 0; i < 4; i++)
            {
                corners[i] = corners[0];
                corners[i + 4] = corners[4];
            }

            corners[1].X += doubleSize.X;
            corners[2].Y += doubleSize.Y;
            corners[3].Z += doubleSize.Z;
            corners[5].X -= doubleSize.X;
            corners[6].Y -= doubleSize.Y;
            corners[7].Z -= doubleSize.Z;

            for (int i = 0; i < 8; i++)
            {
                corners[i] = new Float3(Rotation.RotateVectorAroundPivot(new Float4(corners[i]), Center));
            }

            return corners;
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
