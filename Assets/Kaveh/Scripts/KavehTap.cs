using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kaveh
{
    /// <summary>هر چیزی که با لمس/کلیک فعال می‌شود (دست برداشتن، خود کاوه، پنل کوله).</summary>
    public interface IKavehTapTarget
    {
        int TapPriority { get; }
        bool HitTest(Vector2 screenPos);
        void OnTap();
    }

    public static class KavehTapUtil
    {
        public static bool Hit(Bounds b, Vector2 screenPos, float margin)
        {
            var cam = Camera.main;
            if (cam == null) return false;
            Vector3 w = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            b.Expand(margin * 2f);
            w.z = b.center.z;
            return b.Contains(w);
        }
    }

    /// <summary>
    /// لمس را به بالاترین‌اولویت هدفی که زیر آن است می‌رساند و حرکت کاوه را در آن لمس خاموش می‌کند.
    /// خودکار ساخته می‌شود.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class KavehTapRouter : MonoBehaviour
    {
        static readonly List<IKavehTapTarget> targets = new List<IKavehTapTarget>();
        static KavehTapRouter instance;

        /// <summary>لمسی که هیچ هدفی نداشت.</summary>
        public static event Action<Vector2> TapMissed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { targets.Clear(); instance = null; TapMissed = null; }

        public static void Register(IKavehTapTarget t)
        {
            if (!targets.Contains(t)) targets.Add(t);
            if (instance == null)
            {
                var g = new GameObject("KavehTapRouter");
                instance = g.AddComponent<KavehTapRouter>();
            }
        }

        public static void Unregister(IKavehTapTarget t) { targets.Remove(t); }

        void Update()
        {
            Vector2 pos;
            if (!KavehInput.PointerPressedThisFrame(out pos)) return;

            IKavehTapTarget best = null;
            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                if (t.HitTest(pos) && (best == null || t.TapPriority > best.TapPriority)) best = t;
            }

            if (best != null)
            {
                KavehInput.SuppressPointerMovementUntilRelease();
                best.OnTap();
            }
            else if (TapMissed != null) TapMissed(pos);
        }
    }
}
