using System;
using Pathfinding;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public struct GroundPointInfo
    {
        public Vector3 point;
        public Vector3 normal;
    }
    public class MapSystem : ComponentSystem
    {
        private static float MaxHeight = 1000;
        private MapComponent mapComponent;
        protected override void OnCreate()
        {
            mapComponent = World.AddSingletonComponent<MapComponent>();
        }

        public GroundPointInfo GetGroundInfo(Vector3 pos)
        {
            Vector3 origin = new Vector3(pos.x, MaxHeight, pos.z);
            RaycastHit hit;
            if (Physics.Raycast(origin, Vector3.down, out hit, float.MaxValue, LayerDefine.TerrainMask))
            {
                var info = new GroundPointInfo();
                info.point = hit.point;
                info.normal = hit.normal;
                return info;
            }

            return default(GroundPointInfo);
        }

        protected override void OnUpdate()
        {
        }
    }
}