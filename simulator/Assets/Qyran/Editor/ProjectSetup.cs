using System;
using System.IO;
using Qyran.Flight;
using Qyran.Input;
using Qyran.UI;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Qyran.Editor
{
    public static class ProjectSetup
    {
        public const string ScenePath = "Assets/Qyran/Scenes/TrainingYard.unity";

        [MenuItem("Qyran/Prepare training scene")]
        public static void Prepare()
        {
            Directory.CreateDirectory("Assets/Qyran/Scenes");
            Directory.CreateDirectory("Assets/Qyran/Prefabs");
            Directory.CreateDirectory("Assets/Qyran/UI");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PlayerSettings.companyName = "Qyran";
            PlayerSettings.productName = "Qyran";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultScreenWidth = 1440;
            PlayerSettings.defaultScreenHeight = 810;
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetApiCompatibilityLevel(UnityEditor.Build.NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            settings.FindProperty("activeInputHandler").intValue = 1;
            settings.ApplyModifiedPropertiesWithoutUndo();
            EditorSettings.serializationMode = SerializationMode.ForceText;
            UnityEditor.VersionControlSettings.mode = "Visible Meta Files";
            QualitySettings.vSyncCount = 1;
            QualitySettings.antiAliasing = 4;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadowDistance = 100;
            Time.fixedDeltaTime = 0.02f;
            Time.maximumDeltaTime = 0.1f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.68f, 0.72f, 0.70f);
            RenderSettings.ambientEquatorColor = new Color(0.57f, 0.59f, 0.52f);
            RenderSettings.ambientGroundColor = new Color(0.3f, 0.32f, 0.26f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.69f, 0.73f, 0.70f);
            RenderSettings.fogDensity = 0.0028f;
            var sunObject = new GameObject("Afternoon sun");
            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.1f;
            sun.color = new Color(1, 0.95f, 0.84f);
            sun.shadows = LightShadows.Soft;
            sun.shadowBias = 0.03f;
            sunObject.transform.rotation = Quaternion.Euler(38, -32, 0);
            RenderSettings.sun = sun;
            string skyPath = "Assets/Qyran/Art/Materials/Sky.mat";
            Directory.CreateDirectory("Assets/Qyran/Art/Materials");
            var sky = AssetDatabase.LoadAssetAtPath<Material>(skyPath);
            if (sky == null)
            {
                sky = new Material(Shader.Find("Skybox/Procedural"));
                AssetDatabase.CreateAsset(sky, skyPath);
            }
            sky.SetColor("_SkyTint", new Color(0.62f, 0.67f, 0.68f));
            sky.SetColor("_GroundColor", new Color(0.39f, 0.42f, 0.32f));
            sky.SetFloat("_AtmosphereThickness", 1.05f);
            sky.SetFloat("_SunSize", 0.025f);
            sky.SetFloat("_Exposure", 1);
            RenderSettings.skybox = sky;
            var hoverMarker = TrainingYardBuilder.Build();
            var drone = QuadrotorBuilder.Build();
            drone.transform.position = new Vector3(0, 0.22f, 0);
            PrefabUtility.SaveAsPrefabAsset(drone, "Assets/Qyran/Prefabs/Quadrotor.prefab");
            var input = new GameObject("RC input").AddComponent<RcInput>();
            var cameraObject = new GameObject("Flight camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.025f;
            camera.farClipPlane = 650;
            camera.fieldOfView = 68;
            camera.allowHDR = true;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.position = new Vector3(0, 1.72f, -3.5f);
            cameraObject.transform.LookAt(new Vector3(0, 0.37f, 1.7f));
            var follow = cameraObject.AddComponent<FollowCamera>();
            follow.target = drone.transform;
            var session = drone.GetComponent<FlightSession>();
            session.input = input;
            session.flightCamera = follow;
            session.hoverMarker = hoverMarker;
            var uiObject = new GameObject("Flight interface");
            var document = uiObject.AddComponent<UIDocument>();
            const string panelPath = "Assets/Qyran/UI/FlightPanel.asset";
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelPath);
            if (panel == null)
            {
                panel = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panel, panelPath);
            }
            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panel.referenceResolution = new Vector2Int(1600, 900);
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panel.match = 0.5f;
            panel.themeStyleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>("Assets/Qyran/UI/FlightTheme.tss");
            document.panelSettings = panel;
            document.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Qyran/UI/Flight.uxml");
            var hud = uiObject.AddComponent<FlightHud>();
            hud.input = input;
            hud.session = session;
            hud.flightCamera = follow;
            var controller = uiObject.AddComponent<ControllerPanel>();
            controller.input = input;
            controller.session = session;
            EditorUtility.SetDirty(panel);
            EditorUtility.SetDirty(sky);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log("Qyran training scene ready.");
        }

        [MenuItem("Qyran/Build Windows")]
        public static void BuildWindows()
        {
            if (!File.Exists(ScenePath)) Prepare();
            string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Builds/Windows/Qyran.exe"));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Windows build failed: " + report.summary.result);
            Debug.Log("Built Qyran: " + output);
        }
    }
}
