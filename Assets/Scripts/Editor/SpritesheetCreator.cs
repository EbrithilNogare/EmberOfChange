using UnityEngine;
using UnityEditor;
using System.IO;

public class SpritesheetCreator : EditorWindow
{
    private Texture2D[] loadedTextures;
    private string folderPath;
    private int rows = 1;
    private int cols = 1;

    [MenuItem("Tools/Spritesheet Creator")]
    public static void ShowWindow() => GetWindow<SpritesheetCreator>("Spritesheet Creator");

    private void OnEnable()
    {
        folderPath = @"C:\Users\WheeroWeyi\Desktop\Active projects\EmberOfChange\Source\Animation Export";
        LoadTexturesFromFolder(folderPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Spritesheet Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Folder"))
        {
            folderPath = EditorUtility.OpenFolderPanel("Select Folder with PNGs", folderPath, "");
            if (!string.IsNullOrEmpty(folderPath))
                LoadTexturesFromFolder(folderPath);
        }

        if (!string.IsNullOrEmpty(folderPath))
            GUILayout.Label($"Selected Folder: {folderPath}");

        if (loadedTextures != null)
            GUILayout.Label($"Loaded Textures: {loadedTextures.Length}");

        rows = EditorGUILayout.IntField("Rows", rows);
        cols = EditorGUILayout.IntField("Columns", cols);

        if (GUILayout.Button("Generate Spritesheet"))
        {
            if (loadedTextures != null && loadedTextures.Length > 0)
                GenerateSpritesheet();
            else
                Debug.LogError("No textures loaded!");
        }
    }

    private void LoadTexturesFromFolder(string path)
    {
        string[] files = Directory.GetFiles(path, "*.png");
        loadedTextures = new Texture2D[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            byte[] fileData = File.ReadAllBytes(files[i]);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
                loadedTextures[i] = tex;
            else
                Debug.LogError($"Failed to load texture from {files[i]}");
        }
        Debug.Log($"Loaded {loadedTextures.Length} textures from {path}");
    }

    private void GenerateSpritesheet()
    {
        if (loadedTextures == null || loadedTextures.Length == 0)
        {
            Debug.LogError("No textures to generate spritesheet!");
            return;
        }

        int spriteWidth = loadedTextures[0].width;
        int spriteHeight = loadedTextures[0].height;

        Texture2D spritesheet = new Texture2D(spriteWidth * cols, spriteHeight * rows);

        for (int i = 0; i < loadedTextures.Length; i++)
        {
            Texture2D tex = loadedTextures[i];
            int x = (i % cols) * spriteWidth;
            int y = ((rows - 1) - (i / cols)) * spriteHeight;

            spritesheet.SetPixels(x, y, spriteWidth, spriteHeight, tex.GetPixels());
        }

        spritesheet.Apply();

        string savePath = EditorUtility.SaveFilePanel("Save Spritesheet", "", "spritesheet.png", "png");
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllBytes(savePath, spritesheet.EncodeToPNG());
            Debug.Log("Spritesheet saved to " + savePath);
        }
    }
}
