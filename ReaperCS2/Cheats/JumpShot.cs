using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ReaperCS2.Cheats
{
    class JumpShot : CheatBase
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private const int HOTKEY = 0x06;
        private Vector3 velocity;

        public JumpShot(Swed swed, IntPtr client, Renderer renderer) : base(swed, client, renderer) { }

        public void Run(Entity localPlayer, List<Entity> entities)
        {
            int fFlag = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_fFlags);
            Vector3 velocity = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecAbsVelocity);

            if (renderer.jumpshot && fFlag == 65664 && GetAsyncKeyState(HOTKEY) < 0)
            {
                if (velocity.Z <= 8f && velocity.Z >= -8f)
                {
                    swed.WriteInt(client, Offsets.dwAttack, 65537);
                    Thread.Sleep(1);
                    swed.WriteInt(client, Offsets.dwAttack, 256);
                }
            }
        }
    }
}
