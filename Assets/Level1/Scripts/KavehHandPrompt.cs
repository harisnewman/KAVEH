using System;
using UnityEngine;

namespace Kaveh
{
    /// <summary>علامت دست بالای یک آبجکت. با لمس آن، onTap اجرا می‌شود.</summary>
    public class KavehHandPrompt : MonoBehaviour, IKavehTapTarget
    {
        SpriteRenderer sr;
        Action onTap;
        Transform owner;
        Vector3 basePos;
        float baseScale = 1f;
        bool shown;

        public static KavehHandPrompt Create(Transform owner, Action onTap)
        {
            var g = new GameObject("HandPrompt");
            g.transform.SetParent(owner, false);
            var h = g.AddComponent<KavehHandPrompt>();
            h.owner = owner;
            h.onTap = onTap;
            h.sr = g.AddComponent<SpriteRenderer>();
            h.sr.sprite = Resources.Load<Sprite>("Kaveh/UI/hand");
            h.sr.sortingOrder = 500;
            h.sr.enabled = false;
            if (h.sr.sprite != null)
                h.baseScale = 0.62f / h.sr.sprite.bounds.size.x / Mathf.Max(0.0001f, owner.lossyScale.x);
            g.transform.localScale = Vector3.one * h.baseScale;
            return h;
        }

        public void Show()
        {
            if (shown) return;
            shown = true;
            var osr = owner.GetComponent<SpriteRenderer>();
            Bounds b = osr != null ? osr.bounds : new Bounds(owner.position, Vector3.one * 0.5f);
            basePos = new Vector3(b.center.x, b.max.y + 0.55f, 0f);
            transform.position = basePos;
            sr.enabled = true;
            KavehTapRouter.Register(this);
        }

        public void Hide()
        {
            if (!shown) return;
            shown = false;
            sr.enabled = false;
            KavehTapRouter.Unregister(this);
        }

        void Update()
        {
            if (!shown) return;
            float p = 1f + 0.08f * Mathf.Sin(Time.time * 5f);
            transform.localScale = Vector3.one * (baseScale * p);
            transform.position = basePos + Vector3.up * (0.06f * Mathf.Sin(Time.time * 3f));
        }

        void OnDestroy() { KavehTapRouter.Unregister(this); }

        public int TapPriority { get { return 50; } }

        public bool HitTest(Vector2 screenPos)
        {
            return shown && KavehTapUtil.Hit(sr.bounds, screenPos, 0.2f);
        }

        public void OnTap() { if (onTap != null) onTap(); }
    }
}
