using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kaveh
{
    /// <summary>
    /// ورودی: لمس نیمه چپ صفحه = چپ، نیمه راست = راست (کیبورد و ماوس هم برای تست).
    /// اگر لمس روی یک هدف تعاملی (دست، کاوه، کوله) شروع شود، تا رها شدن، حرکت نمی‌دهد.
    /// </summary>
    public static class KavehInput
    {
        static bool suppress;

        public static void SuppressPointerMovementUntilRelease() { suppress = true; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { suppress = false; }

        public static bool AnyPointerHeld()
        {
#if ENABLE_INPUT_SYSTEM
            var ts = Touchscreen.current;
            if (ts != null)
                foreach (var t in ts.touches) if (t.press.isPressed) return true;
            var m = Mouse.current;
            return m != null && m.leftButton.isPressed;
#else
            return Input.touchCount > 0 || Input.GetMouseButton(0);
#endif
        }

        /// <summary>در این فریم یک لمس/کلیک جدید شروع شده؟</summary>
        public static bool PointerPressedThisFrame(out Vector2 pos)
        {
            pos = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            var ts = Touchscreen.current;
            if (ts != null)
                foreach (var t in ts.touches)
                    if (t.press.wasPressedThisFrame) { pos = t.position.ReadValue(); return true; }
            var m = Mouse.current;
            if (m != null && m.leftButton.wasPressedThisFrame) { pos = m.position.ReadValue(); return true; }
            return false;
#else
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began) { pos = t.position; return true; }
            }
            if (Input.touchCount == 0 && Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
            return false;
#endif
        }

        public static float Horizontal()
        {
            if (!AnyPointerHeld()) suppress = false;
            float h = 0f;
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            }
            if (!suppress)
            {
                bool anyTouch = false;
                var ts = Touchscreen.current;
                if (ts != null)
                    foreach (var t in ts.touches)
                        if (t.press.isPressed)
                        {
                            anyTouch = true;
                            h += t.position.ReadValue().x < Screen.width * 0.5f ? -1f : 1f;
                        }
                var m = Mouse.current;
                if (!anyTouch && m != null && m.leftButton.isPressed)
                    h += m.position.ReadValue().x < Screen.width * 0.5f ? -1f : 1f;
            }
#else
            h = Input.GetAxisRaw("Horizontal");
            if (!suppress)
            {
                for (int i = 0; i < Input.touchCount; i++)
                    h += Input.GetTouch(i).position.x < Screen.width * 0.5f ? -1f : 1f;
                if (Input.touchCount == 0 && Input.GetMouseButton(0))
                    h += Input.mousePosition.x < Screen.width * 0.5f ? -1f : 1f;
            }
#endif
            return Mathf.Clamp(h, -1f, 1f);
        }
    }
}
