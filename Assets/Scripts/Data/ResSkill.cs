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
		public int targetSelector { get; private set; }
		public int targetFitlerId { get; private set; }
		public string script { get; private set; }
		public float cd { get; private set; }
		public int combo { get; private set; }
		public int casterState { get; private set; }
		public int inGroundReplace { get; private set; }
		public int inAirReplace { get; private set; }
		public ResSkill(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			targetSelector = int.Parse(node.Attribute("targetSelector"));
			targetFitlerId = int.Parse(node.Attribute("targetFitlerId"));
			script = node.Attribute("script");
			cd = float.Parse(node.Attribute("cd"));
			combo = int.Parse(node.Attribute("combo"));
			casterState = int.Parse(node.Attribute("casterState"));
			inGroundReplace = int.Parse(node.Attribute("inGroundReplace"));
			inAirReplace = int.Parse(node.Attribute("inAirReplace"));
		}
	}
}