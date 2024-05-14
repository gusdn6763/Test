using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommand : VillageCommand
{
    [Header("찾음 여부")]
    [SerializeField] private bool found;
    public bool Found { get => found; set => found = value; }

    [Header("보존 여부")]
    [SerializeField] private bool saveLocation;
    public bool SaveLocation { get => saveLocation;}

    [Header("자식 지역")]
    [SerializeField] private List<VillageMoveCommand> childLocations = new List<VillageMoveCommand>();
    public List<VillageMoveCommand> ChildLocations { get => childLocations; }

    [Header("대체 지역")]
    public VillageMoveCommand alternativeLocation;

    public override bool IsCondition
    {
        get { return base.IsCondition && Found; }
        set
        {
            base.IsCondition = value;
            if (IsFirstShow && base.IsCondition)    //최초로 활성화가 될경우
            {
                IsFirstShow = false;
                IsInitCircleEnabled = true;
            }
        }
    }

    public bool IsDisable { get { return ChildCommands.Count == 0; } }

    public static int vertexCount = 0;

    private int vertex = -1;

    public int Vertex
    {
        get
        {
            if (vertex == -1)
                vertex = vertexCount++;

            return vertex;
        }
    }
}
