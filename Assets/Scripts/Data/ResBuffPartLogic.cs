using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResBuffPartLogic
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string eventTriggerType { get; private set; }
		public int eventTriggerCount { get; private set; }
		public float duration { get; private set; }
		public float interval { get; private set; }
		public List<int> buffIds { get; private set; }
		public List<string> properties { get; private set; }
		public string script { get; private set; }
		public float overrideDistance { get; private set; }
		public ResBuffPartLogic(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			eventTriggerType = node.Attribute("eventTriggerType");
			eventTriggerCount = int.Parse(node.Attribute("eventTriggerCount"));
			duration = float.Parse(node.Attribute("duration"));
			interval = float.Parse(node.Attribute("interval"));
			buffIds = new List<int>();
			string str_buffIds = node.Attribute("buffIds");
			if(!string.IsNullOrEmpty(str_buffIds))
			{
				string[] buffIdsArr = str_buffIds.Split(',');
				if (buffIdsArr != null || buffIdsArr.Length > 0)
				{
					for (int i = 0; i < buffIdsArr.Length; i++)
					{
						buffIds.Add(int.Parse(buffIdsArr[i]));
					}
				}
			}
			properties = new List<string>();
			string str_properties = node.Attribute("properties");
			if(!string.IsNullOrEmpty(str_properties))
			{
				string[] propertiesArr = str_properties.Split(',');
				if (propertiesArr != null || propertiesArr.Length > 0)
				{
					for (int i = 0; i < propertiesArr.Length; i++)
					{
						properties.Add(propertiesArr[i]);
					}
				}
			}
			script = node.Attribute("script");
			overrideDistance = float.Parse(node.Attribute("overrideDistance"));
		}
	}
}