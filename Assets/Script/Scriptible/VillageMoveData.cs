using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VillageMoveData", menuName = "Data/VillageMoveData", order = 2)]
public class VillageMoveData : MultiTreeData
{
    [Header("찾음 여부")]
    public bool found = false;

    [Header("자식 지역들")]
    public List<VillageMoveData> childLocations = new List<VillageMoveData>();

    [Header("대체 지역 명칭")]
    public string showName;

    public int vertex = 0;
}
