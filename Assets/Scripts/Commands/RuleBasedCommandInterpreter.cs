using UnityEngine;

namespace AIRunner.Commands
{
    // 日本語のキーワード一致による簡易コマンド解釈。
    public class RuleBasedCommandInterpreter : ICommandInterpreter
    {
        public CompanionResponse Interpret(CompanionContext context)
        {
            string text = context.PlayerMessage;

            if (string.IsNullOrWhiteSpace(text))
            {
                return Unknown(text);
            }

            string t = text.Trim();

            if (Contains(t, "ジャンプ", "跳んで", "飛んで"))
            {
                return CompanionResponse.Of(
                    CompanionCommand.Of(CommandType.Jump, text),
                    "ジャンプ!",
                    EmotionType.Excited,
                    "Jump");
            }

            if (Contains(t, "止まって", "ストップ", "止まれ"))
            {
                return CompanionResponse.Of(
                    CompanionCommand.Of(CommandType.Stop, text),
                    "止まるね",
                    EmotionType.Neutral,
                    "Stop");
            }

            if (Contains(t, "待って", "待機"))
            {
                return CompanionResponse.Of(
                    CompanionCommand.Of(CommandType.Wait, text),
                    "ここで待ってる",
                    EmotionType.Neutral,
                    "Idle");
            }

            if (Contains(t, "ついてきて", "ついて来て", "来て", "追いかけて"))
            {
                return CompanionResponse.Of(
                    CompanionCommand.Of(CommandType.Follow, text),
                    "了解!ついていくよ",
                    EmotionType.Happy,
                    "Run");
            }

            if (Contains(t, "右"))
            {
                return GoTo(text, Vector3.right);
            }

            if (Contains(t, "左"))
            {
                return GoTo(text, Vector3.left);
            }

            if (Contains(t, "前", "進んで"))
            {
                return GoTo(text, Vector3.forward);
            }

            if (Contains(t, "後ろ", "戻って"))
            {
                return GoTo(text, Vector3.back);
            }

            return Unknown(text);
        }

        private static CompanionResponse GoTo(string text, Vector3 direction)
        {
            return CompanionResponse.Of(
                CompanionCommand.Of(CommandType.GoTo, text, direction),
                "そっちに行くよ",
                EmotionType.Happy,
                "Run");
        }

        private static CompanionResponse Unknown(string text)
        {
            return CompanionResponse.Of(
                CompanionCommand.Of(CommandType.Unknown, text),
                "ごめん、わからないよ",
                EmotionType.Confused,
                "");
        }

        private static bool Contains(string text, params string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                if (text.Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
