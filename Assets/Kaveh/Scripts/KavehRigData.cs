namespace Kaveh
{
    /// <summary>
    /// داده اسکلت کاوه (خودکار از شیت ساخته شده). مختصات به پیکسل تصویر هر قطعه است (مبدأ بالا-چپ).
    /// برای شخصیت دیگر، فقط این جدول و تصاویر Resources/Kaveh/Rig را عوض کنید.
    /// </summary>
    public static class KavehRigData
    {
        public struct Part
        {
            public string name, parent;
            public float parentJointX, parentJointY;   // نقطه اتصال روی قطعه والد
            public float ownJointX, ownJointY;         // مفصل خود قطعه (مرکز چرخش)
            public int w, h, order;
            public Part(string n, string p, float pjx, float pjy, float ojx, float ojy, int w, int h, int order)
            { name = n; parent = p; parentJointX = pjx; parentJointY = pjy; ownJointX = ojx; ownJointY = ojy; this.w = w; this.h = h; this.order = order; }
        }

        /// <summary>ارتفاع کامل ایستاده و فاصله لگن تا زمین، به پیکسل.</summary>
        public const float StandingHeightPx = 1124.8f;
        public const float HipToGroundPx = 587.3f;

        // والد باید قبل از فرزند بیاید
        public static readonly Part[] Parts = new Part[]
        {
            new Part("torso", null, 0.0f, 0.0f, 151.1f, 315.0f, 231, 350, 17),
            new Part("head", "torso", 95.9f, 8.1f, 90.9f, 230.6f, 227, 241, 18),
            new Part("thigh_far", "torso", 151.1f, 315.0f, 72.1f, 10.7f, 160, 245, 13),
            new Part("shin_far", "thigh_far", 72.1f, 234.1f, 42.7f, 18.6f, 220, 379, 14),
            new Part("thigh_near", "torso", 151.1f, 315.0f, 73.8f, 9.6f, 164, 241, 15),
            new Part("shin_near", "thigh_near", 73.8f, 230.2f, 43.6f, 19.3f, 221, 386, 16),
            new Part("uarm_far", "torso", 95.9f, 56.0f, 48.6f, 9.3f, 99, 220, 10),
            new Part("farm_far", "uarm_far", 48.6f, 209.3f, 41.6f, 19.7f, 90, 292, 11),
            new Part("uarm_near", "torso", 95.9f, 56.0f, 39.9f, 10.9f, 88, 220, 19),
            new Part("farm_near", "uarm_near", 39.9f, 208.4f, 43.7f, 19.8f, 97, 287, 20),
            new Part("backpack", "torso", 30.0f, 150.0f, 130.8f, 51.6f, 210, 234, 12),
        };
    }
}
