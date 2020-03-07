namespace Framework
{
    public abstract class BTCondition<T1, T2> : BTDataHandler<T1, T2> where T1 : IBTContext
    {
        sealed protected override BTStatus Handler(T1 context, BTData btData, T2 data)
        {
            if (Evaluate(context, btData, data))
            {
                return BTStatus.Success;
            }
            else
            {
                return BTStatus.Fail;
            }
        }

        protected virtual bool Evaluate(T1 context, BTData btData, T2 data)
        {
            return true;
        }
    }
}