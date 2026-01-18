using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship
{
    public string ShipName;
    public int ShipSize;
    public List<Vector2Int> positions = new List<Vector2Int>();
    public bool isHorizontal;
    public bool IsSunkAnnounced = false;

}
