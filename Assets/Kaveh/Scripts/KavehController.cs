using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// حرکت کاوه روی زمین (دوبعدی، چپ و راست). ظاهر در KavehVisual و کوله‌پشتی در KavehInventory است.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class KavehController : MonoBehaviour
    {
        public KavehConfig config;

        Rigidbody2D rb;
        BoxCollider2D col;
        KavehVisual visual;
        float input;
        float frozenUntil;

        public KavehVisual Visual { get { return visual; } }
        public KavehInventory Inventory { get; private set; }

        public static KavehController Spawn(KavehConfig cfg, Vector2 position)
        {
            var go = new GameObject("Kaveh");
            go.transform.position = position;
            var c = go.AddComponent<KavehController>();
            c.config = cfg;
            c.Setup();
            return c;
        }

        void Awake()
        {
            if (config != null && visual == null) Setup();
        }

        void Setup()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<BoxCollider2D>();
            rb.gravityScale = config.gravityScale;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            col.size = config.colliderSize;
            col.offset = Vector2.zero;

            var v = new GameObject("Visual");
            v.transform.SetParent(transform, false);
            visual = v.AddComponent<KavehVisual>();
            visual.Init(config, -config.colliderSize.y * 0.5f);

            Inventory = gameObject.AddComponent<KavehInventory>();
        }

        /// <summary>چند ثانیه حرکت نکند (مثلاً هنگام برداشتن چیزی).</summary>
        public void FreezeFor(float seconds)
        {
            frozenUntil = Mathf.Max(frozenUntil, Time.time + seconds);
        }

        void Update()
        {
            if (config == null) return;
            float raw = KavehInput.Horizontal();
            input = Time.time < frozenUntil ? 0f : raw;
            visual.Tick(GetVelX(), Mathf.Abs(input) > 0.01f);
        }

        void FixedUpdate()
        {
            if (config == null) return;
            float target = input * config.walkSpeed;
            float vx = Mathf.MoveTowards(GetVelX(), target, config.acceleration * Time.fixedDeltaTime);
            SetVelX(vx);
        }

        float GetVelX()
        {
#if UNITY_6000_0_OR_NEWER
            return rb.linearVelocity.x;
#else
            return rb.velocity.x;
#endif
        }

        void SetVelX(float x)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
#else
            rb.velocity = new Vector2(x, rb.velocity.y);
#endif
        }
    }
}
