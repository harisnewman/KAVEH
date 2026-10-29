using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// تنظیمات کاوه. برای عوض کردن ظاهر یا سرعت، فقط این فایل (Asset) را عوض کنید.
    /// Create > KAVEH > Character Config
    /// </summary>
    [CreateAssetMenu(menuName = "KAVEH/Character Config", fileName = "KavehConfig")]
    public class KavehConfig : ScriptableObject
    {
        [Header("Visual")]
        [Tooltip("از اسکلت قطعه‌قطعه (Resources/Kaveh/Rig) و انیمیشن راه رفتن استفاده کن. اگر قطعات نبود، به اسپرایت ساده برمی‌گردد.")]
        public bool useRig = true;
        [Tooltip("فاصله‌ای که کاوه در یک چرخه کامل راه رفتن (دو قدم) طی می‌کند، به واحد دنیا")]
        public float strideCycleLength = 1.5f;
        [Tooltip("ارتفاع کاوه در دنیای بازی (واحد Unity)")]
        public float visualHeight = 1.8f;
        public Color tint = Color.white;

        [Header("Fallback sprite (بدون اسکلت)")]
        public Sprite idleSprite;
        public Sprite[] walkFrames;
        public float walkFps = 8f;
        public bool spriteFacesRight = true;

        [Header("Movement")]
        public float walkSpeed = 2f;
        public float acceleration = 25f;
        public float gravityScale = 3f;

        [Header("Collider")]
        public Vector2 colliderSize = new Vector2(0.45f, 1.7f);
    }
}
