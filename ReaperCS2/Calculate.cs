using Swed64;
using System.Numerics;

namespace ReaperCS2
{
    public class Calculate
    {
        public static Vector2 CalculateAngles(Vector3 from, Vector3 to)
        {
            Vector3 delta = new Vector3(to.X - from.X, to.Y - from.Y, to.Z - from.Z);

            float distance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

            float pitch = (float)(-Math.Atan2(delta.Z, distance) * 180 / Math.PI);
            float yaw = (float)(Math.Atan2(delta.Y, delta.X) * 180 / Math.PI);

            pitch = NormalizeAngle(pitch);
            yaw = NormalizeAngle(yaw);

            return new Vector2(pitch, yaw);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;

            return angle;
        }

        public static Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, int width, int height)
        {
            Vector2 screenCoordinates = new Vector2();

            float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

            if (screenW > 0.001f)
            {
                float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;
                float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

                float camX = width / 2;
                float camY = height / 2;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                screenCoordinates.X = X;
                screenCoordinates.Y = Y;
                return screenCoordinates;
            }
            else
            {
                return new Vector2(-99, -99);
            }
        }

        public static List<Vector3> ReadBones(IntPtr boneAddress, Swed swed)
        {
            byte[] boneBytes = swed.ReadBytes(boneAddress, 27 * 32 * 10);
            List<Vector3> bones = new List<Vector3>();

            foreach (var boneId in Enum.GetValues(typeof(BonesIds)))
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4);
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }

            return bones;
        }

        public static List<Vector2> ReadBones2d(List<Vector3> bones, ViewMatrix viewMatrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();
            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = WorldToScreen(viewMatrix, bone, (int)screenSize.X, (int)screenSize.Y);
                bones2d.Add(bone2d);
            }
            return bones2d;
        }
    }
}
