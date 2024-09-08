using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class TemplateImporter : EditorWindow {

    private static TemplateImporter _window;

    [MenuItem("Tools/Template Importer")]
    public static void ShowWindow() {
        _window = GetWindow<TemplateImporter>("Template Importer");

        var size = new Vector2(400, 200);
        _window.minSize = size;
        _window.maxSize = size;
    }

    private void OnGUI() {
        var centeredStyle = new GUIStyle(EditorStyles.boldLabel) {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20
        };

        if (PackageCheck.PackageFound) {
            GUILayout.Label("Project template already imported", centeredStyle);
        }
        else {
            CreateImport(centeredStyle);
            GUILayout.Space(20);
            GUILayout.Label("To rename project, close the editor and rename folder.", EditorStyles.helpBox);
        }
    }

    private void CreateImport(GUIStyle centeredStyle) { //TODO create package projects
        GUILayout.Label("Import Project Template", centeredStyle);
        GUILayout.Space(10);

        if (GUILayout.Button("Import HULL template")) {
            string[] foldersToImport = { "Scenes" };
            DownloadPackage("https://github.com/Thomas-Bata-University/UTW-Hull-dev-package.git",
                PackageCheck.HULL_TEMPLATE, foldersToImport);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import TURRET template")) {
            // string[] foldersToImport = { "Scenes" };
            // DownloadPackage("https://github.com/Thomas-Bata-University/UTW-Hull-dev-package.git",
            //     PackageCheck.TURRET_TEMPLATE, foldersToImport);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import WEAPONRY template")) {
            // string[] foldersToImport = { "Scenes" };
            // DownloadPackage("https://github.com/Thomas-Bata-University/UTW-Hull-dev-package.git",
            //     PackageCheck.WEAPONRY_TEMPLATE, foldersToImport);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import SUSPENSION template")) {
            // string[] foldersToImport = { "Scenes" };
            // DownloadPackage("https://github.com/Thomas-Bata-University/UTW-Hull-dev-package.git",
            //     PackageCheck.SUSPENSION_TEMPLATE, foldersToImport);
        }
    }

    #region Package

    private void DownloadPackage(string url, string package, string[] foldersToImport) {
        AddRequest request = Client.Add(url);

        while (!request.IsCompleted) {
            //Can add progress bar
        }

        if (request.Status == StatusCode.Success) {
            Debug.Log("Package successfully imported.");
            // CopyPackageFiles(package, foldersToImport);
        }
        else {
            Debug.LogError($"An error occured during importing: {request.Error.message}");
        }
    }

    /// <summary>
    /// We can copy data from Package to Assets if needed
    /// </summary>
    /// <param name="package"></param>
    /// <param name="foldersToImport"></param>
    private void CopyPackageFiles(string package, string[] foldersToImport) {
        string packageBasePath = Path.Combine("Packages", package, "Editor", "Content");

        foreach (var folder in foldersToImport) {
            string packagePath = Path.Combine(packageBasePath, folder);
            string destPath = Path.Combine(Application.dataPath, folder);

            if (Directory.Exists(packagePath)) {
                CopyDirectory(packagePath, destPath);
            }
            else {
                Debug.LogWarning($"Folder path does not exist: {packagePath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Selected folders copied successfully!");
    }

    private void CopyDirectory(string sourceDir, string destDir) {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir)) {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            string destMetaFile = destFile + ".meta";

            if (File.Exists(destFile)) {
                string existingContent = File.ReadAllText(destFile);
                string newContent = File.ReadAllText(file);

                File.WriteAllText(destFile, existingContent + "\n" + newContent);
                Debug.Log($"Data appended to existing file: {destFile}");
            }
            else {
                File.Copy(file, destFile, true);
                Debug.Log($"File copied: {destFile}");
            }

            string sourceMetaFile = file + ".meta";
            if (File.Exists(sourceMetaFile)) {
                CopyAndModifyMetaFile(sourceMetaFile, destMetaFile);
            }
        }

        foreach (var directory in Directory.GetDirectories(sourceDir)) {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(directory));
            CopyDirectory(directory, destSubDir);
        }
    }

    private void CopyAndModifyMetaFile(string sourceMetaFile, string destMetaFile) {
        File.Copy(sourceMetaFile, destMetaFile, true);
        ModifyMetaFile(destMetaFile);
    }

    private void ModifyMetaFile(string metaFilePath) {
        string content = File.ReadAllText(metaFilePath);
        string guidPattern = @"guid: (\w+)";
        Match match = Regex.Match(content, guidPattern);

        if (match.Success) {
            string oldGuid = match.Groups[1].Value;
            string newGuid = Guid.NewGuid().ToString("N");

            content = content.Replace(oldGuid, newGuid);
            File.WriteAllText(metaFilePath, content);

            Debug.Log($"Modified GUID in meta file: {metaFilePath}");
        }
    }

    #endregion

} //END