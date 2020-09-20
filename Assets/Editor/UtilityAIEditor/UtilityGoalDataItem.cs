using System;
using Framework;
using UnityEngine.UIElements;

public class UtilityGoalDataItem : UtilityNodeItem
{
    private VisualElement decisionFactorContainer;
    private VisualElement actionContainer;
    private VisualElement selectedInertiaEvaluatorContainer;
    private VisualElement unselectedInertiaEvaluatorContainer;
    public UtilityGoalDataItem(object data) : base(data)
    {
        decisionFactorContainer = this.Q<VisualElement>("decisionFactorContainer");
        actionContainer = this.Q<VisualElement>("actionContainer");

        selectedInertiaEvaluatorContainer = this.Q<VisualElement>("selectedInertiaEvaluatorContainer");
        unselectedInertiaEvaluatorContainer = this.Q<VisualElement>("unselectedInertiaEvaluatorContainer");
    }

    public override object Serializable()
    {
        UtilityGoalData utilityGoalData = this.data as UtilityGoalData;
        UtilityNodeItem nodeItem = this.decisionFactorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        utilityGoalData.mDecisionFactorData = (IDecisionFactorData)nodeItem.Serializable();

        nodeItem = this.actionContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        utilityGoalData.mActionData = (IUtilityActionData)nodeItem.Serializable();

        nodeItem = this.selectedInertiaEvaluatorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        utilityGoalData.mSelectedInertiaEvaluatorData = null;
        if (nodeItem != null)
        {
            utilityGoalData.mSelectedInertiaEvaluatorData = (IEvaluatorData) nodeItem.Serializable();
        }

        nodeItem = this.unselectedInertiaEvaluatorContainer.GetFirstLayerChildElement<UtilityNodeItem>();
        utilityGoalData.mUnselectedInertiaEvaluatorData = null;
        if (nodeItem != null)
        {
            utilityGoalData.mUnselectedInertiaEvaluatorData = (IEvaluatorData) nodeItem.Serializable();
        }

        return base.Serializable();
    }

    public override void Deserializable()
    {
        UtilityGoalData utilityGoalData = this.data as UtilityGoalData;
        this.selectedInertiaEvaluatorContainer.Clear();
        if (utilityGoalData.mSelectedInertiaEvaluatorData != null)
        {
            if (utilityGoalData.mSelectedInertiaEvaluatorData is CompositeTwoPointEvaluatorData)
            {
                this.selectedInertiaEvaluatorContainer.Add(
                    new UtilityCompositeTwoPointEvaluatorDataItem(utilityGoalData.mSelectedInertiaEvaluatorData));
            }
            else
            {
                this.selectedInertiaEvaluatorContainer.Add(
                    new UtilityNodeItem(utilityGoalData.mSelectedInertiaEvaluatorData));
            }
        }

        this.unselectedInertiaEvaluatorContainer.Clear();
        if (utilityGoalData.mUnselectedInertiaEvaluatorData != null)
        {
            if (utilityGoalData.mUnselectedInertiaEvaluatorData is CompositeTwoPointEvaluatorData)
            {
                this.unselectedInertiaEvaluatorContainer.Add(
                    new UtilityCompositeTwoPointEvaluatorDataItem(utilityGoalData.mUnselectedInertiaEvaluatorData));
            }
            else
            {
                this.unselectedInertiaEvaluatorContainer.Add(
                    new UtilityNodeItem(utilityGoalData.mUnselectedInertiaEvaluatorData));
            }
        }

        decisionFactorContainer.Clear();
        if (utilityGoalData.decisionFactorData is ICompositeDecisionFactorData)
        {
            decisionFactorContainer.Add(new UtilityCompositeDecisionFactorDataItem(utilityGoalData.decisionFactorData));
        }
        else if (utilityGoalData.decisionFactorData is DecisionFactorData)
        {
            decisionFactorContainer.Add(new UtilityDecisionFactorDataItem(utilityGoalData.decisionFactorData));
        }
        else
        {
            decisionFactorContainer.Add(new UtilityNodeItem(utilityGoalData.decisionFactorData));
        }

        actionContainer.Clear();
        actionContainer.Add(new UtilityNodeItem(utilityGoalData.actionData));
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
                menu.AppendAction(string.Format("selectedInertiaEvaluator/Composite/{0}", lst[i].Name), action =>
                {
                    selectedInertiaEvaluatorContainer.Clear();
                    var item = new UtilityCompositeTwoPointEvaluatorDataItem(Activator.CreateInstance(type));
                    selectedInertiaEvaluatorContainer.Add(item);
                });
            }
            else
            {
                menu.AppendAction(string.Format("selectedInertiaEvaluator/{0}", lst[i].Name), action =>
                {
                    selectedInertiaEvaluatorContainer.Clear();
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    selectedInertiaEvaluatorContainer.Add(item);
                });
            }
        }
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            if (type == typeof(CompositeTwoPointEvaluatorData))
            {
                menu.AppendAction(string.Format("unselectedInertiaEvaluator/Composite/{0}", lst[i].Name), action =>
                {
                    unselectedInertiaEvaluatorContainer.Clear();
                    var item = new UtilityCompositeTwoPointEvaluatorDataItem(Activator.CreateInstance(type));
                    unselectedInertiaEvaluatorContainer.Add(item);
                });
            }
            else
            {
                menu.AppendAction(string.Format("unselectedInertiaEvaluator/{0}", lst[i].Name), action =>
                {
                    unselectedInertiaEvaluatorContainer.Clear();
                    var item = new UtilityNodeItem(Activator.CreateInstance(type));
                    unselectedInertiaEvaluatorContainer.Add(item);
                });
            }

        }

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(ICompositeDecisionFactorData));
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

        lst = UtilityAIInitialize.GetUtilityDataTypesByInterface(typeof(IUtilityActionData));
        for (int i = 0; i < lst.Count; i++)
        {
            var type = lst[i];
            menu.AppendAction(string.Format("IUtilityActionData/{0}", lst[i].Name), action =>
            {
                actionContainer.Clear();
                var item = new UtilityNodeItem(Activator.CreateInstance(type));
                actionContainer.Add(item);
            });
        }
    }
}
