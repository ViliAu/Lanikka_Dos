using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

public class IconCreator : EditorWindow {

    [MenuItem("DUtil/Generate Icons")]
    static void StartCreating() {
        // Scan all the items from items folder
        Object[] objs = Resources.LoadAll("Prefabs");
        if (objs.Length == 0) {
            Debug.Log("No prefabs fou nd");
            return;
        }

        // Save each icon as PNG
        for (int i = 0; i < objs.Length; i++) {
            Texture2D tex = GetPreviewTexture(objs[i]);
            if (tex == null)
                return;

            tex = ClipPixels(tex);

            // Encode icon to png
            byte[] decodedPhoto;
            decodedPhoto = tex.EncodeToPNG();
            if (decodedPhoto == null)
                return;

            // Save the file as PNG
            string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\icon_";
            string fileName = objs[i].name.ToLower() + ".png";
            fileName = fileName.Replace(" ", "_");
            File.WriteAllBytes(path + fileName, decodedPhoto);
            DestroyImmediate(tex);
        }

        // Refresh project panel
        AssetDatabase.Refresh();
        ChangeToSprite(ref objs);
    }

    static void ChangeToSprite(ref Object[] objs) {
        for (int i = 0; i < objs.Length; i++) {
            string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\icon_";
            string fileName = objs[i].name.ToLower() + ".png";
            fileName = fileName.Replace(" ", "_");
            TextureImporter importer = AssetImporter.GetAtPath(path + fileName) as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }

            else {
                Debug.LogWarning("Couldn't change texture import settings");
            }
        }
    }

    static Texture2D GetPreviewTexture(Object obj) {
        Texture2D tex = null;
        int iterations = 250;

        while (tex == null && iterations > 0) {
            iterations--;
            tex = AssetPreview.GetAssetPreview(obj);
            Thread.Sleep(1);
        }

        return tex;
    }

    static Texture2D ClipPixels(Texture2D tex) {
        Color[] pixels = tex.GetPixels();
        Color clipColor = pixels[0];

        // Clip all pixels that are similiar enough to clipColor
        
        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i] == clipColor) {
                pixels[i].a = ColorPerimeter(pixels, clipColor, i, 2);
            }
        }
        tex.SetPixels(pixels);
        return tex;
    }

    static float ColorPerimeter(Color[] pixels, Color alphaColor, int i, int width) {
        int texLength = pixels.Length;
        int texSide = 128;
        // Avoid ArgumentOutOfRangeExeption
        if (i + width > texLength-1 || i - width < 0 || i + texSide * width > texLength-1 || i - texSide * width < 0 ) {
            return 0;
        }
        // Check front
        if (pixels[i+width] != alphaColor && pixels[i+width].a != 0) {
            return 1;
        }
        // Check back
        if (pixels[i-width] != alphaColor && pixels[i-width].a != 0) {
            return 1;
        }
        // Check down
        if (pixels[i+width*texSide] != alphaColor && pixels[i+width*texSide].a != 0) {
            return 1;
        }
        // Check up
        if (pixels[i-width*texSide] != alphaColor && pixels[i-width*texSide].a != 0) {
            return 1;
        }
        return 0;
    }
}