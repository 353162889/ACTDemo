using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResTargetFilter
	{
		[ResCfgKey]
		public int id { get; private set; }
		public int ownerType { get; private set; }
		public List<string> includeStates { get; private set; }
		public List<string> excludeStates { get; private set; }
		public ResTargetFilter(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			ownerType = int.Parse(node.Attribute("ownerType"));
			includeStates = new List<string>();
			string str_includeStates = node.Attribute("includeStates");
			if(!string.IsNullOrEmpty(str_includeStates))
			{
				string[] includeStatesArr = str_includeStates.Split(',');
				if (includeStatesArr != null || includeStatesArr.Length > 0)
				{
					for (int i = 0; i < includeStatesArr.Length; i++)
					{
						includeStates.Add(includeStatesArr[i]);
					}
				}
			}
			excludeStates = new List<string>();
			string str_excludeStates = node.Attribute("excludeStates");
			if(!string.IsNullOrEmpty(str_excludeStates))
			{
				string[] excludeStatesArr = str_excludeStates.Split(',');
				if (excludeStatesArr != null || excludeStatesArr.Length > 0)
				{
					for (int i = 0; i < excludeStatesArr.Length; i++)
					{
						excludeStates.Add(excludeStatesArr[i]);
					}
				}
			}
		}
	}
}