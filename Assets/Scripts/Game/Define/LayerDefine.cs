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

        //受击框
        public static string BehitColliderStr = "BehitCollider";
        public static int BehitColliderInt = LayerMask.NameToLayer(BehitColliderStr);
        public static int BehitColliderMask = LayerMask.GetMask(BehitColliderStr);

        //攻击框
        public static string AttackColliderStr = "AttackCollider";
        public static int AttackColliderInt = LayerMask.NameToLayer(AttackColliderStr);
        public static int AttackColliderMask = LayerMask.GetMask(AttackColliderStr);

        //目标检测
        public static string TargetTriggerColliderStr = "TargetTriggerCollider";
        public static int TargetTriggerColliderInt = LayerMask.NameToLayer(TargetTriggerColliderStr);
        public static int TargetTriggerColliderMask = LayerMask.GetMask(TargetTriggerColliderStr);
    }
}