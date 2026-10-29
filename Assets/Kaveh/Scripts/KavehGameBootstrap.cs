using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// دوربین افقی، زمین و کاوه را می‌سازد.
    /// اگر در صحنه LevelBackground باشد، دوربین زوم شده و فقط در محدوده تصویر حرکت می‌کند.
    /// اگر در صحنه نباشد، KavehAutoStart خودکار آن را می‌سازد.
    /// </summary>
    public class KavehGameBootstrap : MonoBehaviour
    {
        [Tooltip("اختیاری. اگر خالی باشد، تنظیمات پیش‌فرض و اسپرایت Resources/Kaveh/kaveh_idle استفاده می‌شود.")]
        public KavehConfig config;
        public float groundTopY = -2.2f;
        public float spawnX = 0f;
        [Tooltip("اندازه دوربین وقتی پس‌زمینه مرحله وجود ندارد")]
        public float cameraSize = 3.5f;
        [Tooltip("نمایش مستطیل تیره زمین. در مرحله با پس‌زمینه خاموش است.")]
        public bool showGround = true;
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
            var k = KavehController.Spawn(config, new Vector2(spawnX, groundTopY + 1.2f));
            kaveh = k.transform;

            UpdateCamera(true);
        }

        void CreateGround()
        {
            float width = 400f;
            const float thickness = 4f;
            var lb = LevelBackground.Current;
            if (lb != null) width = lb.Width + 8f;

            var g = new GameObject("Ground");
            g.transform.position = new Vector3(0f, groundTopY - thickness * 0.5f, 0f);
            g.transform.localScale = new Vector3(width, thickness, 1f);

            if (showGround)
            {
                var tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                sr.color = groundColor;
            }

            var box = g.AddComponent<BoxCollider2D>();
            box.size = Vector2.one; // نسبت به scale
        }

        void LateUpdate()
        {
            UpdateCamera(false);
        }

        void UpdateCamera(bool snap)
        {
            if (kaveh == null || cam == null) return;
            var lb = LevelBackground.Current;
            var p = cam.transform.position;

            if (lb != null)
            {
                float size = lb.ComputeOrthoSize(cam.aspect);
                cam.orthographicSize = size;
                float half = size * cam.aspect;
                float targetX = snap ? kaveh.position.x : Mathf.Lerp(p.x, kaveh.position.x, 6f * Time.deltaTime);
                p.x = lb.ClampCameraX(targetX, half);
                p.y = lb.Bottom + size;   // پایین تصویر همیشه ته صفحه، خیابان دیده شود
            }
            else
            {
                p.x = snap ? kaveh.position.x : Mathf.Lerp(p.x, kaveh.position.x, 6f * Time.deltaTime);
                p.y = groundTopY + cameraSize * 0.55f;
            }
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
