using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Classes;

public class WallInfoGenerator : EditorWindow
{
    private Texture2D map;
    private Texture2D mapPreview;
    private WallInfoObject wallInfo;

    [MenuItem("Tools/Wall Info Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(WallInfoGenerator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Wall Info", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        map = EditorGUILayout.ObjectField("Add a Texture", map, typeof(Texture2D), true) as Texture2D;
        if (EditorGUI.EndChangeCheck())
        {
            MakeMapPreview(map);
        }
        wallInfo = EditorGUILayout.ObjectField("Block Info", wallInfo, typeof(WallInfoObject), false) as WallInfoObject;

        if (mapPreview != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(25, 145, 300, 300), mapPreview);
        }
        if (GUI.Button(new Rect(125, 460, 100, 40), "Generate"))
        {
            Generate();

        }
    }

    private void MakeMapPreview(Texture2D texture)
    {
        mapPreview = Instantiate(texture);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).a == 0)
                {
                    mapPreview.SetPixel(x, y, Color.white);
                    mapPreview.Apply();
                }

            }
        }
    }

    private void Generate()
    {

        if (!wallInfo)
        {
            wallInfo = CreateInstance<WallInfoObject>();
            AssetDatabase.CreateAsset(wallInfo, $"Assets/Scripts/Dynamic Block/WallInfo.asset");
            
        }
        else
        {
            wallInfo.Clear();
        }

        EditorUtility.SetDirty(wallInfo);
        AssetDatabase.SaveAssetIfDirty(wallInfo);

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                AssignWall(x, y);
            }
        }

        EditorUtility.SetDirty(wallInfo);
        AssetDatabase.SaveAssetIfDirty(wallInfo);
        Debug.Log("DONE GENERATING");
    }

    private void AssignWall(int x, int y)
    {
        if (map.GetPixel(x, y) == Color.black) return;
        int up, down, left, right;
        up = y + 1 != map.height ? y+1 : -5;
        down = y != 0 ? y - 1 : -5;
        left = x != 0 ? x-1 : -5;
        right = x + 1 != map.width ? x+1 : -5;
        Coordinate c = new Coordinate { X = x, Y = y };
        if (up != -5 && down != -5)
        {
            if (map.GetPixel(x, up) == Color.black && map.GetPixel(x, down) == Color.black)
            {
                // Vertical
                wallInfo.Add(WallDirection.Horizontal, c);
                return;
            }
        }
        else if (up == -5)
        {
            if (map.GetPixel(x, down) == Color.black)
            {
                // Vertical
                wallInfo.Add(WallDirection.Horizontal, c);
                return;
            }
        }
        else
        {
            if (map.GetPixel(x, up) == Color.black)
            {
                // Vertical
                wallInfo.Add(WallDirection.Horizontal, c);
                return;
            }
        }
        if (left != -5 && right != -5)
        {
            if (map.GetPixel(right, y) == Color.black && map.GetPixel(left, y) == Color.black)
            {
                // Horizontal
                wallInfo.Add(WallDirection.Vertical, c);
                return;
            }
        }
        else if (left == -5)
        {
            if (map.GetPixel(right, y) == Color.black)
            {
                // Horizontal
                wallInfo.Add(WallDirection.Vertical, c);
                return;
            }
        }
        else
        {
            if (map.GetPixel(left, y) == Color.black)
            {
                // Horizontal
                wallInfo.Add(WallDirection.Vertical, c);
                return;
            }
        }
    }
}