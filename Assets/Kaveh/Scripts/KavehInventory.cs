using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// کوله‌پشتی کاوه. هرچه برداشته شود اینجا اضافه می‌شود. لمس کاوه پنل کوله را باز/بسته می‌کند.
    /// فهرست آیتم‌ها استاتیک است و بین مرحله‌ها می‌ماند.
    /// </summary>
    public class KavehInventory : MonoBehaviour, IKavehTapTarget
    {
        public static readonly List<KavehItem> Items = new List<KavehItem>();
        public static event Action Changed;

        public static void Add(KavehItem item)
        {
            Items.Add(item);
            if (Changed != null) Changed();
        }

        public static bool Has(string id) { return Count(id) > 0; }

        public static int Count(string id)
        {
            int n = 0;
            foreach (var i in Items) if (i.id == id) n++;
            return n;
        }

        public static bool Remove(string id)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].id == id)
                {
                    Items.RemoveAt(i);
                    if (Changed != null) Changed();
                    return true;
                }
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { Items.Clear(); Changed = null; }

        BoxCollider2D col;
        KavehBackpackPanel panel;

        public KavehBackpackPanel Panel { get { return panel; } }

        void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            panel = new GameObject("BackpackPanel").AddComponent<KavehBackpackPanel>();
        }

        void OnEnable() { KavehTapRouter.Register(this); }
        void OnDisable() { KavehTapRouter.Unregister(this); }
        void OnDestroy() { if (panel != null) Destroy(panel.gameObject); }

        public int TapPriority { get { return 10; } }

        public bool HitTest(Vector2 screenPos)
        {
            return col != null && KavehTapUtil.Hit(col.bounds, screenPos, 0.35f);
        }

        public void OnTap() { panel.Toggle(); }
    }
}
