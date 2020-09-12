using System;
using System.CodeDom;
using System.Collections.Generic;
using Framework;
using UnityEngine.UIElements;

public class UtilityCompositeGoalDataItem : UtilityNodeContainerItem
{
    private VisualElement decisionFactorContainer;
    private VisualElement selectorContainer;

    public UtilityCompositeGoalDataItem(object data) : base(data)
    {
        decisionFactorContainer = this.Q<VisualElement>("decisionFactorContainer");
        selectorContainer = this.Q<VisualElement>("selectorContainer");
    }

    public override object Serializable()
    {
        CompositeUtilityGoalData compositeUtilityGoalData = this.data as CompositeUtilityGoalData;
        UtilityNodeItem nodeItem = this.decisionFactorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        compositeUtilityGoalData.mDecisionFactorData = (IDecisionFactorData)nodeItem.Serializable();

        nodeItem = this.selectorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        compositeUtilityGoalData.mSelectorData = (ISelectorData) nodeItem.Serializable();

        compositeUtilityGoalData.mLstGoalDatas = new List<IUtilityGoalData>();
        for (int i = 0; i < this.foldout.contentContainer.childCount; i++)
        {
            UtilityNodeItem item = this.foldout.contentContainer[i] as UtilityNodeItem;
            compositeUtilityGoalData.mLstGoalDatas.Add((IUtilityGoalData)item.Serializable());
        }
        return base.Serializable();
    }

    public override void Deserializable()
    {
        CompositeUtilityGoalData compositeUtilityGoalData = this.data as CompositeUtilityGoalData;
        decisionFactorContainer.Clear();
        if (compositeUtilityGoalData.decisionFactorData is ICompositeDecisionFactorData)
        {
            decisionFactorContainer.Add(new UtilityCompositeDecisionFactorDataItem(compositeUtilityGoalData.decisionFactorData));
        }
        else if(compositeUtilityGoalData.decisionFactorData is DecisionFactorData)
        {
            decisionFactorContainer.Add(new UtilityDecisionFactorDataItem(compositeUtilityGoalData.decisionFactorData));
        }
        else
        {
            decisionFactorContainer.Add(new UtilityNodeItem(compositeUtilityGoalData.decisionFactorData));
        }


        selectorContainer.Clear();
        selectorContainer.Add(new UtilityNodeItem(compositeUtilityGoalData.selectorData));

        this.foldout.contentContainer.Clear();
        if (compositeUtilityGoalData.lstGoalDatas != null)
        {
            for (int i = 0; i < compositeUtilityGoalData.lstGoalDatas.Count; i++)
            {
                var goalData = compositeUtilityGoalData.lstGoalDatas[i];
                if (goalData is ICompositeUtilityGoalData)
                {
                    this.AddChildItem(new UtilityCompositeGoalDataItem(goalData));
                }
                else
                {
                    this.AddChildItem(new UtilityGoalDataItem(goalData));
                }
            }
        }
        base.Deserializable();
    }

    protected override void OnAddAction(DropdownMenu menu)
    {
        var lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ICompositeDecisionFactorData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("IDecisionFactorData/Composite/{0}", lst[i].Name), action =>
            {
                decisionFactorContainer.Clear();
                var item = new UtilityCompositeDecisionFactorDataItem(Activator.CreateInstance(type));
                decisionFactorContainer.Add(item);
            });
        }
        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IDecisionFactorData));
        var compositeDecisionFactorType = typeof(ICompositeDecisionFactorData);
        for (int i = lst.Count - 1; i > -1; i--)
        {
            if (compositeDecisionFactorType.IsAssignableFrom(lst[i]))
            {
                lst.RemoveAt(i);
            }
        }
        var decisionFactorDataType = typeof(DecisionFactorData);
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            if (decisionFactorDataType.IsAssignableFrom(type) || type == decisionFactorDataType)
            {
                menu.AppendAction(string.Format("IDecisionFactorData/DecisionFactorData/{0}", lst[i].Name), action =>
                {
                    decisionFactorContainer.Clear();
                    var item = new UtilityDecisionFactorDataItem(Activator.CreateInstance(type));
                    decisionFactorContainer.Add(item);
                });
            }
            else
            {
                menu.AppendAction(string.Format("IDecisionFactorData/Other/{0}", lst[i].Name), action =>
                {
                    decisionFactorContainer.Clear();
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    decisionFactorContainer.Add(item);
                });
            }
        }

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ISelectorData));
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

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IUtilityGoalData));
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
