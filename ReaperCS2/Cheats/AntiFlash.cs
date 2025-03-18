using Swed64;

namespace ReaperCS2.Cheats
{
    class AntiFlash : CheatBase
    {
        public AntiFlash(Swed swed, IntPtr client, Renderer renderer) : base(swed, client, renderer) { }

        public void Run(Entity localPlayer)
        {
            float flashTime = swed.ReadFloat(localPlayer.pawnAddress + Offsets.m_flFlashBangTime);
            if (flashTime > 0 && renderer.antiflash)
            {
                swed.WriteFloat(localPlayer.pawnAddress + Offsets.m_flFlashBangTime, 0);
            }
        }
    }
}
