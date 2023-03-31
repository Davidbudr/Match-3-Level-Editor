using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [HideInInspector]
    public int levelwidth;
    [HideInInspector]
    public int levelheight;
    [HideInInspector]
    public float Size = 1f;
    public List<levellayer> layers = new List<levellayer>();
    public List<EditorPrefab> Prefabs = new List<EditorPrefab>();
    
}

[Serializable]
public class EditorPrefab
{
    [SerializeField]
    public GameObject ObjPrefab;
    public Color ObjColour;

    public EditorPrefab()
    {
        ObjColour = new Color(0, 0, 0, 1);
    }
    public EditorPrefab(GameObject g)
    {
        ObjPrefab = g;
        ObjColour = new Color(0, 0, 0, 1);
    }
}
[Serializable]
public class levellayer
{
    [SerializeField]
    public List<int> Piece = new List<int>();
}


