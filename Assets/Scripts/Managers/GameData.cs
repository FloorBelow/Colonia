using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [Header("Data sets")]
    [SubjectNerd.Utilities.Reorderable]
    public GameObject[] buildings;

    [SubjectNerd.Utilities.Reorderable]
    public ResourceData[] resources;

    //needs
    public NeedData[] needs;
    public NeedData needFood;
    public NeedData needGoods;
    public NeedData needSanitation;
    public NeedData needReligon;
    public NeedData needEntertainment;

    [Header("Art")]
    public Material modelMat;
    public Material outlineMat;
    public Color transparentColor;
    public Color transparentRedColor;
}
