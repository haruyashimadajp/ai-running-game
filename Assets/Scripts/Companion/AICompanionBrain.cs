using System;
using AIRunner.Commands;
using UnityEngine;

namespace AIRunner.Companion
{
    // プレイヤーからのテキスト指示を受け取り、AICompanionControllerの行動と
    // セリフ・感情・アニメーションを決定する。
    // ICommandInterpreterを差し替えることで、ルールベース→ローカルLLMベースへの
    // 移行が可能。
    [RequireComponent(typeof(AICompanionController))]
    public class AICompanionBrain : MonoBehaviour
    {
        public event Action<CompanionResponse> OnResponse;

        private AICompanionController controller;
        private ICommandInterpreter interpreter;
        private readonly ConversationMemory memory = new ConversationMemory();

        private void Awake()
        {
            controller = GetComponent<AICompanionController>();
            interpreter = new RuleBasedCommandInterpreter();
        }

        public void SetInterpreter(ICommandInterpreter newInterpreter)
        {
            interpreter = newInterpreter;
        }

        public void ReceiveMessage(string text)
        {
            memory.Add(ConversationSpeaker.Player, text);

            CompanionContext context = BuildContext(text);
            CompanionResponse response = interpreter.Interpret(context);

            ApplyCommand(response.Command);
            memory.Add(ConversationSpeaker.Companion, response.DialogueText);

            OnResponse?.Invoke(response);
        }

        private CompanionContext BuildContext(string text)
        {
            Transform player = controller.FollowTarget;

            return new CompanionContext
            {
                PlayerMessage = text,
                History = memory.Turns,
                PlayerPosition = player != null ? player.position : Vector3.zero,
                CompanionPosition = transform.position,
                CompanionState = controller.CurrentState
            };
        }

        private void ApplyCommand(CompanionCommand command)
        {
            switch (command.Type)
            {
                case CommandType.Follow:
                    controller.Follow();
                    break;
                case CommandType.Stop:
                    controller.Stop();
                    break;
                case CommandType.Wait:
                    controller.Wait();
                    break;
                case CommandType.Jump:
                    controller.Jump();
                    break;
                case CommandType.GoTo:
                    controller.MoveInDirection(command.Direction);
                    break;
                case CommandType.Unknown:
                default:
                    break;
            }
        }
    }
}
