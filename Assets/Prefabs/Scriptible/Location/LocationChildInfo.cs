using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationChildInfo", menuName = "Data/LocationChildInfo", order = 1)]
public class LocationChildInfo : ScriptableObject
{
    public List<LocationChildInfo> childLocations = new List<LocationChildInfo>();
    public Status defaultStatus;
    public int vertex;
}
