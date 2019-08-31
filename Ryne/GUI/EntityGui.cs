//using Ryne.Entities;
//using Ryne.Gui.GuiElements;
//using Ryne.Scene.Components;
//using Ryne.Utility;
//using Ryne.Utility.Math;

//namespace Ryne.Gui
//{
//    class EntityGui : WindowGui
//    {
//        private readonly Entity Entity;

//        public EntityGui(ImGuiWrapper gui, Entity entity) : base(gui, entity.Name)
//        {
//            Entity = entity;

//            AddComponents();
//            AddProperties();
//        }

//        public override void RenderContents()
//        {
//            base.RenderContents();
//        }

//        private void AddComponents()
//        {
//            if (Entity.HasComponent<TransformComponent>())
//            {
//                var transform = Global.EntityManager.TransformComponents[Entity.EntityId];

//                var element = new InputFloat3GuiElement("Position", new Float3(transform.Position));
//                element.OnValueChangedCallback += value => Global.EntityManager.TransformComponents[Entity.EntityId].Position = (Float4)value;
//                AddElement(element);

//                element = new InputFloat3GuiElement("Pivot", new Float3(transform.Pivot));
//                element.OnValueChangedCallback += value => Global.EntityManager.TransformComponents[Entity.EntityId].Pivot = (Float4)value;
//                AddElement(element);

//                element = new InputFloat3GuiElement("Rotation", transform.Rotation.ToRotator().ToFloat3());
//                element.OnValueChangedCallback += value => Global.EntityManager.TransformComponents[Entity.EntityId].Rotation = new Rotator((Float3)value).ToQuaternion();
//                AddElement(element);

//                element = new InputFloat3GuiElement("Scale", new Float3(transform.Scale));
//                element.OnValueChangedCallback += value => Global.EntityManager.TransformComponents[Entity.EntityId].Scale = (Float4)value;
//                AddElement(element);
//            }
//        }

//        /// <summary>
//        /// Add all public properties of an entity to edit
//        /// Currently pretty slow due to Reflection
//        /// </summary>
//        private void AddProperties()
//        {
//            var properties = Entity.GetType().GetProperties();
//            foreach (var property in properties)
//            {
//                if (property.GetSetMethod() != null)
//                {
//                    var name = property.Name;
//                    var value = property.GetMethod.Invoke(Entity, new object[] { });
//                    InputChangedDelegate callback = newValue => { property.SetMethod.Invoke(Entity, new[] { newValue }); };

//                    if (property.PropertyType == typeof(float))
//                    {
//                        var element = new InputFloatGuiElement(name, (float)value);
//                        element.OnValueChangedCallback += callback;
//                        AddElement(element);
//                    }

//                    if (property.PropertyType == typeof(int))
//                    {
//                        var element = new InputIntGuiElement(name, (int)value);
//                        element.OnValueChangedCallback += callback;
//                        AddElement(element);
//                    }
//                }
//            }
//        }
//    }
//}
