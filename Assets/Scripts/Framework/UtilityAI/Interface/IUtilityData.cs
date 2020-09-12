using System;
using System.Collections.Generic;

namespace Framework
{
    public interface IUtilityData
    {
    }

    public interface IUtilityAIData : IUtilityData
    {
        ISelectorData selectorData { get; }
        List<IUtilityGoalData> lstGoalDatas { get; }
    }

    public interface IEvaluatorData : IUtilityData { }

    public interface ICombineData : IUtilityData { }

    public interface IDecisionFactorData : IUtilityData
    {
        float weight { get; }
        IEvaluatorData evaluatorData { get; }
    }

    public interface ICompositeDecisionFactorData : IDecisionFactorData
    {
        ICombineData combineData { get; }
        List<IDecisionFactorData> lstFactor { get; }
    }

    public interface ISelectorData : IUtilityData { }

    public interface IUtilityActionData : IUtilityData
    {
    }

    public interface IUtilityGoalData : IUtilityData
    {
        IUtilityActionData actionData { get; }
        IDecisionFactorData decisionFactorData { get; }
        float weight { get; }
    }

    public interface ICompositeUtilityGoalData : IUtilityGoalData
    {
        ISelectorData selectorData { get; }
        List<IUtilityGoalData> lstGoalDatas { get; }
    }
}