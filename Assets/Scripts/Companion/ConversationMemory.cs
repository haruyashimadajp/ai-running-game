using System.Collections.Generic;
using AIRunner.Commands;

namespace AIRunner.Companion
{
    // 直近の会話履歴を保持し、CompanionContext.Historyとして渡す。
    public class ConversationMemory
    {
        private readonly int maxTurns;
        private readonly List<ConversationTurn> turns = new List<ConversationTurn>();

        public ConversationMemory(int maxTurns = 10)
        {
            this.maxTurns = maxTurns;
        }

        public IReadOnlyList<ConversationTurn> Turns => turns;

        public void Add(ConversationSpeaker speaker, string text)
        {
            turns.Add(new ConversationTurn { Speaker = speaker, Text = text });

            while (turns.Count > maxTurns)
            {
                turns.RemoveAt(0);
            }
        }
    }
}
