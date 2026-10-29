using System.Collections.Generic;
using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// پنل محتوای کوله‌پشتی. فقط با SpriteRenderer کشیده می‌شود (بدون uGUI)،
    /// به دوربین چسبیده و با اندازه صفحه هماهنگ می‌شود.
    /// </summary>
    public class KavehBackpackPanel : MonoBehaviour, IKavehTapTarget
    {
        const int Cols = 4, MaxSlots = 8;

        Transform root;
        SpriteRenderer panelSr;
        readonly List<SpriteRenderer> slots = new List<SpriteRenderer>();
        readonly List<SpriteRenderer> icons = new List<SpriteRenderer>();
        readonly List<Vector2> slotPos = new List<Vector2>();
        Sprite panelSprite;
        bool open;

        public bool IsOpen { get { return open; } }

        static SpriteRenderer Make(string name, Sprite s, int order, Vector2 pos, Transform parent)
        {
            var g = new GameObject(name);
            g.transform.SetParent(parent, false);
            g.transform.localPosition = pos;
            var sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = s;
            sr.sortingOrder = order;
            return sr;
        }

        void Awake()
        {
            panelSprite = Resources.Load<Sprite>("Kaveh/UI/panel");
            var slotSprite = Resources.Load<Sprite>("Kaveh/UI/slot");
            var packSprite = Resources.Load<Sprite>("Kaveh/Rig/backpack");

            root = new GameObject("PanelRoot").transform;
            root.SetParent(transform, false);
            panelSr = Make("Panel", panelSprite, 1000, Vector2.zero, root);

            if (packSprite != null)
            {
                var b = Make("PackIcon", packSprite, 1001, new Vector2(0f, 2.65f), root);
                b.transform.localScale = Vector3.one * (1.0f / packSprite.bounds.size.y);
                b.transform.localPosition = new Vector3(0f, 2.65f, 0f) - b.transform.localScale.x * packSprite.bounds.center;
            }

            for (int i = 0; i < MaxSlots; i++)
            {
                float x = -3.75f + 2.5f * (i % Cols);
                float y = i < Cols ? 0.6f : -1.9f;
                var p = new Vector2(x, y);
                slotPos.Add(p);
                slots.Add(Make("Slot" + i, slotSprite, 1001, p, root));
                icons.Add(Make("Icon" + i, null, 1002, p, root));
            }
            root.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            KavehTapRouter.Register(this);
            KavehInventory.Changed += Refresh;
            KavehTapRouter.TapMissed += OnTapMissed;
        }

        void OnDisable()
        {
            KavehTapRouter.Unregister(this);
            KavehInventory.Changed -= Refresh;
            KavehTapRouter.TapMissed -= OnTapMissed;
        }

        void OnTapMissed(Vector2 pos) { if (open) Close(); }

        public void Toggle() { if (open) Close(); else Open(); }

        public void Open()
        {
            open = true;
            root.gameObject.SetActive(true);
            Layout();
            Refresh();
        }

        public void Close()
        {
            open = false;
            root.gameObject.SetActive(false);
        }

        void Refresh()
        {
            if (!open) return;
            var items = KavehInventory.Items;
            for (int i = 0; i < icons.Count; i++)
            {
                var ic = icons[i];
                if (i < items.Count && items[i].icon != null)
                {
                    var s = items[i].icon;
                    float k = Mathf.Min(1.7f / s.bounds.size.x, 1.7f / s.bounds.size.y);
                    ic.sprite = s;
                    ic.transform.localScale = Vector3.one * k;
                    // مرکز اسپرایت را وسط خانه بگذار (پیوت‌ها فرق دارند)
                    ic.transform.localPosition = (Vector3)slotPos[i] - (Vector3)(Vector2)(s.bounds.center * k);
                    ic.enabled = true;
                }
                else ic.enabled = false;
            }
        }

        void LateUpdate()
        {
            if (open) Layout();
        }

        void Layout()
        {
            var cam = Camera.main;
            if (cam == null || panelSprite == null) return;
            if (transform.parent != cam.transform) transform.SetParent(cam.transform, false);

            float viewH = cam.orthographicSize * 2f;
            float viewW = viewH * cam.aspect;
            float k = Mathf.Min(viewW * 0.62f / panelSprite.bounds.size.x, viewH * 0.85f / panelSprite.bounds.size.y);
            root.localScale = Vector3.one * k;
            transform.localPosition = new Vector3(0f, viewH * 0.03f, 10f);
            transform.localRotation = Quaternion.identity;
        }

        // پنل باز، لمس‌های روی خودش را می‌گیرد تا کاوه حرکت نکند
        public int TapPriority { get { return 100; } }

        public bool HitTest(Vector2 screenPos)
        {
            return open && panelSr != null && KavehTapUtil.Hit(panelSr.bounds, screenPos, 0f);
        }

        public void OnTap() { }
    }
}
