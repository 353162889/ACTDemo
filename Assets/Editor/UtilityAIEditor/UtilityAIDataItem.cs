using System;
using System.CodeDom;
using System.Collections.Generic;
using Framework;
using UnityEngine.UIElements;

public class UtilityAIDataItem : UtilityNodeContainerItem
{
    private VisualElement selectorContainer;

    public UtilityAIDataItem(object data) : base(data)
    {
        selectorContainer = this.Q<VisualElement>("selectorContainer");
    }

    public override object Serializable()
    {
        UtilityAIData utilityAIData = this.data as UtilityAIData;
        UtilityNodeItem nodeItem = this.selectorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        utilityAIData.mSelectorData = (ISelectorData)nodeItem.Serializable();
        utilityAIData.mLstGoalDatas = new List<IUtilityGoalData>();
        for (int i = 0; i < this.foldout.contentContainer.childCount; i++)
        {
            UtilityNodeItem item = this.foldout.contentContainer[i] as UtilityNodeItem;
            utilityAIData.mLstGoalDatas.Add((IUtilityGoalData)item.Serializable());
        }
        return base.Serializable();
    }


    public override void Deserializable()
    {
        UtilityAIData utilityAIData = this.data as UtilityAIData;
        selectorContainer.Clear();
        selectorContainer.Add(new UtilityNodeItem(utilityAIData.selectorData));

        this.foldout.contentContainer.Clear();
        for (int i = 0; i < utilityAIData.lstGoalDatas.Count; i++)
        {
            var goalData = utilityAIData.lstGoalDatas[i];
            if (goalData is ICompositeUtilityGoalData)
            {
                this.AddChildItem(new UtilityCompositeGoalDataItem(goalData));
            }
            else
            {
                this.AddChildItem(new UtilityGoalDataItem(goalData));
            }
        }
        base.Deserializable();
    }

    protected override void OnAddAction(DropdownMenu menu)
    {
        var lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ISelectorData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("ISelectorData/{0}", lst[i].Name), action =>
            {
                selectorContainer.Clear();
                var item = new UtilityNodeItem(Activator.CreateInstance(type));
                selectorContainer.Add(item);
            });            
        }

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ICompositeUtilityGoalData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("ICompositeUtilityGoalData/{0}", lst[i].Name), action =>
            {
                var item = new UtilityCompositeGoalDataItem(Activator.CreateInstance(type));
                this.AddChildItem(item);
            });
        }
        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IUtilityGoalData));
        var compositeUtilityGoalDataType = typeof(ICompositeUtilityGoalData);
        for (int i = lst.Count - 1; i > -1; i--)
        {
            if (compositeUtilityGoalDataType.IsAssignableFrom(lst[i]))
            {
                lst.RemoveAt(i);
            }
        }
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("IUtilityGoalData/{0}", lst[i].Name), action =>
            {
                var item = new UtilityGoalDataItem(Activator.CreateInstance(type));
                this.AddChildItem(item);
            });
        }
    }
}
