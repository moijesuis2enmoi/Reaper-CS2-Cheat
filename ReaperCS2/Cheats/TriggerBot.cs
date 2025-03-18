using Swed64;
using System.Runtime.InteropServices;

namespace ReaperCS2.Cheats
{
    class TriggerBot : CheatBase
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private const int HOTKEY = 0x06;

        public TriggerBot(Swed swed, IntPtr client, Renderer renderer) : base(swed, client, renderer) { }

        public void Run(Entity localPlayer, List<Entity> entities)
        {
            int entIndex = swed.ReadInt(localPlayer.pawnAddress + Offsets.m_iIDEntIndex);
            if (entIndex > 0)
            {
                var entityEntry = swed.ReadLong(client, 0x8 * (entIndex >> 9) + 0x10);
                var entity = swed.ReadLong(entityEntry + 120 * (entIndex & 0x1FF));
                var entityTeam = swed.ReadInt(entity, Offsets.m_iTeamNum);

                if (renderer.triggerbot && GetAsyncKeyState(HOTKEY) < 0)
                {
                    if (entityTeam != localPlayer.team || (entityTeam == localPlayer.team && renderer.aimOnTeam))
                    {
                        swed.WriteInt(client, Offsets.dwAttack, 65537);
                        Thread.Sleep(1);
                        swed.WriteInt(client, Offsets.dwAttack, 256);
                    }
                }
            }
        }
    }
}
