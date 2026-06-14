using AIRunner.CameraControl;
using AIRunner.Companion;
using AIRunner.Player;
using AIRunner.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AIRunner.EditorTools
{
    // Tools > AI Runner > Build Demo Scene で、プレイヤー・AI仲間・カメラ・
    // チャットUIをひとまとめに配置した検証用シーンを構築する。
    public static class DemoSceneBuilder
    {
        [MenuItem("Tools/AI Runner/Build Demo Scene")]
        public static void Build()
        {
            CreateGround();
            CreateObstacles();

            GameObject player = CreatePlayer();
            GameObject companion = CreateCompanion(player.transform);

            SetupCamera(player.transform);
            SetupChatUI(companion);

            Debug.Log("Demo scene built. Press Play and type messages in the chat box.");
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
        }

        private static void CreateObstacles()
        {
            Vector3[] positions =
            {
                new Vector3(3f, 0.5f, 5f),
                new Vector3(-4f, 0.5f, 10f),
                new Vector3(2f, 0.5f, 15f),
                new Vector3(-2f, 0.5f, 20f),
            };

            foreach (Vector3 position in positions)
            {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = "Obstacle";
                obstacle.transform.position = position;
                obstacle.transform.localScale = Vector3.one;
                Undo.RegisterCreatedObjectUndo(obstacle, "Create Obstacle");
            }
        }

        private static GameObject CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, -5f);

            RemoveCollider<CapsuleCollider>(player);
            player.AddComponent<CharacterController>();
            player.AddComponent<PlayerController>();

            Undo.RegisterCreatedObjectUndo(player, "Create Player");
            return player;
        }

        private static GameObject CreateCompanion(Transform followTarget)
        {
            GameObject companion = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            companion.name = "AICompanion";
            companion.transform.position = new Vector3(2f, 1f, -5f);

            Renderer renderer = companion.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = new Material(renderer.sharedMaterial);
                material.color = new Color(0.3f, 0.6f, 1f);
                renderer.sharedMaterial = material;
            }

            RemoveCollider<CapsuleCollider>(companion);
            companion.AddComponent<CharacterController>();
            AICompanionController controller = companion.AddComponent<AICompanionController>();
            AICompanionBrain brain = companion.AddComponent<AICompanionBrain>();

            controller.SetFollowTarget(followTarget);

            Undo.RegisterCreatedObjectUndo(companion, "Create AI Companion");
            return companion;
        }

        private static void SetupCamera(Transform target)
        {
            Camera camera = Camera.main;
            GameObject cameraObject;
            if (camera == null)
            {
                cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
                Undo.RegisterCreatedObjectUndo(cameraObject, "Create Main Camera");
            }
            else
            {
                cameraObject = camera.gameObject;
            }

            CameraFollow follow = cameraObject.GetComponent<CameraFollow>();
            if (follow == null)
            {
                follow = cameraObject.AddComponent<CameraFollow>();
            }
            follow.SetTarget(target);
        }

        private static void SetupChatUI(GameObject companion)
        {
            EnsureEventSystem();

            GameObject canvasObject = new GameObject("ChatCanvas", typeof(RectTransform));
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Chat Canvas");

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            // 背景パネル
            GameObject panel = CreateUIObject("ChatPanel", canvasObject.transform);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(0f, 0f);
            panelRect.pivot = new Vector2(0f, 0f);
            panelRect.anchoredPosition = new Vector2(20f, 20f);
            panelRect.sizeDelta = new Vector2(420f, 220f);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.5f);

            // ログ表示
            GameObject logObject = CreateUIObject("LogText", panel.transform);
            RectTransform logRect = logObject.GetComponent<RectTransform>();
            logRect.anchorMin = new Vector2(0f, 0.25f);
            logRect.anchorMax = new Vector2(1f, 1f);
            logRect.offsetMin = new Vector2(10f, 0f);
            logRect.offsetMax = new Vector2(-10f, -10f);
            Text logText = logObject.AddComponent<Text>();
            logText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            logText.fontSize = 16;
            logText.color = Color.white;
            logText.alignment = TextAnchor.LowerLeft;
            logText.horizontalOverflow = HorizontalWrapMode.Wrap;
            logText.verticalOverflow = VerticalWrapMode.Truncate;
            logText.text = string.Empty;

            // 入力欄
            InputField inputField = CreateInputField("MessageInput", panel.transform);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0f, 0f);
            inputRect.anchorMax = new Vector2(0.7f, 0.22f);
            inputRect.offsetMin = new Vector2(10f, 10f);
            inputRect.offsetMax = new Vector2(-5f, 0f);

            // 送信ボタン
            Button sendButton = CreateButton("SendButton", panel.transform, "送信");
            RectTransform buttonRect = sendButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.7f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0.22f);
            buttonRect.offsetMin = new Vector2(5f, 10f);
            buttonRect.offsetMax = new Vector2(-10f, 0f);

            // ChatUIControllerの配線
            AICompanionBrain brain = companion.GetComponent<AICompanionBrain>();
            ChatUIController chatUI = canvasObject.AddComponent<ChatUIController>();

            SerializedObject serializedChatUI = new SerializedObject(chatUI);
            serializedChatUI.FindProperty("inputField").objectReferenceValue = inputField;
            serializedChatUI.FindProperty("logText").objectReferenceValue = logText;
            serializedChatUI.FindProperty("companionBrain").objectReferenceValue = brain;
            serializedChatUI.ApplyModifiedPropertiesWithoutUndo();

            sendButton.onClick.AddListener(chatUI.Send);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create Event System");
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static InputField CreateInputField(string name, Transform parent)
        {
            GameObject obj = CreateUIObject(name, parent);
            Image background = obj.AddComponent<Image>();
            background.color = Color.white;

            InputField inputField = obj.AddComponent<InputField>();

            GameObject textObject = CreateUIObject("Text", obj.transform);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;
            text.supportRichText = false;

            GameObject placeholderObject = CreateUIObject("Placeholder", obj.transform);
            RectTransform placeholderRect = placeholderObject.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(8f, 4f);
            placeholderRect.offsetMax = new Vector2(-8f, -4f);
            Text placeholder = placeholderObject.AddComponent<Text>();
            placeholder.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            placeholder.fontSize = 16;
            placeholder.color = new Color(0f, 0f, 0f, 0.5f);
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.text = "AIに指示を入力 (例: ついてきて)";

            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.targetGraphic = background;

            return inputField;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            GameObject obj = CreateUIObject(name, parent);
            Image background = obj.AddComponent<Image>();
            background.color = new Color(0.2f, 0.6f, 1f);

            Button button = obj.AddComponent<Button>();
            button.targetGraphic = background;

            GameObject textObject = CreateUIObject("Text", obj.transform);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = label;

            return button;
        }

        private static void RemoveCollider<T>(GameObject obj) where T : Collider
        {
            T collider = obj.GetComponent<T>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }
    }
}
