using UnityEngine;
using UnityEngine.Events;

namespace Kaveh
{
    /// <summary>
    /// پایه همه آبجکت‌های قابل‌تعامل مرحله. وقتی کاوه وارد/خارج محدوده (Trigger) شود،
    /// رویدادها اجرا می‌شوند. در Inspector می‌توانید بدون کدنویسی اکشن به رویدادها وصل کنید،
    /// یا کلاس جدیدی بسازید و OnKavehEnter / OnKavehExit / OnInteract را override کنید.
    /// </summary>
    public class KavehInteractable : MonoBehaviour
    {
        public UnityEvent onKavehEnter;
        public UnityEvent onKavehExit;
        public UnityEvent onInteract;

        public bool KavehInside { get; private set; }
        /// <summary>کاوه‌ای که الان داخل محدوده است (یا آخرین کاوه).</summary>
        protected KavehController Kaveh { get; private set; }

        void OnTriggerEnter2D(Collider2D other)
        {
            var k = other.GetComponentInParent<KavehController>();
            if (k == null) return;
            Kaveh = k;
            KavehInside = true;
            OnKavehEnter();
            onKavehEnter.Invoke();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponentInParent<KavehController>() == null) return;
            KavehInside = false;
            OnKavehExit();
            onKavehExit.Invoke();
        }

        /// <summary>از هر جای دیگر (دکمه، اسکریپت) صدا بزنید.</summary>
        public void Interact()
        {
            OnInteract();
            onInteract.Invoke();
        }

        protected virtual void OnKavehEnter() { }
        protected virtual void OnKavehExit() { }
        protected virtual void OnInteract() { }
    }
}
