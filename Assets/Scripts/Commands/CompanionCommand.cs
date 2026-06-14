using UnityEngine;

namespace AIRunner.Commands
{
    public enum CommandType
    {
        Follow,
        Stop,
        Wait,
        Jump,
        GoTo,
        Unknown
    }

    public struct CompanionCommand
    {
        public CommandType Type;
        public Vector3 Direction;
        public string RawText;

        public static CompanionCommand Of(CommandType type, string rawText, Vector3 direction = default)
        {
            return new CompanionCommand
            {
                Type = type,
                Direction = direction,
                RawText = rawText
            };
        }
    }
}
