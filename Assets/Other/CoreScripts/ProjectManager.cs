using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Other.CoreScripts {
    public class ProjectManager {

        public static string CreateMetadata(Metadata metadata) {
            string json = JsonUtility.ToJson(metadata);

            string path = metadata.projectPath + AssetPaths.METADATA;
            File.WriteAllText(path, json);

            Debug.Log($"Metadata created to {path}");

            return path;
        }

        public static Metadata GetMetadata(string path) {
            if (string.IsNullOrEmpty(path)) return null;
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Metadata>(json);
        }

        public static void CreatePrefabWithMetadata(string metadataPath) {
            CreatePrefab(GetMetadata(metadataPath).prefabPath);
        }

        public static void CreatePrefab(string prefabPath) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + ".prefab");
            PrefabUtility.InstantiatePrefab(prefab);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        public static void OverridePrefab(Metadata metadata) {
            GameObject prefab = GameObject.Find(metadata.prefabName);
            PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        public static void RemovePrefab(Metadata metadata) {
            GameObject prefab = GameObject.Find(metadata.prefabName);
            Object.DestroyImmediate(prefab);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        [Serializable]
        public class Metadata {

            public string projectName;
            public string projectPath;

            public string prefabName;
            public string prefabPath;

            public string created;

            public Metadata(string prefabPath, string projectPath) {
                this.projectName = Path.GetFileName(projectPath);
                this.projectPath = projectPath;
                this.prefabName = Path.GetFileName(prefabPath);
                this.prefabPath = prefabPath;
                this.created = DateTime.Now.ToString("yyyy MMMM dd", new CultureInfo("en-US"));
            }

        }

    }
} //END