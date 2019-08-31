using Ryne.Scene.Components;

namespace Ryne.Scene
{
    public class Sphere
    {
        public Float4 Data;

        public Sphere(Float3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Float3 Position
        {
            get => new Float3(Data);
            set => Data = new Float4(value, Data.W);
        }

        public float Radius
        {
            get => Data.W;
            set => Data.W = value;
        }

        public bool Contains(Float3 position)
        {
            return (Position - position).Length() < Radius;
        }

        public Sphere Transform(TransformComponent transform)
        {
            Sphere result = new Sphere(new Float3(Data + transform.Position), Radius);
            return result;
        }
    }
}
