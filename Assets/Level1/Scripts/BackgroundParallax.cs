using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// پس‌زمینه مرحله. تصویر با پارالکس آهسته حرکت می‌کند و برای ادامه‌دار شدن،
    /// تکه‌ها یکی در میان آینه می‌شوند تا لبه‌ها بدون درز به هم بچسبند.
    /// </summary>
    public class BackgroundParallax : MonoBehaviour
    {
        public Sprite sprite;
        [Range(0f, 1f)] public float parallax = 0.25f;
        public int sortingOrder = -100;
        public float zoom = 1.0f;

        SpriteRenderer[] tiles;
        Camera cam;

        void Awake()
        {
            tiles = new SpriteRenderer[3];
            for (int i = 0; i < 3; i++)
            {
                var g = new GameObject("Tile" + i);
                g.transform.SetParent(transform, false);
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = sortingOrder;
                tiles[i] = sr;
            }
        }

        void LateUpdate()
        {
            if (sprite == null) return;
            if (cam == null) cam = Camera.main;
            if (cam == null) return;

            float viewH = cam.orthographicSize * 2f;
            float viewW = viewH * cam.aspect;
            float sw = sprite.bounds.size.x, sh = sprite.bounds.size.y;
            float scale = Mathf.Max(viewH / sh, viewW / sw) * zoom;
            float tileW = sw * scale;

            Vector3 cp = cam.transform.position;
            float anchor = cp.x * (1f - parallax);
            int baseIdx = Mathf.RoundToInt((cp.x - anchor) / tileW);
            // تراز با پایین دید دوربین (خیابان همیشه دیده شود)
            float y = cp.y - viewH * 0.5f + sh * scale * 0.5f;

            for (int i = 0; i < 3; i++)
            {
                int n = baseIdx + i - 1;
                var t = tiles[i];
                t.transform.position = new Vector3(anchor + n * tileW, y, 5f);
                t.transform.localScale = new Vector3(scale, scale, 1f);
                t.flipX = ((n % 2) + 2) % 2 == 1;
            }
        }
    }
}
