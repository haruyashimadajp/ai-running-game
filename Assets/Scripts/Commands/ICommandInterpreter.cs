namespace AIRunner.Commands
{
    // CompanionContext(発言+文脈)をCompanionResponse(行動+セリフ+感情+アニメーション)へ
    // 変換するインターフェース。
    // 最初はキーワード一致(RuleBasedCommandInterpreter)で実装し、
    // 将来的にローカルLLMベースの実装に差し替えられるようにする。
    public interface ICommandInterpreter
    {
        CompanionResponse Interpret(CompanionContext context);
    }
}
