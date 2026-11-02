using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kaveh.EditorTools
{
    /// <summary>
    /// بار اول که پروژه باز می‌شود: حالت افقی موبایل را تنظیم می‌کند و صحنه Main را می‌سازد.
    /// </summary>
    [InitializeOnLoad]
    public static class KavehProjectSetup
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        static KavehProjectSetup()
        {
            EditorApplication.delayCall += Run;
        }

        static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            ApplyPlayerSettings();
            if (!File.Exists(ScenePath)) CreateScene();
        }

        static void ApplyPlayerSettings()
        {
            if (PlayerSettings.productName == "KAVEH") return;
            PlayerSettings.productName = "KAVEH";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
        }

        [MenuItem("KAVEH/Rebuild Main Scene")]
        static void RebuildFromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("KAVEH", "اول Play را متوقف کنید، بعد دوباره امتحان کنید.", "باشه");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            CreateScene();
        }

        [MenuItem("KAVEH/Rebuild Main Scene", true)]
        static bool RebuildFromMenuValidate()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        static void CreateScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            KavehLevel1Builder.EnsureImports();

            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();

            var go = new GameObject("KavehGame");
            go.AddComponent<KavehGameBootstrap>();

            KavehLevel1Builder.Populate();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.Refresh();
        }
    }
}
