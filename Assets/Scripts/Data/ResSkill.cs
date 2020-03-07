using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResSkill
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public string script { get; private set; }
		public ResSkill(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			script = node.Attribute("script");
		}
	}
}