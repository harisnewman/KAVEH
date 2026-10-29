using UnityEngine;

namespace Kaveh
{
    /// <summary>یک چیز داخل کوله‌پشتی.</summary>
    [System.Serializable]
    public class KavehItem
    {
        public string id;
        public string displayName;
        public Sprite icon;

        public KavehItem(string id, string displayName, Sprite icon)
        {
            this.id = id;
            this.displayName = displayName;
            this.icon = icon;
        }
    }
}
