namespace AIRunner.Commands
{
    // 自然言語変換処理からの出力。ICommandInterpreter.Interpretの戻り値。
    public struct CompanionResponse
    {
        public CompanionCommand Command;
        public string DialogueText;
        public EmotionType Emotion;
        public string AnimationTrigger;

        public static CompanionResponse Of(CompanionCommand command, string dialogueText, EmotionType emotion, string animationTrigger = "")
        {
            return new CompanionResponse
            {
                Command = command,
                DialogueText = dialogueText,
                Emotion = emotion,
                AnimationTrigger = animationTrigger
            };
        }
    }
}
