using UnityEngine;

namespace Game
{
    public static class LayerDefine
    {
        //玩家
        public static string PlayerStr = "Player";
        public static int PlayerInt = LayerMask.NameToLayer(PlayerStr);
        public static int PlayerMask = LayerMask.GetMask(PlayerStr);

        //地形
        public static string TerrainStr = "Terrain";
        public static int TerrainInt = LayerMask.NameToLayer(TerrainStr);
        public static int TerrainMask = LayerMask.GetMask(TerrainStr);
    }
}