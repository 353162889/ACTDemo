namespace Game
{
    public class MoveStateComponent : DataComponent
    {
        public static float defaultSpeed = 5;
        public MoveStateType moveStateType = MoveStateType.Walk;
        public float walkSpeed = 5;
        public float runSpeed = 10;
    }
}