using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// دوربین افقی، زمین و کاوه را می‌سازد. (فعلاً بدون پس‌زمینه و مرحله)
    /// اگر در صحنه نباشد، KavehAutoStart خودکار آن را می‌سازد.
    /// </summary>
    public class KavehGameBootstrap : MonoBehaviour
    {
        [Tooltip("اختیاری. اگر خالی باشد، تنظیمات پیش‌فرض و اسپرایت Resources/Kaveh/kaveh_idle استفاده می‌شود.")]
        public KavehConfig config;
        public float groundTopY = -2.2f;
        [Tooltip("نمایش مستطیل تیره زمین. در مرحله با پس‌زمینه خاموش است.")]
        public bool showGround = true;
        public float cameraSize = 3.5f;
        public Color backgroundColor = new Color(0.16f, 0.30f, 0.33f);
        public Color groundColor = new Color(0.04f, 0.09f, 0.10f);

        Transform kaveh;
        Camera cam;

        void Awake()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
            Application.targetFrameRate = 60;

            cam = Camera.main;
            if (cam == null)
            {
                var cg = new GameObject("Main Camera");
                cg.tag = "MainCamera";
                cam = cg.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = cameraSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = backgroundColor;

            // نسخه زمان اجرا، تا فایل Asset تغییر نکند
            config = config == null ? ScriptableObject.CreateInstance<KavehConfig>() : Instantiate(config);
            if (config.idleSprite == null)
                config.idleSprite = Resources.Load<Sprite>("Kaveh/kaveh_idle");

            CreateGround();
            var k = KavehController.Spawn(config, new Vector2(0f, groundTopY + 1.2f));
            kaveh = k.transform;
        }

        void CreateGround()
        {
            const float width = 400f, thickness = 4f;
            var g = new GameObject("Ground");
            g.transform.position = new Vector3(0f, groundTopY - thickness * 0.5f, 0f);
            g.transform.localScale = new Vector3(width, thickness, 1f);

            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            if (showGround)
            {
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                sr.color = groundColor;
            }

            var box = g.AddComponent<BoxCollider2D>();
            box.size = Vector2.one; // نسبت به scale
        }

        void LateUpdate()
        {
            if (kaveh == null) return;
            var p = cam.transform.position;
            p.x = Mathf.Lerp(p.x, kaveh.position.x, 6f * Time.deltaTime);
            p.y = groundTopY + cameraSize * 0.55f;
            p.z = -10f;
            cam.transform.position = p;
        }
    }

    /// <summary>اگر در صحنه KavehGameBootstrap نبود، خودکار می‌سازد.</summary>
    public static class KavehAutoStart
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
#if UNITY_2023_1_OR_NEWER
            var existing = Object.FindFirstObjectByType<KavehGameBootstrap>();
#else
            var existing = Object.FindObjectOfType<KavehGameBootstrap>();
#endif
            if (existing == null)
                new GameObject("KavehGame").AddComponent<KavehGameBootstrap>();
        }
    }
}
