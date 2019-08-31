namespace Ryne.Utility
{
    interface IUpdatable
    {
        void Initialize();
        void Update(float dt);
    }

    interface IDrawable : IUpdatable
    {
        void Draw(float dt);
    }
}
