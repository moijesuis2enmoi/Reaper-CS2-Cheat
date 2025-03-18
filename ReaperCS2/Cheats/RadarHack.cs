using Swed64;

namespace ReaperCS2.Cheats
{
    class RadarHack : CheatBase
    {
        public RadarHack(Swed swed, IntPtr client, Renderer renderer) : base(swed, client, renderer) { }

        public void Run(List<Entity> entities)
        {
            if (renderer.radarhack)
            {
                foreach (Entity entity in entities)
                {
                    swed.WriteBool(entity.pawnAddress, Offsets.m_entitySpottedState + Offsets.m_bSpotted, true);
                }
            }
        }
    }
}
