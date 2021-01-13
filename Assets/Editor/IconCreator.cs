using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

public class IconCreator : EditorWindow {

    [MenuItem("DUtil/Generate Icons")]
    static void StartCreating() {
        // Scan all the items from items folder
        Entity[] objs = Resources.LoadAll<Entity>("Prefabs");
        if (objs.Length == 0) {
            Debug.Log("No prefabs fou nd");
            return;
        }

        // Save each icon as PNG
        for (int i = 0; i < objs.Length; i++) {
            Texture2D tex = GetPreviewTexture(objs[i].gameObject);
            if (tex == null)
                return;

            tex = ClipPixels(tex);

            // Encode icon to png
            byte[] decodedPhoto;
            decodedPhoto = tex.EncodeToPNG();
            if (decodedPhoto == null)
                return;

            // Save the file as PNG
            string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\";
            string fileName = objs[i].entityName + ".png";
            File.WriteAllBytes(path + fileName, decodedPhoto);
        }

        // Refresh project panel
        AssetDatabase.Refresh();
        ChangeToSprite(ref objs);
    }

    static void ChangeToSprite(ref Entity[] objs) {
        for (int i = 0; i < objs.Length; i++) {
            string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\";
            string fileName = objs[i].entityName + ".png";
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
        Color clipColor = new Color(0, 0, 0, 1);
        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i].a == 1 ) {
                clipColor = pixels[i];
                break;
            }
        }
        // Clip all pixels that are similiar enough to clipColor
        
        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i] == clipColor) {
                pixels[i].a = 0;
                pixels[i].a += ColorPerimeter(pixels, clipColor, i, 3);
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
        for (int j = 0; j < width; j++) {
            if (pixels[i+j] != alphaColor && pixels[i+j].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        
        // Check back
        for (int j = 0; j < width; j++) {
            if (pixels[i-j] != alphaColor && pixels[i-j].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        // Check up
        for (int j = 0; j < width; j++) {
            if (pixels[i+j*texSide] != alphaColor && pixels[i+j*texSide].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        } 
        // Check down
        for (int j = 0; j < width; j++) {
            if (pixels[i-j*texSide] != alphaColor && pixels[i-j*texSide].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        return 0;
    }
}