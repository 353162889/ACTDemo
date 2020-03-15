namespace Game
{
    public enum JumpStateType
    {
        None,
        Jumping,//从起跳到开始着陆（落地）的过程
        JumpBeforeGround,//从开始着陆（落地）到真正落地的过程
    }
}