using Ryne.Scene.Systems;
using Ryne.Utility;

namespace Ryne.Scene.Events
{
    public class CollisionEvent : Event
    {
        public int OtherEntityId { get; }
        CollisionData Data { get; }

        public CollisionEvent(int entityId, int otherEntityId, CollisionData data) : base(entityId)
        {
            OtherEntityId = otherEntityId;
            Data = data;
        }

        public override void Execute()
        {
            var self = Global.EntityManager.Entities[EntityId];
            if (self.Destroyed)
            {
                return;
            }

            self.Events?.OnCollision?.Invoke(OtherEntityId, Data);
        }
    }
}
