namespace AIRunner.Commands
{
    // AI仲間の現在の行動状態。CompanionContextの一部として
    // 自然言語変換処理に渡されるため、Commands名前空間に置く。
    public enum CompanionState
    {
        Idle,
        Follow,
        MoveTo,
        Waiting
    }
}
