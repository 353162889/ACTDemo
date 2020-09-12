using System;
using System.Collections.Generic;
using Framework;
using UnityEngine.UIElements;

public class UtilityCompositeDecisionFactorDataItem : UtilityNodeContainerItem
{
    private VisualElement combineContainer;
    public UtilityCompositeDecisionFactorDataItem(object data) : base(data)
    {
        combineContainer = this.Q<VisualElement>("combineContainer");
    }

    public override object Serializable()
    {
        CompositeDecisionFactorData compositeDecisionFactorData = this.data as CompositeDecisionFactorData;
        UtilityNodeItem nodeItem = this.combineContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        compositeDecisionFactorData.mCombineData = (ICombineData) nodeItem.Serializable();

        compositeDecisionFactorData.mLstFactor = new List<IDecisionFactorData>();
        for (int i = 0; i < this.foldout.contentContainer.childCount; i++)
        {
            UtilityNodeItem item = this.foldout.contentContainer[i] as UtilityNodeItem;
            compositeDecisionFactorData.mLstFactor.Add((IDecisionFactorData)item.Serializable());
        }
        return base.Serializable();
    }

    public override void Deserializable()
    {
        CompositeDecisionFactorData compositeDecisionFactorData = this.data as CompositeDecisionFactorData;
        combineContainer.Clear();
        combineContainer.Add(new UtilityNodeItem(compositeDecisionFactorData.combineData));

        this.foldout.contentContainer.Clear();
        for (int i = 0; i < compositeDecisionFactorData.lstFactor.Count; i++)
        {
            var decisionFactorData = compositeDecisionFactorData.lstFactor[i];
            if (decisionFactorData is ICompositeDecisionFactorData)
            {
                this.AddChildItem(new UtilityCompositeDecisionFactorDataItem(decisionFactorData));
            }
            else if (decisionFactorData is DecisionFactorData)
            {
                this.AddChildItem(new UtilityDecisionFactorDataItem(decisionFactorData));
            }
            else
            {
                this.AddChildItem(new UtilityNodeItem(decisionFactorData));
            }
        }

        base.Deserializable();
    }

    protected override void OnAddAction(DropdownMenu menu)
    {
        var lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ICombineData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("ICombineData/{0}", lst[i].Name), action =>
            {
                combineContainer.Clear();
                var item = new UtilityNodeItem(Activator.CreateInstance(type));
                combineContainer.Add(item);
            });
        }

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ICompositeDecisionFactorData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("IDecisionFactorData/Composite/{0}", lst[i].Name), action =>
            {
                var item = new UtilityCompositeDecisionFactorDataItem(Activator.CreateInstance(type));
                this.Add(item);
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
                    var item = new UtilityDecisionFactorDataItem(Activator.CreateInstance(type));
                    this.AddChildItem(item);
                });
            }
            else
            {
                menu.AppendAction(string.Format("IDecisionFactorData/Other/{0}", lst[i].Name), action =>
                {
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    this.AddChildItem(item);
                });
            }
        }
        base.OnAddAction(menu);
    }
}
