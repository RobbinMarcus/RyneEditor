using Ryne.Scene.Systems;

namespace Ryne.Entities
{
    public delegate void OnCollisionDelegate(int entityId, CollisionData data);

    public class EventBindings
    {
        public OnCollisionDelegate OnCollision;
    }
}
