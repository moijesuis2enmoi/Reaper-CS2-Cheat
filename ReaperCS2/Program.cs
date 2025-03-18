using ReaperCS2;
using ReaperCS2.Cheats;
using Swed64;

class Program
{
    static void Main()
    {
        Swed swed = new Swed("cs2");
        IntPtr client = swed.GetModuleBase("client.dll");
        Renderer renderer = new Renderer();
        renderer.Start().Wait();


        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();
        EntityManager entityManager = new EntityManager(renderer, swed, client, localPlayer, entities);

        Aimbot aimbot = new Aimbot(swed, client, renderer);
        RadarHack radarHack = new RadarHack(swed, client, renderer);
        AntiFlash antiFlash = new AntiFlash(swed, client, renderer);
        TriggerBot triggerBot = new TriggerBot(swed, client, renderer);
        JumpShot jumpShot = new JumpShot(swed, client, renderer);

        while (true)
        {
            entityManager.UpdateEntities();
            radarHack.Run(entities);
            aimbot.Run(localPlayer, entities);
            antiFlash.Run(localPlayer);
            triggerBot.Run(localPlayer, entities);
            jumpShot.Run(localPlayer, entities);
        }
    }
}
