using System;
using Framework;
using UnityEngine.UIElements;

public class UtilityDecisionFactorDataItem : UtilityNodeItem
{
    private VisualElement evaluatorContainer;
    public UtilityDecisionFactorDataItem(object data) : base(data)
    {
        evaluatorContainer = this.Q<VisualElement>("evaluatorContainer");
    }

    public override object Serializable()
    {
        DecisionFactorData decisionFactorData = this.data as DecisionFactorData;
        UtilityNodeItem nodeItem = this.evaluatorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        decisionFactorData.mEvaluatorData = (IEvaluatorData)nodeItem.Serializable();
        return base.Serializable();
    }

    public override void Deserializable()
    {
        DecisionFactorData decisionFactorData = this.data as DecisionFactorData;
        this.evaluatorContainer.Clear();
        if (decisionFactorData.evaluatorData is CompositeTwoPointEvaluatorData)
        {
            this.evaluatorContainer.Add(new UtilityCompositeTwoPointEvaluatorDataItem(decisionFactorData.evaluatorData));
        }
        else
        {
            this.evaluatorContainer.Add(new UtilityNodeItem(decisionFactorData.evaluatorData));
        }
        base.Deserializable();
    }

    protected override void OnAddAction(DropdownMenu menu)
    {
        var lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IEvaluatorData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            if (type == typeof(CompositeTwoPointEvaluatorData))
            {
                menu.AppendAction(string.Format("IEvaluatorData/Composite/{0}", lst[i].Name), action =>
                {
                    evaluatorContainer.Clear();
                    var item = new UtilityCompositeTwoPointEvaluatorDataItem(Activator.CreateInstance(type));
                    evaluatorContainer.Add(item);
                });
            }
            else
            {
                menu.AppendAction(string.Format("IEvaluatorData/{0}", lst[i].Name), action =>
                {
                    evaluatorContainer.Clear();
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    evaluatorContainer.Add(item);
                });
            }
           
        }

       
    }
}
