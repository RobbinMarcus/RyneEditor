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

            PlayerController = new Controller();

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

            PlayerCamera.SetPosition(Transform.Position);

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
                var rotation = PlayerCamera.GetRotation();
                rotation += new Float3(PlayerController.RotationDelta) * dt;
                float rotationY = RyneMath.Clamp(rotation.Y, -89.0f, 89.0f);
                PlayerCamera.SetRotation(new Float3(rotation.X, rotationY, 0));

                // We only want the player entity to rotate horizontally
                Transform.Rotation = new Rotator(new Float3(rotation.X, 0, 0)).ToQuaternion();
            }

            PlayerCamera.SetPosition(Transform.GetLocation());
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
