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
		public ResTargetFilter(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			ownerType = int.Parse(node.Attribute("ownerType"));
		}
	}
}