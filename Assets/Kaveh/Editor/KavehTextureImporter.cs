using UnityEditor;
using UnityEngine;

namespace Kaveh.EditorTools
{
    /// <summary>قوانین ایمپورت تصاویر پروژه (Sprite، اندازه، نقطه چرخش).</summary>
    public static class KavehImportRules
    {
        public static bool TryGetRule(string path, out float ppu, out SpriteAlignment align, out TextureImporterCompression comp)
        {
            ppu = 100f; align = SpriteAlignment.BottomCenter; comp = TextureImporterCompression.Uncompressed;
            if (path.Contains("/Art/Props/"))
            {
                ppu = 800f;
                align = path.Contains("/Flies/") ? SpriteAlignment.Center : SpriteAlignment.BottomCenter;
                return true;
            }
            if (path.Contains("/Art/Backgrounds/"))
            {
                ppu = 100f; align = SpriteAlignment.Center; comp = TextureImporterCompression.CompressedHQ;
                return true;
            }
            if (path.Contains("/Resources/Kaveh/Rig/"))
            {
                ppu = 400f; align = SpriteAlignment.Center;
                return true;
            }
            if (path.Contains("/Resources/Kaveh/UI/"))
            {
                ppu = 100f; align = SpriteAlignment.Center;
                return true;
            }
            if (path.Contains("/Art/Kaveh/") || path.Contains("/Resources/Kaveh/"))
                return true;
            return false;
        }

        public static void Apply(TextureImporter ti, float ppu, SpriteAlignment align, TextureImporterCompression comp)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = ppu;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.filterMode = FilterMode.Bilinear;
            ti.textureCompression = comp;
            ti.maxTextureSize = 4096;

            var s = new TextureImporterSettings();
            ti.ReadTextureSettings(s);
            s.spriteAlignment = (int)align;
            ti.SetTextureSettings(s);
        }
    }

    /// <summary>تصاویر جدید را خودکار با قوانین بالا ایمپورت می‌کند.</summary>
    public class KavehTextureImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            float ppu; SpriteAlignment align; TextureImporterCompression comp;
            if (!KavehImportRules.TryGetRule(assetPath, out ppu, out align, out comp)) return;
            KavehImportRules.Apply((TextureImporter)assetImporter, ppu, align, comp);
        }
    }
}
