using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// پس‌زمینه ثابت مرحله (بدون تکرار). کل مرحله همین تصویر است:
    /// دوربین کمی زوم می‌کند و فقط در محدوده تصویر حرکت می‌کند، و کاوه با دیوارهای نامرئی
    /// لبه‌های تصویر نگه داشته می‌شود. آبجکت‌های مرحله در مختصات دنیا ثابت‌اند و با تصویر حرکت نمی‌کنند.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class LevelBackground : MonoBehaviour
    {
        public Sprite sprite;
        [Tooltip("ارتفاع تصویر در دنیای بازی")]
        public float worldHeight = 10f;
        [Tooltip("پایین تصویر (y دنیا)")]
        public float bottomY = -3.7f;
        [Tooltip("چه کسری از عرض تصویر در یک لحظه دیده شود (کمتر = زوم بیشتر)")]
        [Range(0.4f, 1f)] public float viewFraction = 0.78f;
        [Tooltip("فاصله دیوار نامرئی از لبه تصویر")]
        public float wallInset = 0.35f;
        public int sortingOrder = -100;

        public static LevelBackground Current { get; private set; }

        public float Width { get; private set; }
        public float Left { get { return -Width * 0.5f; } }
        public float Right { get { return Width * 0.5f; } }
        public float Bottom { get { return bottomY; } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { Current = null; }

        void Awake()
        {
            Current = this;
            if (sprite == null) sprite = Resources.Load<Sprite>("Backgrounds/level1");
            if (sprite == null) return;

            float scale = worldHeight / sprite.bounds.size.y;
            Width = sprite.bounds.size.x * scale;

            var g = new GameObject("Image");
            g.transform.SetParent(transform, false);
            g.transform.position = new Vector3(0f, bottomY + worldHeight * 0.5f, 5f);
            g.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            CreateWall("WallLeft", Left + wallInset - 0.5f);
            CreateWall("WallRight", Right - wallInset + 0.5f);
        }

        void OnDestroy() { if (Current == this) Current = null; }

        void CreateWall(string name, float x)
        {
            var w = new GameObject(name);
            w.transform.SetParent(transform, false);
            w.transform.position = new Vector3(x, bottomY + worldHeight * 0.5f, 0f);
            var box = w.AddComponent<BoxCollider2D>();
            box.size = new Vector2(1f, worldHeight + 6f);
        }

        /// <summary>اندازه دوربین برای این نسبت صفحه.</summary>
        public float ComputeOrthoSize(float aspect)
        {
            float s = Width * viewFraction / Mathf.Max(0.1f, aspect) * 0.5f;
            return Mathf.Min(s, worldHeight * 0.5f);
        }

        public float ClampCameraX(float x, float halfViewWidth)
        {
            float min = Left + halfViewWidth, max = Right - halfViewWidth;
            if (min > max) return 0f;
            return Mathf.Clamp(x, min, max);
        }
    }
}
