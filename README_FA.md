# KAVEH – مرحله ۱: خیابان خراب‌شده

## اجرا
1. Unity Hub > Add project from disk > همین پوشه (Unity 6 یا 2022.3 به بالا؛ اگر نسخه فرق دارد، Upgrade را بزنید).
2. صبر کنید پکیج‌ها نصب و اسکریپت‌ها کامپایل شوند. اگر پنجره فعال‌سازی Input System آمد، Yes بزنید.
3. صحنه `Assets/Scenes/Main.unity` خودکار ساخته می‌شود (یا منوی KAVEH > Rebuild Main Scene).
4. Play. کلید A/D یا فلش‌ها، یا ماوس/لمس نیمه چپ و راست صفحه.

## آبجکت‌های مرحله (همه مستقل و Prefab)
در Hierarchy زیر `Level1`:
- `TrashBin` : سطل زباله (x=5)
- `Flies/Fly_01..08` : هر مگس یک آبجکت جدا؛ دور سطل می‌چرخد (کامپوننت Fly)
- `Rats/Rat_01..03` : هر موش یک آبجکت جدا؛ روی زمین چپ و راست می‌رود (کامپوننت Rat)
- `BreadPiece` : تکه نان (قابل برداشتن)، و `BreadCrumbs` : خرده‌های نان
Prefabها در `Assets/Level1/Prefabs`.

## انیمیشن کاوه (برای همه مرحله‌ها)
- کاوه از قطعات جداشده ساخته می‌شود (`Assets/Resources/Kaveh/Rig` و `KavehRig.cs`). ایستادن (نفس کشیدن)، راه رفتن (هماهنگ با سرعت) و دراز کردن دست برای برداشتن.
- هر مرحله فقط کاوه را با `KavehController.Spawn` می‌سازد (Bootstrap این کار را می‌کند)؛ انیمیشن همراه اوست.
- تنظیم‌ها در `KavehConfig`: `strideCycleLength` (طول گام‌ها)، `walkSpeed`، `visualHeight`.
- اگر قطعات موجود نباشند، خودکار به اسپرایت ساده برمی‌گردد.

## پس‌زمینه و دوربین
- پس‌زمینه یک تصویر ثابت است و تکرار نمی‌شود؛ کل مرحله همان تصویر است.
- دوربین کمی زوم می‌کند (`viewFraction` در `LevelBackground`) و فقط در محدوده تصویر حرکت می‌کند.
- کاوه با دو دیوار نامرئی لبه‌های تصویر نگه داشته می‌شود.
- آبجکت‌ها (سطل، موش‌ها، نان) در مختصات دنیا ثابت‌اند و با تصویر حرکت نمی‌کنند.
- بعد از تغییر این کدها، در Unity از منو `KAVEH > Rebuild Main Scene` را بزنید.

## برداشتن و کوله‌پشتی
- نزدیک نان که شوی، علامت دست بالای آن می‌آید. روی دست بزن: کاوه دست دراز می‌کند و نان داخل کوله می‌رود.
- روی خود کاوه بزن تا محتوای کوله‌پشتی باز شود؛ دوباره بزن (یا جای دیگر) تا بسته شود.
- هر آبجکت دیگر را قابل‌برداشتن کنید: کامپوننت `KavehPickup` را اضافه کنید و `itemId` و `displayName` را بگذارید.
- از کد: `KavehInventory.Has("bread")`, `Count("bread")`, `Remove("bread")`, `KavehInventory.Changed`.
- محتوای کوله بین مرحله‌ها می‌ماند.

## اکشن دادن به آبجکت‌ها
سطل، نان و موش‌ها از `KavehInteractable` ارث می‌برند و Trigger دارند.
- بدون کد: در Inspector رویدادهای `On Kaveh Enter / On Kaveh Exit / On Interact` را به تابع دلخواه وصل کنید.
- با کد: در کلاس خود (مثلاً `TrashBin`) متدهای `OnKavehEnter`, `OnKavehExit`, `OnInteract` را override کنید.
- موش: `canMove`, `speed`, `patrolHalfWidth` قابل تغییرند. مگس: `radiusX/Y`, `angularSpeed`.

## ساختار
- `Assets/Kaveh/Scripts` : کاوه (Config, Visual, Controller, Input, Bootstrap)
- `Assets/Level1/Scripts` : Fly, Rat, TrashBin, BreadPiece, LevelBackground, KavehInteractable
- `Assets/Kaveh/Editor` : ساخت خودکار صحنه و ایمپورت تصاویر
- `Assets/Art/Props` : همه تصاویر جدا شده (۴ سطل، ۱۷ مگس، موش‌ها، نان‌ها، موارد اضافه در Extras)
- `Assets/Art/Backgrounds/level1.jpg` : پس‌زمینه مرحله

## تعویض
- کاوه: `Assets/Resources/Kaveh/kaveh_idle.png` را عوض کنید، یا KavehConfig بسازید.
- ظاهر هر آبجکت: Sprite در SpriteRenderer همان Prefab را عوض کنید.
- سطل دیگر: `bin_02..04` در `Art/Props/TrashBins`.

## بیلد اندروید
File > Build Profiles > Android > Build. جهت صفحه افقی از قبل تنظیم شده است.
