namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExNullObj
    {
        public static bool IsNull(this object obj)
        {
            if (obj == null) return true;
            return false;
        }

        public static bool IsNotNull(this object obj)
        {
            if (obj != null) return true;
            return false;
        }

        public static bool IsNull(this UnityEngine.Object obj)
        {
            if (obj == null) return true;
            return false;
        }

        public static bool IsNotNull(this UnityEngine.Object obj)
        {
            if (obj != null) return true;
            return false;
        }
    }
}
