using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public class LevelEditor : EditorWindow
{
    [MenuItem("Match 3/Level Editor")]
    static void Init()
    {
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();
    }
    private Color defaultcol;
    private GUIStyle UtilButton;
    private bool gate;
    private void Awake()
    {
        target = GameObject.FindObjectOfType<LevelContainer>();
        if (target == null)
        {
            GameObject g = new GameObject("Level Container", typeof(LevelContainer));
            target = g.GetComponent<LevelContainer>();
            defaultcol = GUI.color;
        }
    }
    private void firstrun()
    {
        gate = true;
        UtilButton = new GUIStyle(GUI.skin.button);
        UtilButton.alignment = TextAnchor.MiddleCenter;
        UtilButton.fontSize = 15;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.clear);
        tex.Apply();
        UtilButton.normal.background = tex;
        UtilButton.padding = new RectOffset();


    }

    private LevelContainer target;
    private bool delPref;
    public Object source;
    private int selected = -1;
    
    private void OnGUI()
    {
        if (!gate)
        {
            firstrun();
        }
        //left most add new Prefabs
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(250f), GUILayout.ExpandWidth(false));

        source = EditorGUILayout.ObjectField(source, typeof(GameObject), true, GUILayout.Width(200f));
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(250f));
        if (GUILayout.Button("New Prefab", GUILayout.Width(100f)))
        {
            if (source != null)
            {
                target.Prefabs.Add(new EditorPrefab((GameObject)source));
                source = null;
            }
            else
            {
                target.Prefabs.Add(new EditorPrefab());
            }
        }
        if (GUILayout.Button((delPref) ? "Cancel" : "Delete", GUILayout.Width(100f)))
        {
            delPref = !delPref;
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Deselect", GUILayout.Width(203f)))
        {
            selected = -1;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Selected: ");
        if (selected != -1)
        {
            if (target.Prefabs[selected] != null)
            {
                if (target.Prefabs[selected].ObjPrefab != null)
                {
                    GUILayout.Label(target.Prefabs[selected].ObjPrefab.name);
                }
                else
                {
                    GUILayout.Label("Empty Object");
                }
            }
        }
        else
        {
            GUILayout.Label("No Selection");
        }
        GUILayout.EndHorizontal();
        for (var i = 0; i < target.Prefabs.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (delPref)
            {
                if (GUILayout.Button("-", UtilButton, GUILayout.Width(20f)))
                {
                    selected = -1;

                    for (var k = 0; k < target.layers.Count; k++)
                    {
                        for (var j = 0; j < target.layers[i].Piece.Count; j++)
                        {
                            int tkp = target.layers[k].Piece[j];
                            if (tkp == i)
                            {
                                target.layers[k].Piece[j] = -1;
                            }
                            else if (tkp > i)
                            {
                                target.layers[k].Piece[j] -= 1;
                            }
                        }
                    }

                    target.Prefabs.RemoveAt(i);
                    //delPref = false;
                    break;
                }
            }
            else
            {
                if (GUILayout.Button("⦿", UtilButton, GUILayout.Width(20f)))
                {
                    selected = i;
                }
            }
            target.Prefabs[i].ObjPrefab = (GameObject)EditorGUILayout.ObjectField(target.Prefabs[i].ObjPrefab, typeof(GameObject), true, GUILayout.Width(150f));

            target.Prefabs[i].ObjColour = (Color)EditorGUILayout.ColorField(target.Prefabs[i].ObjColour, GUILayout.Width(50f));

            
            GUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Level", GUILayout.Width(100f)))
        {
            DeleteLevel();
        }
        if (GUILayout.Button("Clear Editor", GUILayout.Width(100f)))
        {
            Clear();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level Size");
        
        GUILayout.Label("");
        if (GUILayout.Button("Generate", GUILayout.Width(100f)))
        {
            Generator();
        }
        if (GUILayout.Button("Save", GUILayout.Width(100f)))
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
        }
        GUILayout.EndHorizontal();
        if (target.Prefabs.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Height:", GUILayout.Width(50f));
            target.levelheight = EditorGUILayout.IntField(target.levelheight, GUILayout.Width(75f));

            while (target.layers.Count < target.levelheight)
            {
                target.layers.Add(new levellayer());
            }
            while (target.layers.Count > target.levelheight)
            {
                target.layers.RemoveAt(target.layers.Count - 1);
            }

            GUILayout.Label("");
            GUILayout.Label("Scale: ",GUILayout.Width(50f));
            target.Size = EditorGUILayout.FloatField(target.Size, GUILayout.Width(75f));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Width:", GUILayout.Width(50f));
            target.levelwidth = EditorGUILayout.IntField(target.levelwidth, GUILayout.Width(75f));
            for (var i = 0; i < target.layers.Count; i++)
            {
                while (target.layers[i].Piece.Count < target.levelwidth)
                {
                    target.layers[i].Piece.Add(-1);
                }
                while (target.layers[i].Piece.Count > target.levelwidth)
                {
                    target.layers[i].Piece.RemoveAt(target.layers[i].Piece.Count - 1);
                }
            }
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            BeginWindows();
            Windows = GUILayout.Window(0, new Rect(250, 80, Screen.width - 300, Screen.height - 120), ScrollableWindow, "Level view");
            EndWindows();

        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    Vector2 Panner = Vector2.zero;
    Rect Windows = new Rect(20, 20, 500, 500);
    Vector2 ScrollSize = new Vector2(2000, 2000);

    void ScrollableWindow(int winID)
    {
        GUI.BeginGroup(new Rect(Panner, ScrollSize));
        GUILayout.Label("");

        for (var i = 0; i < target.layers.Count; i++)
        {
            GUILayout.BeginHorizontal();
            for (var j = 0; j < target.layers[i].Piece.Count; j++)
            {
                int tkp = target.layers[i].Piece[j];

                GUI.color = (tkp == -1) ? Color.grey : target.Prefabs[tkp].ObjColour;
                if (GUILayout.Button("" + tkp, GUILayout.Width(25f), GUILayout.Height(25f)))
                {
                    target.layers[i].Piece[j] = selected;
                }
                GUI.color = defaultcol;
            }
            GUILayout.EndHorizontal();
        }
        GUI.EndGroup();

        if (Event.current.type == EventType.MouseDrag)
        {
            Panner.x += Event.current.delta.x;
            Panner.y += Event.current.delta.y;
            Panner.x = Mathf.Clamp(Panner.x, -(target.levelwidth * 25), Screen.width-(300+target.levelwidth*10));
            Panner.y = Mathf.Clamp(Panner.y, -(target.levelheight * 25), Screen.height-(120+target.levelheight*10));
            Repaint();
        }

    }
    private void Generator()
    {
        GameObject parent = GameObject.Find("LevelArea");
        Vector2 home = Vector2.zero;
        if (parent != null)
        {
            home = parent.transform.position;
            DestroyImmediate(parent);
        }
        parent = new GameObject("LevelArea");
        parent.transform.position = new Vector2((target.levelwidth / 2f)-0.5f, -target.levelheight / 2f);

        for (var i = 0; i < target.levelheight; i++)
        {
            for (var j = 0; j < target.levelwidth; j++)
            {
                int tkp = target.layers[i].Piece[j];

                if (tkp != -1)
                {
                    if (target.Prefabs[tkp].ObjPrefab != null)
                    {
                        GameObject g = Instantiate(target.Prefabs[tkp].ObjPrefab);
                        g.transform.position = new Vector2(j, -i);
                        g.transform.parent = parent.transform;
                    }
                    else
                    {
                        GameObject g = new GameObject("Placeholder");
                        g.transform.position = new Vector2(j, -i);
                        g.transform.parent = parent.transform;
                    }
                }
            }
        }
        parent.transform.position = home;
        parent.transform.localScale = Vector3.one * target.Size;

    }
    void DeleteLevel()
    {
        DestroyImmediate(GameObject.Find("LevelArea"));
    }
    void Clear()
    {
        for (var i = 0; i < target.levelheight; i++)
        {
            for (var j = 0; j < target.levelwidth; j++)
            {
                target.layers[i].Piece[j] = -1;
            }
        }

    }
    private void OnHierarchyChange()
    {
        Awake();
        Repaint();
    }
}