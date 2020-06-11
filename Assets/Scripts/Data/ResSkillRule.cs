using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResSkillRule
	{
		[ResCfgKey]
		public int id { get; private set; }
		public List<int> skillId { get; private set; }
		public int condition { get; private set; }
		public float value { get; private set; }
		public ResSkillRule(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			skillId = new List<int>();
			string str_skillId = node.Attribute("skillId");
			if(!string.IsNullOrEmpty(str_skillId))
			{
				string[] skillIdArr = str_skillId.Split(',');
				if (skillIdArr != null || skillIdArr.Length > 0)
				{
					for (int i = 0; i < skillIdArr.Length; i++)
					{
						skillId.Add(int.Parse(skillIdArr[i]));
					}
				}
			}
			condition = int.Parse(node.Attribute("condition"));
			value = float.Parse(node.Attribute("value"));
		}
	}
}