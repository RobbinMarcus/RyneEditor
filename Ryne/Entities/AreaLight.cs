using Ryne.Gui;
using Ryne.Scene;
using Ryne.Scene.Components;
using Ryne.Utility;
using Ryne.Utility.Math;
using MessagePack;

namespace Ryne.Entities
{
    public sealed class AreaLight : Entity
    {
        public Float3 Color { get; set; }
        public float Intensity { get; set; }

        [IgnoreMember]
        public bool Changed { get; set; }

        public AreaLight()
        {
            Color = new Float3(1.0f);
            Intensity = 100.0f;
            Changed = false;

            AddComponent<TransformComponent>();
            AddComponent<CollisionComponent>();
        }

        public override void Initialize()
        {
            base.Initialize();
            Collision.Shape = RyneCollisionShape.CollisionShapeSphere;
            Collision.SetSphere(new Sphere(new Float3(0.0f), 1.0f));
            Collision.Static = true;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override unsafe void RenderGui(ImGuiWrapper gui)
        {
            base.RenderGui(gui);

            if (gui is SceneEditorGui sceneGui)
            {
                var color = Color;
                if (gui.InputFloat3("Color", &color))
                {
                    Color = RyneMath.Max(color, new Float3(0.01f));
                    Changed = true;
                }

                var intensity = Intensity;
                if (gui.InputFloat("Intensity", ref intensity))
                {
                    Intensity = System.Math.Max(intensity, 0.01f);
                    Changed = true;
                }
            }

            if (Changed)
            {
                Global.StateManager.GetCurrentState().SceneRenderData.UpdateLight(ToRenderLight(), RenderId);
                Changed = false;
            }
        }

        public RyneAreaLight ToRenderLight()
        {
            RyneAreaLight result = new RyneAreaLight
            {
                AreaShape = Collision.ToRenderComponent(),
                Intensity = Intensity,
                Color = Color
            };
            return result;
        }
    }
}
