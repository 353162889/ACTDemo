using System.Collections.Generic;
using Framework;
using GameData;
using Unity.Entities;

namespace Game
{
    public static class TargetFilter
    {
        public static void Filter(int targetFilterId, World world,  Entity host, List<Entity> targets, ref List<Entity> results)
        {
            var cfg = ResCfgSys.Instance.GetCfg<ResTargetFilter>(targetFilterId);
            results.Clear();
            for (int i = 0; i < targets.Count; i++)
            {
                results.Add(targets[i]);
            }

            if (cfg == null) return;
            if (cfg.ownerType > 0)
            {
                FilterOwner(world, host, ref results, (FilterOwnerType)cfg.ownerType);
            }
            
        }

        private static void FilterOwner(World world, Entity host, ref List<Entity> results,
            FilterOwnerType ownerType)
        {
            if (ownerType == FilterOwnerType.Self)
            {
                for (int i = results.Count - 1; i > -1; i--)
                {
                    if (results[i] != host)
                    {
                        results.RemoveAt(i);
                    }
                }
            }
            else
            {
                for (int i = results.Count - 1; i > -1; i--)
                {
                    if (results[i] == host)
                    {
                        results.RemoveAt(i);
                    }
                }
            }
        }
    }
}