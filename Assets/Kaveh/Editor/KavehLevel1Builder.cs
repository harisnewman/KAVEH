using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kaveh.EditorTools
{
    /// <summary>
    /// مرحله ۱ را در صحنه می‌سازد: پس‌زمینه، سطل زباله، مگس‌ها، موش‌ها، نان.
    /// همه چیز آبجکت واقعی در Hierarchy و Prefab در Assets/Level1/Prefabs است.
    /// </summary>
    public static class KavehLevel1Builder
    {
        const string PropsDir = "Assets/Art/Props";
        const string PrefabDir = "Assets/Level1/Prefabs";
        const float GroundTopY = -2.2f;

        public static void EnsureImports()
        {
            AssetDatabase.Refresh();
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art", "Assets/Resources" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                float ppu; SpriteAlignment align; TextureImporterCompression comp;
                if (!KavehImportRules.TryGetRule(path, out ppu, out align, out comp)) continue;
                var ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                if (ti.textureType == TextureImporterType.Sprite && Mathf.Approximately(ti.spritePixelsPerUnit, ppu)) continue;
                KavehImportRules.Apply(ti, ppu, align, comp);
                ti.SaveAndReimport();
            }
        }

        static Sprite Load(string rel)
        {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(rel);
            if (s == null) Debug.LogWarning("KAVEH: sprite not found: " + rel);
            return s;
        }

        static GameObject MakePrefab<T>(string name, string spritePath, int order, bool collider, Action<T> init = null) where T : Component
        {
            var sprite = Load(spritePath);
            if (sprite == null) return null;

            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            var comp = go.AddComponent<T>();
            if (init != null) init(comp);
            if (collider)
            {
                var col = go.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = sprite.bounds.size;
                col.offset = sprite.bounds.center;
            }

            Directory.CreateDirectory(PrefabDir);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/" + name + ".prefab");
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject Place(GameObject prefab, Transform parent, Vector2 pos, string name = null)
        {
            if (prefab == null) return null;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (name != null) inst.name = name;
            inst.transform.SetParent(parent, false);
            inst.transform.position = pos;
            return inst;
        }

        /// <summary>محتوای مرحله ۱ را به صحنه فعلی اضافه می‌کند.</summary>
        public static void Populate()
        {
            // پس‌زمینه ثابت (بدون تکرار)؛ پایین تصویر ۱.۵ واحد زیر سطح زمین
            var bgSprite = Load("Assets/Art/Backgrounds/level1.jpg");
            var bgGo = new GameObject("Background");
            var bg = bgGo.AddComponent<LevelBackground>();
            bg.sprite = bgSprite;
            bg.worldHeight = 10f;
            bg.bottomY = GroundTopY - 1.5f;
            float bgWidth = bgSprite != null ? bg.worldHeight * bgSprite.bounds.size.x / bgSprite.bounds.size.y : 17.9f;
            float right = bgWidth * 0.5f;

            var root = new GameObject("Level1").transform;
            var flies = new GameObject("Flies").transform; flies.SetParent(root, false);
            var rats = new GameObject("Rats").transform; rats.SetParent(root, false);

            // سطل زباله (آبجکت مستقل)
            var binPrefab = MakePrefab<TrashBin>("TrashBin", PropsDir + "/TrashBins/bin_01.png", 5, true);
            var bin = Place(binPrefab, root, new Vector2(right - 1.7f, GroundTopY), "TrashBin");

            // مگس‌ها: هرکدام آبجکت مستقل، همه دور سطل
            for (int i = 1; i <= 8; i++)
            {
                var fp = MakePrefab<Fly>("Fly_" + i.ToString("00"), PropsDir + "/Flies/fly_" + i.ToString("00") + ".png", 6, false);
                var f = Place(fp, flies, new Vector2(right - 1.7f, GroundTopY + 1f));
                if (f == null) continue;
                var fly = f.GetComponent<Fly>();
                if (bin != null) fly.center = bin.transform;
                fly.offset = new Vector2(0f, 0.95f);
                fly.phase = i * 0.8f;
                fly.angularSpeed = (i % 2 == 0 ? 1f : -1f) * (2.2f + 0.35f * i);
                fly.radiusX = 0.45f + 0.08f * i;
                fly.radiusY = 0.25f + 0.04f * i;
            }

            // موش‌ها: هرکدام آبجکت مستقل و با محدوده گشت جدا
            float[] rx = { -2f, 4.8f, -4.8f };
            float[] half = { 1.8f, 1.2f, 0.9f };
            float[] spd = { 1.3f, 1.7f, 1.1f };
            for (int i = 1; i <= 3; i++)
            {
                var rp = MakePrefab<Rat>("Rat_" + i.ToString("00"), PropsDir + "/Rats/rat_" + i.ToString("00") + ".png", 5, true);
                var r = Place(rp, rats, new Vector2(rx[i - 1], GroundTopY - 0.08f));
                if (r == null) continue;
                var rat = r.GetComponent<Rat>();
                rat.patrolHalfWidth = half[i - 1];
                rat.speed = spd[i - 1];
                rat.spriteFacesRight = true;
            }

            // تکه نان و خرده‌هایش
            var breadPrefab = MakePrefab<BreadPiece>("BreadPiece", PropsDir + "/Bread/bread_slice.png", 5, true,
                b => { b.itemId = "bread"; b.displayName = "نان"; });
            Place(breadPrefab, root, new Vector2(1.4f, GroundTopY - 0.05f), "BreadPiece");
            var crumbsPrefab = MakePrefab<KavehInteractable>("BreadCrumbs", PropsDir + "/Bread/crumbs.png", 4, false);
            Place(crumbsPrefab, root, new Vector2(1.95f, GroundTopY - 0.1f), "BreadCrumbs");

            // تنظیم بوت‌استرپ: زمین نامرئی روی خیابان پس‌زمینه
#if UNITY_2023_1_OR_NEWER
            var boot = UnityEngine.Object.FindFirstObjectByType<KavehGameBootstrap>();
#else
            var boot = UnityEngine.Object.FindObjectOfType<KavehGameBootstrap>();
#endif
            if (boot != null)
            {
                boot.groundTopY = GroundTopY;
                boot.showGround = false;
                boot.spawnX = -right + 2.2f;
            }
        }
    }
}
