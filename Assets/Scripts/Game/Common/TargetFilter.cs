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

            if (cfg.includeStates.Count > 0)
            {
                FilterIncludeStates(world, host, ref results, cfg.includeStates);
            }

            if (cfg.excludeStates.Count > 0)
            {
                FilterExcludeStates(world, host, ref results, cfg.excludeStates);
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

        private static void FilterIncludeStates(World world, Entity host, ref List<Entity> results,
            List<string> includeStates)
        {
            var buffStateSystem = world.GetExistingSystem<BuffStateSystem>();
            var lst = ResetObjectPool<List<BuffStateType>>.Instance.GetObject();
            foreach (string s in includeStates)
            {
                var state = BuffStateConfig.GetStateTypeByString(s);
                lst.Add(state);
            }
            for (int i = results.Count - 1; i > -1; i--)
            {
                var entity = results[i];
                foreach (var buffStateType in lst)
                {
                    if (!buffStateSystem.HasState(entity, buffStateType))
                    {
                        results.RemoveAt(i);
                        break;
                    }
                }
            }
            ResetObjectPool<List<BuffStateType>>.Instance.SaveObject(lst);
        }

        private static void FilterExcludeStates(World world, Entity host, ref List<Entity> results,
            List<string> excludeStates)
        {
            var buffStateSystem = world.GetExistingSystem<BuffStateSystem>();
            var lst = ResetObjectPool<List<BuffStateType>>.Instance.GetObject();
            foreach (string s in excludeStates)
            {
                var state = BuffStateConfig.GetStateTypeByString(s);
                lst.Add(state);
            }
            for (int i = results.Count - 1; i > -1; i--)
            {
                var entity = results[i];
                foreach (var buffStateType in lst)
                {
                    if (buffStateSystem.HasState(entity, buffStateType))
                    {
                        results.RemoveAt(i);
                        break;
                    }
                }
            }
            ResetObjectPool<List<BuffStateType>>.Instance.SaveObject(lst);
        }
    }
}