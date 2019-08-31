using Ryne.Entities;
using Ryne.Scene.Events;
using Ryne.Utility.Collections;

namespace Ryne.Scene.Systems
{
    class EventSystem : BaseSystem
    {
        private readonly DynamicArray<Event> StoredEvents;

        public EventSystem(EntityManager manager) : base(manager, 0)
        {
            StoredEvents = new DynamicArray<Event>();
        }

        public override void RegisterEntity(Entity entity)
        {
        }

        public override void Initialize()
        {
        }

        public override void Update(float dt)
        {
            foreach (var @event in StoredEvents)
            {
                ExecuteEvent(@event);
            }
            StoredEvents.Clear();
        }

        public void ExecuteEvent(Event e)
        {
            e.Execute();
        }

        public void PushEvent(Event e)
        {
            StoredEvents.Add(e);
        }
    }
}
