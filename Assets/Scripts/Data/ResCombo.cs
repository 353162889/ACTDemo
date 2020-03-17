using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResCombo
	{
		[ResCfgKey]
		public int id { get; private set; }
		public List<int> lstSkillId { get; private set; }
		public ResCombo(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			lstSkillId = new List<int>();
			string str_lstSkillId = node.Attribute("lstSkillId");
			if(!string.IsNullOrEmpty(str_lstSkillId))
			{
				string[] lstSkillIdArr = str_lstSkillId.Split(',');
				if (lstSkillIdArr != null || lstSkillIdArr.Length > 0)
				{
					for (int i = 0; i < lstSkillIdArr.Length; i++)
					{
						lstSkillId.Add(int.Parse(lstSkillIdArr[i]));
					}
				}
			}
		}
	}
}