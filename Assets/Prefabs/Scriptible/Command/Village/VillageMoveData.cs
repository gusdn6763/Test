using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VillageMoveData", menuName = "Data/VillageMoveData", order = 2)]
public class VillageMoveData : MultiTreeData
{
    [Header("ã�� ����")]
    public bool found = false;

    [Header("�ڽ� ������")]
    public List<VillageMoveData> childLocations = new List<VillageMoveData>();

    [Header("���� ����")]
    public bool saveLocation;

    public int vertex = 0;
}
