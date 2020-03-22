using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResBuff
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public float duration { get; private set; }
		public List<int> parts { get; private set; }
		public List<string> states { get; private set; }
		public int multiCount { get; private set; }
		public bool isBuff { get; private set; }
		public bool isDeBuff { get; private set; }
		public bool isHidden { get; private set; }
		public bool isPurgable { get; private set; }
		public string anim { get; private set; }
		public string effectName1 { get; private set; }
		public string effect1Mount { get; private set; }
		public string effectName2 { get; private set; }
		public string effect2Mount { get; private set; }
		public string script { get; private set; }
		public ResBuff(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			duration = float.Parse(node.Attribute("duration"));
			parts = new List<int>();
			string str_parts = node.Attribute("parts");
			if(!string.IsNullOrEmpty(str_parts))
			{
				string[] partsArr = str_parts.Split(',');
				if (partsArr != null || partsArr.Length > 0)
				{
					for (int i = 0; i < partsArr.Length; i++)
					{
						parts.Add(int.Parse(partsArr[i]));
					}
				}
			}
			states = new List<string>();
			string str_states = node.Attribute("states");
			if(!string.IsNullOrEmpty(str_states))
			{
				string[] statesArr = str_states.Split(',');
				if (statesArr != null || statesArr.Length > 0)
				{
					for (int i = 0; i < statesArr.Length; i++)
					{
						states.Add(statesArr[i]);
					}
				}
			}
			multiCount = int.Parse(node.Attribute("multiCount"));
			isBuff = bool.Parse(node.Attribute("isBuff"));
			isDeBuff = bool.Parse(node.Attribute("isDeBuff"));
			isHidden = bool.Parse(node.Attribute("isHidden"));
			isPurgable = bool.Parse(node.Attribute("isPurgable"));
			anim = node.Attribute("anim");
			effectName1 = node.Attribute("effectName1");
			effect1Mount = node.Attribute("effect1Mount");
			effectName2 = node.Attribute("effectName2");
			effect2Mount = node.Attribute("effect2Mount");
			script = node.Attribute("script");
		}
	}
}