using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

public class BlockInfoGenerator : EditorWindow
{
    private Texture2D map;
    private Texture2D mapPreview;
    private BlockInfoObject blockInfo;
    private float threshold = 1f;
    private bool manualConfirmation = false;

    #region Coroutine Buttons/Vars
    private bool confirmBtn = false;
    private bool manualBtn = false;
    private bool confirm = false;
    private bool horiz = false;
    #endregion

    #region Coroutines
    EditorCoroutine c1;
    EditorCoroutine c2;
    #endregion

    [MenuItem("Tools/BlockInfo Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BlockInfoGenerator));
    }

    private void OnEnable()
    {
        manualBtn = false;
        confirmBtn = false;
        confirm = false;
        horiz = false;
        map = null;
    }

    private void OnDisable()
    {
        if(c1 != null) EditorCoroutineUtility.StopCoroutine(c1);
        if(c2 != null) EditorCoroutineUtility.StopCoroutine(c2);
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Blocking Info", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        map = EditorGUILayout.ObjectField("Add a Texture", map, typeof(Texture2D), true) as Texture2D;
        if (EditorGUI.EndChangeCheck())
        {
            MakeMapPreview(map);
        }
        blockInfo = EditorGUILayout.ObjectField("Block Info", blockInfo, typeof(BlockInfoObject), false) as BlockInfoObject;

        if(map)
        {
            threshold = EditorGUILayout.Slider("Threshold", threshold, 0f, map.width);
        }
        else
        {
            threshold = EditorGUILayout.Slider("Threshold", threshold, 0f, 5f);
        }
        
        manualConfirmation = EditorGUILayout.Toggle("Ask to Confirm?", manualConfirmation);
        

        if (mapPreview != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(25, 165, 300, 300), mapPreview);
        }
        if (GUI.Button(new Rect(125, 480, 100, 40), "Generate"))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(Generate());
        }
        if (manualBtn)
        {
            if (GUI.Button(new Rect(60, 530, 100, 30), "Horizontal"))
            {
                horiz = true;
                manualBtn = false;
            }
            if (GUI.Button(new Rect(190, 530, 100, 30), "Vertical"))
            {
                horiz = false;
                manualBtn = false;
            }
            EditorGUI.LabelField(new Rect(85, 570, 300, 30), "YELLOW = START, RED = END", EditorStyles.boldLabel);
        }
        if (confirmBtn)
        {

            if (GUI.Button(new Rect(60, 530, 100, 30), "Confirm"))
            {
                confirm = true;
                confirmBtn = false;
            }
            if (GUI.Button(new Rect(190, 530, 100, 30), "Deny"))
            {
                confirm = false;
                confirmBtn = false;
            }
            EditorGUI.LabelField(new Rect(85, 570, 300, 30), "YELLOW = START, RED = END", EditorStyles.boldLabel);
        }
    }

    private void MakeMapPreview(Texture2D texture)
    {
        mapPreview = Instantiate(texture);
        
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if(texture.GetPixel(x,y).a == 0)
                {
                    mapPreview.SetPixel(x, y, Color.white);
                    mapPreview.Apply();
                }
                
            }
        }
    }

    private IEnumerator Generate()
    {
        if (!blockInfo)
        {
            blockInfo = CreateInstance<BlockInfoObject>();
            AssetDatabase.CreateAsset(blockInfo, $"Assets/Scripts/Dynamic Block/BlockingInfo.asset");
            EditorUtility.SetDirty(blockInfo);
            AssetDatabase.SaveAssetIfDirty(blockInfo);
        }
        else
        {
            blockInfo.Clear();
        }

        

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GetPaintingLocations(x, y);
            }
        }

        foreach (var item in blockInfo.paintingLocations)
        {
            Debug.Log(item.GridLocString());
        }

        foreach (Coordinate location in blockInfo.paintingLocations)
        {
            GridLocation start = location.ToGridLocation();
            Dictionary<GridLocation, List<Coordinate>> paintingMappings = new();
            foreach (Coordinate other in blockInfo.paintingLocations)
            {
                if (other == location) continue;
                List<Coordinate> openSpots = new();
                int xDist = other.X - location.X;
                int yDist = other.Y - location.Y;
                if (Vector2.Distance(new Vector2(location.X, location.Y), new Vector2(other.X, other.Y)) <= threshold * Mathf.Sqrt(2))
                {
                    if (manualConfirmation)
                    {
                        confirmBtn = true;
                        c1 = EditorCoroutineUtility.StartCoroutineOwnerless(ManualConfirm(location, other));
                        yield return c1;
                        confirmBtn = false;
                        if (!confirm) continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                if(xDist == yDist)
                {
                    manualBtn = true;
                    c2 = EditorCoroutineUtility.StartCoroutineOwnerless(ManualChoose(location, other));
                    yield return c2;
                    if (horiz)
                    {
                        for (int i = 0; i < map.width; i++)
                        {
                            if (map.GetPixel(i, other.Y) != Color.black)
                            {
                                openSpots.Add(new Coordinate { X = i, Y = other.Y });
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < map.height; i++)
                        {
                            if (map.GetPixel(other.X, i) != Color.black)
                            {
                                openSpots.Add(new Coordinate { X = other.Y, Y = i });
                            }
                        }
                    }
                }

                if(Mathf.Abs(xDist) < Mathf.Abs(yDist))
                {
                    for (int i = 0; i < map.width; i++)
                    {
                        if(map.GetPixel(i,other.Y) != Color.black)
                        {
                            openSpots.Add(new Coordinate { X = i, Y = other.Y });
                        }
                    }
                }
                if(Mathf.Abs(xDist) > Mathf.Abs(yDist))
                {
                    for (int i = 0; i < map.height; i++)
                    {
                        if (map.GetPixel(other.X, i) != Color.black)
                        {
                            openSpots.Add(new Coordinate { X = other.Y, Y = i });
                        }
                    }
                }
                paintingMappings.Add(other.ToGridLocation(), openSpots);
            }
            blockInfo.Add(start, paintingMappings);
        }

        EditorUtility.SetDirty(blockInfo);
        AssetDatabase.SaveAssetIfDirty(blockInfo);
        blockInfo.PopulateList();
        EditorUtility.SetDirty(blockInfo);
        AssetDatabase.SaveAssetIfDirty(blockInfo);
        Debug.Log("DONE GENERATING");
    }

    IEnumerator ManualConfirm(Coordinate location, Coordinate other)
    {
        confirm = false;
        Debug.Log($"Start: {location.GridLocString()} End: {other.GridLocString()}");
        while (confirmBtn)
        {
            mapPreview.SetPixel(other.X, other.Y, Color.red);
            mapPreview.SetPixel(location.X, location.Y, Color.yellow);
            mapPreview.Apply();
            yield return null;
        }
        mapPreview.SetPixel(other.X, other.Y, Color.green);
        mapPreview.SetPixel(location.X, location.Y, Color.green);
        mapPreview.Apply();
        
        
    }
    IEnumerator ManualChoose(Coordinate location, Coordinate other)
    {
        horiz = false;
        while (manualBtn)
        {
            mapPreview.SetPixel(other.X, other.Y, Color.red);
            mapPreview.SetPixel(location.X, location.Y, Color.yellow);
            mapPreview.Apply();
            yield return null;
        }
        mapPreview.SetPixel(other.X, other.Y, Color.green);
        mapPreview.SetPixel(location.X, location.Y, Color.green);
        mapPreview.Apply();


    }

    private void GetPaintingLocations(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);
        if (pixelColor != Color.green) return;
        if(pixelColor == Color.green)
        {
            blockInfo.paintingLocations.Add(new Coordinate { X = x, Y = y });
        }
    }
}
