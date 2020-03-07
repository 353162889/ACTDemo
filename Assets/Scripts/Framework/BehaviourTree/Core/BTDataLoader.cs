using System;
using System.Collections.Generic;
using GameData;
using NodeEditor;
using Pathfinding.Util;

namespace Framework
{
    public class BTDataLoader<T>
    {
        private NEDataLoader neDataLoader;
        private Dictionary<T, string> dicKeyToFilePath = new Dictionary<T, string>();
        private Dictionary<T, BTTreeData> dicBtTreeData = new Dictionary<T, BTTreeData>();
        public void LoadResCfgs(Dictionary<T, string> dicKeyToFilePath, Action onFinish)
        {
            Clear();
            neDataLoader = new NEDataLoader();
            var files = new List<string>();
            this.dicKeyToFilePath = dicKeyToFilePath;
            foreach (var pair in dicKeyToFilePath)
            {
                if(!files.Contains(pair.Value))files.Add(pair.Value);
            }
            neDataLoader.Load(files, BTDataHandlerInitialize.GetAllDataType(), OnProgress, onFinish);
        }

        public bool IsFinish()
        {
            return neDataLoader != null ? neDataLoader.IsFinish() : true;
        }

        private void OnProgress(string path, NEData neData)
        {
            BTTreeData btTreeData = new BTTreeData();
            int index = 0;
            var rootData = CreateBTData(ref index, neData);
            btTreeData.rootData = rootData;
            btTreeData.indexToData = new BTData[index + 1];
            InitBTTreeData(btTreeData, rootData);

            foreach (var pair in dicKeyToFilePath)
            {
                if (pair.Value == path)
                {
                    dicBtTreeData.Add(pair.Key, btTreeData);
                }
            }
        }

        private BTData CreateBTData(ref int index, NEData neData)
        {
            BTData btData = new BTData(index, neData.data);
            for (int i = 0; i < neData.lstChild.Count; i++)
            {
                index++;
                var btChildData = CreateBTData(ref index, neData.lstChild[i]);
                btData.AddChild(btChildData);
            }
            return btData;
        }

        private void InitBTTreeData(BTTreeData btTreeData, BTData btData)
        {
            if (btData != null)
            {
                btTreeData.indexToData[btData.dataIndex] = btData;
                if (btData.children != null)
                {
                    for (int i = 0; i < btData.children.Count; i++)
                    {
                        InitBTTreeData(btTreeData, btData.children[i]);
                    }
                }
            }
        }

        public BTTreeData GetData(T key)
        {
            BTTreeData value;
            dicBtTreeData.TryGetValue(key, out value);
            return value;
        }

        public void Clear()
        {
            if (neDataLoader != null)
            {
                neDataLoader.Clear();
                neDataLoader = null;
            }
            dicBtTreeData.Clear();
            dicKeyToFilePath.Clear();
        }
    }
}