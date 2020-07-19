using Ryne.Scene.Components;

namespace Ryne.Entities
{
    class Spawn : Entity
    {
        public Spawn()
        {
            Name = "Spawn";
            AddComponent<TransformComponent>();
        }
    }
}
