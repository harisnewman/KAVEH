using System.Collections;
using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// چیزی که کاوه می‌تواند برش دارد. وقتی کاوه نزدیک شد علامت دست می‌آید؛
    /// با لمس دست، آیتم به کوله‌پشتی اضافه می‌شود. برای هر آیتم جدید، از این کلاس ارث ببرید
    /// یا فقط itemId و displayName را در Inspector بگذارید.
    /// </summary>
    public class KavehPickup : KavehInteractable
    {
        public string itemId = "item";
        public string displayName = "";
        [Tooltip("اگر خالی باشد، اسپرایت خود آبجکت آیکون می‌شود.")]
        public Sprite iconOverride;
        [Tooltip("گسترش محدوده تشخیص نزدیک بودن کاوه")]
        public Vector2 triggerPadding = new Vector2(1.2f, 0.8f);

        KavehHandPrompt hand;
        bool collected;

        void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size += triggerPadding;
            }
            hand = KavehHandPrompt.Create(transform, Collect);
        }

        protected override void OnKavehEnter() { if (!collected) hand.Show(); }
        protected override void OnKavehExit() { hand.Hide(); }

        public void Collect()
        {
            if (collected || Kaveh == null) return;
            collected = true;
            hand.Hide();
            StartCoroutine(CollectRoutine(Kaveh));
        }

        IEnumerator CollectRoutine(KavehController k)
        {
            k.FreezeFor(1.3f);
            k.Visual.FaceTowards(transform.position.x);
            k.Visual.PlayReach(0.85f);
            yield return new WaitForSeconds(0.4f);

            Vector3 start = transform.position;
            Vector3 startScale = transform.localScale;
            const float dur = 0.4f;
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, t / dur);
                transform.position = Vector3.Lerp(start, k.Visual.BackpackWorldPosition, u);
                transform.localScale = Vector3.Lerp(startScale, startScale * 0.15f, u);
                yield return null;
            }

            var sr = GetComponent<SpriteRenderer>();
            Sprite icon = iconOverride != null ? iconOverride : (sr != null ? sr.sprite : null);
            KavehInventory.Add(new KavehItem(itemId, string.IsNullOrEmpty(displayName) ? itemId : displayName, icon));
            onInteract.Invoke();
            gameObject.SetActive(false);
        }
    }
}
