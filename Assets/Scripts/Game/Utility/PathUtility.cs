namespace Game
{
    public static class PathUtility
    {
        public static string GetBasePrefabPath(string name)
        {
            if (!name.EndsWith(".prefab"))
            {
                return name + ".prefab";
            }
            return name;
        }

        public static string BattleEffectDir = "Prefab/Effect/";
        public static string GetBattleEffectPath(string name)
        {
            return GetBasePrefabPath(BattleEffectDir + name);
        }
    }
}
