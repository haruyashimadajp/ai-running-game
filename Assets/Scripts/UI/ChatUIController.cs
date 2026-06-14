using AIRunner.Commands;
using AIRunner.Companion;
using UnityEngine;
using UnityEngine.UI;

namespace AIRunner.UI
{
    // チャット風の入力UI。プレイヤーの発言をAICompanionBrainへ送り、
    // 応答(セリフ+感情)をログに表示する。
    public class ChatUIController : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Text logText;
        [SerializeField] private AICompanionBrain companionBrain;
        [SerializeField] private int maxLogLines = 12;

        private void OnEnable()
        {
            if (companionBrain != null)
            {
                companionBrain.OnResponse += HandleCompanionResponse;
            }

            if (inputField != null)
            {
                inputField.onSubmit.AddListener(_ => Send());
            }
        }

        private void OnDisable()
        {
            if (companionBrain != null)
            {
                companionBrain.OnResponse -= HandleCompanionResponse;
            }

            if (inputField != null)
            {
                inputField.onSubmit.RemoveListener(_ => Send());
            }
        }

        public void Send()
        {
            if (inputField == null || string.IsNullOrWhiteSpace(inputField.text))
            {
                return;
            }

            string message = inputField.text.Trim();
            AppendLog($"あなた: {message}");

            if (companionBrain != null)
            {
                companionBrain.ReceiveMessage(message);
            }

            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        private void HandleCompanionResponse(CompanionResponse response)
        {
            string emotionLabel = EmotionLabel(response.Emotion);
            AppendLog($"AI{emotionLabel}: {response.DialogueText}");
        }

        private static string EmotionLabel(EmotionType emotion)
        {
            switch (emotion)
            {
                case EmotionType.Happy:
                    return "(嬉しい)";
                case EmotionType.Excited:
                    return "(はりきって)";
                case EmotionType.Confused:
                    return "(困惑)";
                case EmotionType.Worried:
                    return "(心配)";
                case EmotionType.Apologetic:
                    return "(申し訳なさそう)";
                case EmotionType.Neutral:
                default:
                    return "";
            }
        }

        private void AppendLog(string line)
        {
            if (logText == null)
            {
                return;
            }

            logText.text += (string.IsNullOrEmpty(logText.text) ? "" : "\n") + line;

            string[] lines = logText.text.Split('\n');
            if (lines.Length > maxLogLines)
            {
                int skip = lines.Length - maxLogLines;
                logText.text = string.Join("\n", lines, skip, maxLogLines);
            }
        }
    }
}
