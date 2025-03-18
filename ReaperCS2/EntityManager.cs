using Swed64;
using System.Numerics;

namespace ReaperCS2
{
    class EntityManager
    {
        private Swed swed;
        private Renderer renderer;
        private IntPtr client;
        private Entity localPlayer;
        private List<Entity> entities;
        private Vector2 screenSize;
        private Vector2 screenPos;

        public EntityManager(Renderer renderer, Swed swed, IntPtr client, Entity localPlayer, List<Entity> entities)
        {
            this.renderer = renderer;
            this.screenSize = renderer.screenSize;
            this.screenPos = renderer.screenPos;
            this.swed = swed;
            this.client = client;
            this.localPlayer = localPlayer;
            this.entities = entities;
        }

        public void UpdateEntities()
        {
            entities.Clear();
            IntPtr entityList = swed.ReadPointer(client + Offsets.dwEntityList);

            if (entityList == IntPtr.Zero) return;
            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

            if (listEntry == IntPtr.Zero) return;

            localPlayer.pawnAddress = swed.ReadPointer(client + Offsets.dwLocalPlayerPawn);
            localPlayer.team = swed.ReadInt(localPlayer.pawnAddress + Offsets.m_iTeamNum);
            localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress + Offsets.m_vOldOrigin);
            localPlayer.view = swed.ReadVec(localPlayer.pawnAddress + Offsets.m_vecViewOffset);

            for (int i = 0; i < 64; i++)
            {
                IntPtr currentController = swed.ReadPointer(listEntry + i * 0x78);
                if (currentController == IntPtr.Zero) continue;

                IntPtr pawnHandle = swed.ReadPointer(currentController + Offsets.m_hPlayerPawn);
                if (pawnHandle == IntPtr.Zero) continue;

                int listIndex = (int)((pawnHandle & 0x7FFF) >> 9);
                int listOffset = 0x8 * listIndex + 0x10;
                if (listOffset < 0 || listOffset > 0x1000) continue;

                IntPtr listEntry2 = swed.ReadPointer(entityList, listOffset);
                if (listEntry2 == IntPtr.Zero) continue;

                IntPtr currentPawn = swed.ReadPointer(listEntry2 + 0x78 * (pawnHandle & 0x1FF));
                if (currentPawn == localPlayer.pawnAddress || currentPawn == IntPtr.Zero) continue;

                IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
                IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);

                int health = swed.ReadInt(currentPawn + Offsets.m_iHealth);
                int team = swed.ReadInt(currentPawn + Offsets.m_iTeamNum);
                uint lifeState = swed.ReadUInt(currentPawn + Offsets.m_lifeState);
                bool spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);

                if (lifeState != 256) continue;

                ViewMatrix viewMatrix = ReadMatrix(client + Offsets.dwViewMatrix);

                Entity entity = new Entity();
                entity.pawnAddress = currentPawn;
                entity.controllerAddress = currentController;
                entity.health = health;
                entity.lifeState = lifeState;
                entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
                entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
                entity.head = swed.ReadVec(boneMatrix, 6 * 32);
                entity.team = team;
                entity.name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16).Split("\0")[0];
                entity.spotted = spotted;
                entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
                entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, (int)screenSize.X, (int)screenSize.Y);
                entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), (int)screenSize.X, (int)screenSize.Y);
                entity.bones = Calculate.ReadBones(boneMatrix, swed);
                entity.bones2d = Calculate.ReadBones2d(entity.bones, viewMatrix, screenSize);


                entity.head2d = Calculate.WorldToScreen(viewMatrix, entity.head, (int)screenSize.X, (int)screenSize.Y);
                entity.pixelDistance = Vector2.Distance(entity.head2d, new Vector2(screenSize.X / 2, screenSize.Y / 2));
                entities.Add(entity);
            }
            renderer.UpdateLocalPlayer(localPlayer);
            renderer.UpdateEntities(entities);
        }

        public ViewMatrix ReadMatrix(IntPtr matrixAddress)
        {
            var viewMatrix = new ViewMatrix();
            var matrix = swed.ReadMatrix(matrixAddress);

            viewMatrix.m11 = matrix[0];
            viewMatrix.m12 = matrix[1];
            viewMatrix.m13 = matrix[2];
            viewMatrix.m14 = matrix[3];
            viewMatrix.m21 = matrix[4];
            viewMatrix.m22 = matrix[5];
            viewMatrix.m23 = matrix[6];
            viewMatrix.m24 = matrix[7];
            viewMatrix.m31 = matrix[8];
            viewMatrix.m32 = matrix[9];
            viewMatrix.m33 = matrix[10];
            viewMatrix.m34 = matrix[11];
            viewMatrix.m41 = matrix[12];
            viewMatrix.m42 = matrix[13];
            viewMatrix.m43 = matrix[14];
            viewMatrix.m44 = matrix[15];

            return viewMatrix;
        }
    }

}