﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridObjectRendererData : ScriptableObject
{
    public Sprite spriteNorm;
    public Sprite spriteOverlay;
    public Sprite[] sprites;
    public Mesh model;
    public int sizeX;
    public int sizeY;
    public int sizeZ;
	public bool isTerrainOnly;
}
