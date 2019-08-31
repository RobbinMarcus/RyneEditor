using Ryne.Gui;
using Ryne.Input;
using Ryne.Scene;
using Ryne.Scene.Components;
using Ryne.Scene.Systems;
using Ryne.Utility;
using Ryne.Utility.Math;

namespace Ryne.Entities
{
    class Player : Entity
    {
        public RyneCamera PlayerCamera { get; private set; }
        public Controller PlayerController { get; private set; }

        public Player()
        {
            Name = "Player";

            var renderSize = Global.Application.GetRenderSize();
            PlayerCamera = new RyneCamera(renderSize.X, renderSize.Y);

            // Components
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AddComponent<CollisionComponent>();

            // This entity is only an in-game object, nothing to change in editor
            SetFlag(EntityFlag.EditorNotEditable, true);
        }

        public bool RenderEntity(ImGuiWrapper gui, string label)
        {
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            Collision.SetCube(new Cube(new Float4(0.0f), new Float4(0.25f, 0.25f, 0.5f, 0.0f), Quaternion.Default));

            PlayerCamera.Position = Transform.Position;

            PlayerController = new Controller();
            PlayerController.SetMouseCentered(true);
            PlayerController.ControlEntity(this);
            PlayerController.SetInputMapping(new PlayerInputMapping());

            // Events
            Events = new EventBindings
            {
                OnCollision = OnCollision
            };
        }

        public override void Update(float dt)
        {
            PlayerController.Update(dt);

            if (PlayerController.Changed())
            {
                PlayerCamera.Rotation += new Float3(PlayerController.RotationDelta) * dt;
                float rotationY = RyneMath.Clamp(PlayerCamera.Rotation.Y, -89.0f, 89.0f);
                PlayerCamera.Rotation = new Float3(PlayerCamera.Rotation.X, rotationY, 0);

                // We only want the player entity to rotate horizontally
                Transform.Rotation = new Rotator(new Float3(PlayerCamera.Rotation.X, 0, 0)).ToQuaternion();
            }

            PlayerCamera.Position = Transform.GetLocation();
            PlayerCamera.Update(dt);
        }

        public override void Destroy()
        {
            PlayerController.Destroy();
        }

        void OnCollision(int entityId, CollisionData data)
        {
            Logger.Log("Player oncollision");
        }
    }
}
