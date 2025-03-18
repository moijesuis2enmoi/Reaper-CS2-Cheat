using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;


namespace ReaperCS2.Cheats
{
    class Aimbot : CheatBase
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        public Aimbot(Swed swed, IntPtr client, Renderer renderer) : base(swed, client, renderer) { }

        public void Run(Entity localPlayer, List<Entity> entities)
        {
            if (!renderer.aimOnTeam)
                entities = entities.Where(x => x.team != localPlayer.team).ToList();
            if (renderer.aimOnSpotted)
                entities = entities.Where(x => x.spotted).ToList();
            entities = entities.OrderBy(x => x.pixelDistance).ToList();

            if (entities.Count > 0 && renderer.aimbot && GetAsyncKeyState(Settings.HOTKEY) < 0)
            {
                Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);
                if (entities[0].pixelDistance < renderer.FOV)
                {
                    Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                    Vector3 newAnglesVec3 = new Vector3(newAngles.X, newAngles.Y, 0.0f);

                    swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
                }

            }
        }
    }

}
