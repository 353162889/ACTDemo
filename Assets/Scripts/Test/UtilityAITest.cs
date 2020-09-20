using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

[Serializable]
public class TestContext : IUtilityContext
{
    [Range(0, 100)]
    public float hunger;
    [Range(0, 100)]
    public float thirst;

    public float time
    {
        get { return Time.time; }
    }
}

//public class HungerDecisionFactor : DecisionFactorBase<TestContext, DecisionFactorData>
//{
//    public override float weight
//    {
//        get { return this.convertData.weight; }
//    }
//
//    private IDecisionFactorEvaluator m_cEvaluator;
//
//    public HungerDecisionFactor()
//    {
//    }
//
//    public override UtilityValue Decision(TestContext context)
//    {
//        return new UtilityValue(m_cEvaluator.Evaluate(context.hunger), weight);
//    }
//
//    protected override void OnInit(DecisionFactorData data)
//    {
//        m_cEvaluator = (IDecisionFactorEvaluator) this.utilityAI.CreateUtility(data.evaluatorData);
//    }
//}
//
//public class ThirstDecisionFactor : DecisionFactorBase<TestContext, DecisionFactorData>
//{
//    public override float weight
//    {
//        get { return this.convertData.weight; }
//    }
//
//    private IDecisionFactorEvaluator m_cEvaluator;
//
//    public ThirstDecisionFactor(IDecisionFactorEvaluator evaluator)
//    {
//    }
//
//    public override UtilityValue Decision(TestContext context)
//    {
//        return new UtilityValue(m_cEvaluator.Evaluate(context.thirst), weight);
//    }
//
//    protected override void OnInit(DecisionFactorData data)
//    {
//        m_cEvaluator = (IDecisionFactorEvaluator) this.utilityAI.CreateUtility(data.evaluatorData);
//    }
//}

public class UtilityAITest : MonoBehaviour
{
    private UtilityAI utilityAI;
    public TestContext context;
    void Start()
    {
//        context = new TestContext();
//        context.hunger = 100;
//        context.thirst = 100;
//
//        MaxUtilitySelector selector = new MaxUtilitySelector();
//        utilityAI = new UtilityAI(selector);
//
//        //吃东西
//        UtilityAction eatAction = new UtilityAction("eat");
//        HungerDecisionFactor hungerDecisionFactor = new HungerDecisionFactor(new LinearTwoPointEvaluator(new Vector2(0, 1), new Vector2(100, 0)), 1);
//        UtilityGoal eatGoal = new UtilityGoal(eatAction, hungerDecisionFactor);
//        utilityAI.AddUtilityGoal(eatGoal);
//        //喝水
//        UtilityAction drinkAction = new UtilityAction("drink");
//        ThirstDecisionFactor thirstDecisionFactor = new ThirstDecisionFactor(new LinearTwoPointEvaluator(new Vector2(0, 1), new Vector2(100, 0)), 1);
//        UtilityGoal drinkGoal = new UtilityGoal(drinkAction, thirstDecisionFactor);
//        utilityAI.AddUtilityGoal(drinkGoal);
//        //打豆豆
    }

    void Update()
    {
//        UtilityAction action = (UtilityAction)utilityAI.Select(context);
//        if (action != null)
//        {
//            Debug.Log("action:"+action.name);
//        }
    }
}
