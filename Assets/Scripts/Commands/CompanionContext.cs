using System.Collections.Generic;
using UnityEngine;

namespace AIRunner.Commands
{
    // 自然言語変換処理への入力。ICommandInterpreter.Interpretに渡される。
    public struct CompanionContext
    {
        public string PlayerMessage;
        public IReadOnlyList<ConversationTurn> History;
        public Vector3 PlayerPosition;
        public Vector3 CompanionPosition;
        public CompanionState CompanionState;
    }
}
