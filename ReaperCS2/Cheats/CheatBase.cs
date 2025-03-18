using Swed64;

namespace ReaperCS2.Cheats
{
    class CheatBase
    {
        protected Swed swed;
        protected IntPtr client;
        protected Renderer renderer;
        protected Entity localPlayer;
        protected List<Entity> entities;

        public CheatBase(Swed swed, IntPtr client, Renderer renderer)
        {
            this.swed = swed;
            this.client = client;
            this.renderer = renderer;
            this.localPlayer = new Entity();
            this.entities = new List<Entity>();
        }
    }
}
