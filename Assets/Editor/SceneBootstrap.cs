using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using PerturbationRestoration.Visualization;

namespace PerturbationRestoration.EditorTools
{
    public static class SceneBootstrap
    {
        public static void CreateSampleScene()
        {
            string scenesFolder = "Assets/Scenes";

            if (!AssetDatabase.IsValidFolder(scenesFolder))
                AssetDatabase.CreateFolder("Assets", "Scenes");

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects,
                NewSceneMode.Single
            );

            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.transform.position = new Vector3(0f, 0f, -12f);
                camera.transform.rotation = Quaternion.identity;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.08f, 0.08f, 0.10f);
            }

            Light light = Object.FindFirstObjectByType<Light>();
            if (light != null)
            {
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                light.intensity = 1.2f;
            }

            GameObject graphObject = new GameObject("GraphVisualizer");
            graphObject.AddComponent<GraphVisualizer>();

            string scenePath = Path.Combine(scenesFolder, "SampleScene.unity");
            EditorSceneManager.SaveScene(scene, scenePath);

            Debug.Log($"Created sample scene at {scenePath}");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
