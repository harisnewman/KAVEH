using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// ظاهر کاوه. اگر اسکلت (Resources/Kaveh/Rig) موجود باشد از KavehRig استفاده می‌کند،
    /// در غیر این صورت اسپرایت ساده یا سیلوئت جایگزین. منطق حرکت اینجا نیست.
    /// </summary>
    public class KavehVisual : MonoBehaviour
    {
        SpriteRenderer sr;
        KavehConfig cfg;
        KavehRig rig;
        float animTime;
        float baseY;
        bool facingRight = true;
        bool useFrames;
        float reachT, reachDur;

        public bool UsesRig { get { return rig != null; } }

        public Vector3 BackpackWorldPosition
        {
            get { return rig != null ? rig.Backpack.position : transform.position + Vector3.up * (cfg != null ? cfg.visualHeight * 0.6f : 1f); }
        }

        public void Init(KavehConfig config, float feetLocalY)
        {
            cfg = config;
            baseY = feetLocalY;

            if (cfg.useRig)
            {
                rig = KavehRig.Build(transform, cfg.visualHeight);
                if (rig != null)
                {
                    float s = rig.Scale;
                    transform.localScale = new Vector3(s, s, 1f);
                    transform.localPosition = new Vector3(0f, feetLocalY, 0f);
                    return;
                }
            }

            sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.color = cfg.tint;
            sr.sortingOrder = 10;

            Sprite idle = cfg.idleSprite != null ? cfg.idleSprite : CreatePlaceholder();
            sr.sprite = idle;
            useFrames = cfg.walkFrames != null && cfg.walkFrames.Length > 0;

            float h = idle.bounds.size.y;
            float sc = h > 0.0001f ? cfg.visualHeight / h : 1f;
            transform.localScale = new Vector3(sc, sc, 1f);
            baseY = feetLocalY - idle.bounds.min.y * sc;
            transform.localPosition = new Vector3(0f, baseY, 0f);
        }

        /// <summary>دراز کردن دست برای برداشتن چیزی.</summary>
        public void PlayReach(float duration)
        {
            reachT = 0f;
            reachDur = duration;
        }

        public void FaceTowards(float worldX)
        {
            facingRight = worldX >= transform.position.x;
            ApplyFacing();
        }

        void ApplyFacing()
        {
            bool spriteRight = rig != null ? true : cfg.spriteFacesRight;
            float sign = (facingRight == spriteRight) ? 1f : -1f;
            var sc = transform.localScale;
            sc.x = Mathf.Abs(sc.x) * sign;
            transform.localScale = sc;
        }

        public void Tick(float velocityX, bool moving)
        {
            if (cfg == null) return;

            if (Mathf.Abs(velocityX) > 0.05f)
            {
                facingRight = velocityX > 0f;
                ApplyFacing();
            }

            if (rig != null)
            {
                float reach = 0f;
                if (reachDur > 0f)
                {
                    reachT += Time.deltaTime;
                    float k = reachT / reachDur;
                    if (k >= 1f) reachDur = 0f; else reach = Mathf.Sin(k * Mathf.PI);
                }
                rig.Tick(Time.deltaTime, Mathf.Abs(velocityX), cfg.strideCycleLength, reach);
                return;
            }

            if (moving)
            {
                animTime += Time.deltaTime;
                if (useFrames)
                {
                    int i = (int)(animTime * cfg.walkFps) % cfg.walkFrames.Length;
                    sr.sprite = cfg.walkFrames[i];
                    transform.localPosition = new Vector3(0f, baseY, 0f);
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    float w = animTime * 9f;
                    transform.localPosition = new Vector3(0f, baseY + Mathf.Abs(Mathf.Sin(w)) * 0.05f, 0f);
                    transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(w) * 2f);
                }
            }
            else
            {
                animTime = 0f;
                if (cfg.idleSprite != null) sr.sprite = cfg.idleSprite;
                transform.localPosition = new Vector3(0f, baseY, 0f);
                transform.localRotation = Quaternion.identity;
            }
        }

        static Sprite placeholder;
        static Sprite CreatePlaceholder()
        {
            if (placeholder != null) return placeholder;
            const int W = 32, H = 64;
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var clear = new Color32(0, 0, 0, 0);
            var black = new Color32(8, 10, 10, 255);
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    bool head = (x - 16) * (x - 16) + (y - 54) * (y - 54) <= 49;
                    bool body = x >= 10 && x <= 22 && y >= 20 && y <= 46;
                    bool legs = y < 20 && ((x >= 11 && x <= 15) || (x >= 17 && x <= 21));
                    tex.SetPixel(x, y, (head || body || legs) ? black : clear);
                }
            tex.Apply();
            placeholder = Sprite.Create(tex, new Rect(0, 0, W, H), new Vector2(0.5f, 0f), 32f);
            return placeholder;
        }
    }
}
