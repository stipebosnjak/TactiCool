using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MixamoManager : EditorWindow
{
    private static MixamoManager editor;
    private static int width = 350;
    private static int height = 300;
    private static int x = 0;
    private static int y = 0;

    private Avatar _avatar;
    private Object _avatarObject;

    [MenuItem("Window/Mixamo Manager")]
    static void ShowEditor()
    {
        editor = EditorWindow.GetWindow<MixamoManager>();
        CenterWindow();
    }

    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        _avatarObject = EditorGUILayout.ObjectField(_avatarObject, typeof(Object), true);
        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Rename"))
        {
            if (_avatarObject is Avatar avatar)
            {
                _avatar = avatar;                
            }
            else
            {
                return;
            }

            string path = EditorUtility.OpenFolderPanel("Path to Character Animations", "", "");

            if(string.IsNullOrWhiteSpace(path))
                return;

            var paths = DirSearch(path);

            Rename(paths);
        }
    }

    public void Rename(List<string> filePaths)
    {
        if (filePaths.Count > 0)
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                //int idx = allFiles[i].IndexOf("Assets");
                //string asset = allFiles[i].Substring(idx);
                AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(
                    filePaths[i], typeof(AnimationClip));

                var fileName = Path.GetFileNameWithoutExtension(filePaths[i]);
                var importer = (ModelImporter) AssetImporter.GetAtPath(filePaths[i]);

                RenameAndImport(importer, fileName);
            }
        }
    }

    private void RenameAndImport(ModelImporter asset, string name)
    {
        ModelImporter modelImporter = asset as ModelImporter;
        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
        modelImporter.animationType = ModelImporterAnimationType.Human;
        modelImporter.sourceAvatar = _avatar;

        
        for (int i = 0; i < clipAnimations.Length; i++)
        {

            var clip = clipAnimations[i];
            clip.loop = true;
            clip.loopPose = true;
            clip.loopTime = true;

            clip.lockRootHeightY = true;
            clip.lockRootRotation = true;

            clip.name = name;
        }

        modelImporter.clipAnimations = clipAnimations;
        modelImporter.SaveAndReimport();
    }

    private static void CenterWindow()
    {
        editor = EditorWindow.GetWindow<MixamoManager>();
        x = (Screen.currentResolution.width - width) / 2;
        y = (Screen.currentResolution.height - height) / 2;
        editor.position = new Rect(x, y, width, height);
        editor.maxSize = new Vector2(width, height);
        editor.minSize = editor.maxSize;
    }

    List<string> DirSearch(string path)
    {
        var files = Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories);
        var stringBuilder = new StringBuilder();

        var renameHistoryFilePath = Path.Combine(path, "renameHistory.txt");
        var renameHistoryFilePaths = new List<string>();
        if (File.Exists(renameHistoryFilePath))
        {
            renameHistoryFilePaths = File.ReadAllLines(renameHistoryFilePath).ToList();
        }

        var nonProcessedFiles = new List<string>();
        foreach (var file in files.Select(x=>x.Substring(x.IndexOf("Assets"))).Except(renameHistoryFilePaths))
        {
            if (file.EndsWith(".fbx"))
            {
                nonProcessedFiles.Add(file);
                stringBuilder.AppendLine(file);
            }
        }

        if (nonProcessedFiles.Any())
        {
            File.AppendAllText(renameHistoryFilePath, stringBuilder.ToString());
        }

        return nonProcessedFiles;
    }
}