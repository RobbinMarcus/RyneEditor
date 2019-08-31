namespace Ryne.Scene.Components
{
    sealed class Components
    {
        public static int GetComponentIndex<T>()
        {
            if (typeof(T) == typeof(TransformComponent)) return 1 << 0;
            if (typeof(T) == typeof(PhysicsComponent)) return 1 << 1;
            if (typeof(T) == typeof(CollisionComponent)) return 1 << 2;
            if (typeof(T) == typeof(MeshComponent)) return 1 << 3;

            return 0;
        }
    }
}
