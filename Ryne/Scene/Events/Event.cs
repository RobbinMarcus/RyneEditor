namespace Ryne.Scene.Events
{
    public class Event
    {
        public int EntityId { get; }
        
        public Event(int entityId)
        {
            EntityId = entityId;
        }

        public virtual void Execute()
        {

        }
    }
}
