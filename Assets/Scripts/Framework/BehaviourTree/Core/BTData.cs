using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 行为树节点数据
    /// </summary>
    public class BTData
    {
        public int dataIndex { get; private set; }
        public int keyIndex { get; private set; }
        public object data { get; private set; } 
        public List<BTData> children { get; private set; }

        public BTData(int dataIndex, object data)
        {
            this.dataIndex = dataIndex;
            this.data = data;
            if (this.data != null)
            {
                this.keyIndex = BTDataHandlerInitialize.GetHandlerIndex(this.data.GetType());
            }
        }

        public void AddChild(BTData btData)
        {
            if (children == null)
            {
                children = new List<BTData>();
            }
            children.Add(btData);
        }
    }
}