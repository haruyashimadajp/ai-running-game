namespace AIRunner.Commands
{
    public enum ConversationSpeaker
    {
        Player,
        Companion
    }

    public struct ConversationTurn
    {
        public ConversationSpeaker Speaker;
        public string Text;
    }
}
