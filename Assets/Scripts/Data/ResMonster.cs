using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResMonster
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public string prefab { get; private set; }
		public List<int> skillIds { get; private set; }
		public List<int> skillRuleIds { get; private set; }
		public string aiScript { get; private set; }
		public float traceStopMoveDistance { get; private set; }
		public ResMonster(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			prefab = node.Attribute("prefab");
			skillIds = new List<int>();
			string str_skillIds = node.Attribute("skillIds");
			if(!string.IsNullOrEmpty(str_skillIds))
			{
				string[] skillIdsArr = str_skillIds.Split(',');
				if (skillIdsArr != null || skillIdsArr.Length > 0)
				{
					for (int i = 0; i < skillIdsArr.Length; i++)
					{
						skillIds.Add(int.Parse(skillIdsArr[i]));
					}
				}
			}
			skillRuleIds = new List<int>();
			string str_skillRuleIds = node.Attribute("skillRuleIds");
			if(!string.IsNullOrEmpty(str_skillRuleIds))
			{
				string[] skillRuleIdsArr = str_skillRuleIds.Split(',');
				if (skillRuleIdsArr != null || skillRuleIdsArr.Length > 0)
				{
					for (int i = 0; i < skillRuleIdsArr.Length; i++)
					{
						skillRuleIds.Add(int.Parse(skillRuleIdsArr[i]));
					}
				}
			}
			aiScript = node.Attribute("aiScript");
			traceStopMoveDistance = float.Parse(node.Attribute("traceStopMoveDistance"));
		}
	}
}