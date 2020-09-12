using System;
using System.Collections.Generic;
using Framework;
using UnityEngine.UIElements;

public class UtilityCompositeTwoPointEvaluatorDataItem : UtilityNodeContainerItem
{
    public UtilityCompositeTwoPointEvaluatorDataItem(object data) : base(data)
    {
    }

    public override object Serializable()
    {
        CompositeTwoPointEvaluatorData compositeData = this.data as CompositeTwoPointEvaluatorData;
        compositeData.mLstEvaluatorDatas = new List<IEvaluatorData>();
        for (int i = 0; i < this.foldout.contentContainer.childCount; i++)
        {
            UtilityNodeItem item = this.foldout.contentContainer[i] as UtilityNodeItem;
            compositeData.mLstEvaluatorDatas.Add((IEvaluatorData)item.Serializable());
        }
        return base.Serializable();
    }

    public override void Deserializable()
    {
        CompositeTwoPointEvaluatorData compositeData = this.data as CompositeTwoPointEvaluatorData;
        this.foldout.contentContainer.Clear();
        for (int i = 0; i < compositeData.mLstEvaluatorDatas.Count; i++)
        {
            var data = compositeData.mLstEvaluatorDatas[i];
            this.AddChildItem(new UtilityNodeItem(data));
        }
        base.Deserializable();
    }

    protected override void OnAddAction(DropdownMenu menu)
    {
        var lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IEvaluatorData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            if (type != typeof(CompositeTwoPointEvaluatorData))
            {
                menu.AppendAction(string.Format("IEvaluatorData/{0}", lst[i].Name), action =>
                {
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    this.AddChildItem(item);
                });
            }

        }
    }
}
