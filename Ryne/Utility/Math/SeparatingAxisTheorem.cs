using Ryne.Scene;
using Ryne.Scene.Systems;

namespace Ryne.Utility.Math
{
    public struct SatBodyData
    {
        public Float3 Center;
        public Float3[] Vertices;
        public Float3[] Normals;

        public void CreateFrom(AABB aabb)
        {
            Center = aabb.Center();
            Vertices = aabb.GetVertices();
            Normals = new[]
            {
                new Float3(1, 0, 0),
                new Float3(0, 1, 0),
                new Float3(0, 0, 1),
                new Float3(-1, 0, 0),
                new Float3(0, -1, 0),
                new Float3(0, 0, -1),
            };
        }

        public void CreateFrom(Cube cube)
        {
            Center = new Float3(cube.Center);
            Vertices = cube.GetVertices();
            Normals = new[]
            {
                new Float3(1, 0, 0),
                new Float3(0, 1, 0),
                new Float3(0, 0, 1),
                new Float3(-1, 0, 0),
                new Float3(0, -1, 0),
                new Float3(0, 0, -1),
            };
            for (int i = 0; i < Normals.Length; i++)
            {
                Normals[i] = new Float3(cube.Rotation.RotateVector(new Float4(Normals[i])));
            }
        }
    }

    // Struct to hold SeparatingAxisTheorem projection data
    struct SatProjectionData
    {
        float Min, Max;

        public bool Overlaps(SatProjectionData projectionData)
        {
            return projectionData.Max > Min && projectionData.Min < Max;
        }

        public float GetOverlap(SatProjectionData projectionData)
        {
            return System.Math.Min(Max - projectionData.Min, projectionData.Max - Min);
        }

        public float GetMaxOverlap(SatProjectionData projectionData)
        {
            return System.Math.Max(Max - projectionData.Min, projectionData.Max - Min);
        }

        // Projects the satBody on a normal
        public void ProjectVertices(SatBodyData satBody, Float3 normal)
        {
            Min = normal.Dot(satBody.Vertices[0]);
            Max = Min;

            for (int i = 1; i < satBody.Vertices.Length; i++)
            {
                float p = normal.Dot(satBody.Vertices[i]);
                if (p < Min)
                {
                    Min = p;
                }
                else if (p > Max)
                {
                    Max = p;
                }
            }
        }
    }

    public struct SeparatingAxisTheorem
    {
        private SatProjectionData Projection1;
        private SatProjectionData Projection2;

        public CollisionData Test(SatBodyData body1, SatBodyData body2)
        {
            bool bodiesSet = body1.Vertices.Length > 0 && body1.Normals.Length > 0 && body2.Vertices.Length > 0 && body2.Normals.Length > 0;
            if (!bodiesSet)
            {
                Logger.Error("Bodies for SAT test not set correctly");
                return new CollisionData();
            }

            Projection1 = new SatProjectionData();
            Projection2 = new SatProjectionData();

            CollisionData c1 = ProjectNormals(body1, body2);
            if (!c1.Intersecting)
            {
                return c1;
            }

            CollisionData c2 = ProjectNormals(body2, body1);
            if (!c2.Intersecting)
            {
                return c2;
            }

            CollisionData result = c1;
            if (c1.Depth < c2.Depth)
            {
                result = c2;
                result.Normal = -1.0f * result.Normal;
            }
            return result;
        }

        // Special SAT case for spheres
        public CollisionData Test(SatBodyData body, Sphere s)
        {
            bool bodiesSet = body.Vertices.Length > 0 && body.Normals.Length > 0;
            if (!bodiesSet)
            {
                Logger.Error("Bodies for SAT test not set correctly");
                return new CollisionData();
            }

            Logger.Error("SAT Test with sphere not implemented");
            return new CollisionData();
        }

        private CollisionData ProjectNormals(SatBodyData body1, SatBodyData body2)
        {
            Float3 dir = (body1.Center - body2.Center).Normalize();
            CollisionData result = new CollisionData
            {
                Intersecting = true,
                Depth = float.MaxValue
            };

            foreach (var normal in body1.Normals)
            {
                Projection1.ProjectVertices(body1, normal);
                Projection2.ProjectVertices(body2, normal);

                if (!Projection1.Overlaps(Projection2))
                {
                    result.Intersecting = false;
                    return result;
                }

                float depth = normal.Dot(dir) < 0.0f ? Projection1.GetMaxOverlap(Projection2) : Projection1.GetOverlap(Projection2);
                if (depth < result.Depth)
                {
                    result.Depth = depth;
                    result.Normal = normal;
                }
            }

            return result;
        }
    }
}
