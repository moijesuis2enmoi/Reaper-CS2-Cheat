using ClickableTransparentOverlay;
using ImGuiNET;
using System.Collections.Concurrent;
using System.Numerics;

namespace ReaperCS2
{
    public class Renderer : Overlay
    {
        public Vector2 screenSize = new Vector2(1920, 1080);
        public Vector2 screenPos;
        ImDrawListPtr drawList;

        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();


        public bool aimbot = true;
        public bool aimOnTeam = false;
        public bool aimOnSpotted = false;
        public float FOV = 50;
        private Vector4 circleColor = new Vector4(1, 1, 1, 1);

        public bool triggerbot = false;
        public bool jumpshot = false;
        public bool antiflash = true;
        public bool radarhack = true;
        public bool lineEsp = true;
        public bool boxEsp = true;
        public bool healthEsp = true;
        public bool nameEsp = true;
        public bool bonesEsp = true;
        private Vector4 enemyColor = new Vector4(1, 0, 0, 1);
        private Vector4 teamColor = new Vector4(0, 1, 0, 1);
        private Vector4 nameColor = new Vector4(1, 1, 1, 1);
        private Vector4 bonesColor = new Vector4(1, 1, 1, 1);


        protected override void Render()
        {
            (screenSize, Vector2 screenPos) = WindowHelper.GetCS2WindowBounds();


            ImGui.Begin("ReaperCS2");

            ImGui.Text("Aimbot");
            ImGui.Checkbox("Aimbot", ref aimbot);
            ImGui.Checkbox("Aim on team", ref aimOnTeam);
            ImGui.Checkbox("Aim on spotted", ref aimOnSpotted);
            ImGui.SliderFloat("FOV", ref FOV, 10, 300);
            if (ImGui.CollapsingHeader("Circle Color"))
            {
                ImGui.ColorEdit4("Circle Color", ref circleColor);
            }

            ImGui.Text("Triggerbot");
            ImGui.Checkbox("Triggerbot", ref triggerbot);
            ImGui.Checkbox("Jump Shot", ref jumpshot);

            ImGui.Text("AntiFlash");
            ImGui.Checkbox("AntiFlash", ref antiflash);

            ImGui.Text("Radar Hack");
            ImGui.Checkbox("RadarHack", ref radarhack);

            ImGui.Text("ESP");
            ImGui.Checkbox("Line ESP", ref lineEsp);
            ImGui.Checkbox("Box ESP", ref boxEsp);
            if (ImGui.CollapsingHeader("ESP Colors"))
            {
                ImGui.ColorEdit4("Enemy Color", ref enemyColor);
                ImGui.ColorEdit4("Team Color", ref teamColor);
            }
            ImGui.Checkbox("Bones ESP", ref bonesEsp);
            if (ImGui.CollapsingHeader("Bones Color"))
            {
                ImGui.ColorEdit4("Bones Color", ref bonesColor);
            }
            ImGui.Checkbox("Health ESP", ref healthEsp);
            ImGui.Checkbox("Name ESP", ref nameEsp);
            if (ImGui.CollapsingHeader("Name Color"))
            {
                ImGui.ColorEdit4("Name Color", ref nameColor);
            }

            ImGui.End();


            DrawOverlay(screenPos);

            drawList = ImGui.GetForegroundDrawList();
            Vector2 circlePos = new Vector2(screenPos.X + screenSize.X / 2, screenPos.Y + screenSize.Y / 2);
            drawList.AddCircle(circlePos, FOV, ImGui.ColorConvertFloat4ToU32(circleColor));


            foreach (Entity entity in entities)
            {
                if (EntityOnScreen(entity))
                {
                    if (boxEsp) Drawbox(entity);
                    if (lineEsp) DrawLine(entity);
                    if (healthEsp) DrawHealthBar(entity);
                    if (nameEsp) DrawName(entity, 15);
                    if (bonesEsp) DrawBones(entity);
                }
            }
        }

        private void DrawName(Entity entity, int yOffset)
        {
            float entityHeight = Math.Abs(entity.position2D.Y - entity.viewPosition2D.Y);

            Vector2 textSize = ImGui.CalcTextSize(entity.name);

            Vector2 textLocation = new Vector2(
                entity.viewPosition2D.X - textSize.X / 2,
                entity.viewPosition2D.Y - yOffset
            );

            drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.name}");
        }

        private void DrawBones(Entity entity)
        {
            uint uintColor = ImGui.ColorConvertFloat4ToU32(bonesColor);

            float currentBoneThickness = 4 / entity.distance;

            // draw lines between bones
            drawList.AddLine(entity.bones2d[1], entity.bones2d[2], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[3], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[6], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[3], entity.bones2d[4], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[6], entity.bones2d[7], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[4], entity.bones2d[5], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[7], entity.bones2d[8], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[1], entity.bones2d[0], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[9], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[0], entity.bones2d[11], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[9], entity.bones2d[10], uintColor, currentBoneThickness);
            drawList.AddLine(entity.bones2d[11], entity.bones2d[12], uintColor, currentBoneThickness);

            drawList.AddCircle(entity.bones2d[2], 3 + currentBoneThickness, uintColor); // circle on head
        }

        private void DrawHealthBar(Entity entity)
        {
            float entityHeight = Math.Abs(entity.position2D.Y - entity.viewPosition2D.Y);
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.viewPosition2D.X + entityHeight / 3;
            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);

            float barHeight = entityHeight * (entity.health / 100f);

            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);

            Vector4 barColor;
            if (entity.health > 50)
                barColor = new Vector4(0.2f, 0.8f, 0.2f, 1f);
            else if (entity.health > 30)
                barColor = new Vector4(1f, 0.65f, 0f, 1f);
            else
                barColor = new Vector4(0.86f, 0.08f, 0.24f, 1f);

            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
        }


        private void Drawbox(Entity entity)
        {
            float height = Math.Abs(entity.position2D.Y - entity.viewPosition2D.Y);

            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - height / 3, entity.viewPosition2D.Y);
            Vector2 rectBottom = new Vector2(entity.position2D.X + height / 3, entity.position2D.Y);

            Vector4 boxColor = entity.team == localPlayer.team ? teamColor : enemyColor;

            drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
        }



        private void DrawLine(Entity entity)
        {
            Vector4 lineColor = entity.team == localPlayer.team ? teamColor : enemyColor;

            Vector2 start = new Vector2(screenPos.X + screenSize.X / 2, screenPos.Y + screenSize.Y);
            Vector2 end = new Vector2(screenPos.X + entity.position2D.X, screenPos.Y + entity.position2D.Y);

            drawList.AddLine(start, end, ImGui.ColorConvertFloat4ToU32(lineColor));
        }


        bool EntityOnScreen(Entity entity)
        {
            return entity.position2D.X + screenPos.X >= screenPos.X &&
                   entity.position2D.X + screenPos.X <= screenPos.X + screenSize.X &&
                   entity.position2D.Y + screenPos.Y >= screenPos.Y &&
                   entity.position2D.Y + screenPos.Y <= screenPos.Y + screenSize.Y;
        }


        public void UpdateLocalPlayer(Entity newLocalPlayer)
        {
            lock (entityLock)
            {
                this.localPlayer = newLocalPlayer;
            }
        }

        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            lock (entityLock)
            {
                this.entities = new ConcurrentQueue<Entity>(newEntities);
            }
        }

        void DrawOverlay(Vector2 screenPos)
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(screenPos);

            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
            );

            ImGui.End();
        }

    }
}
