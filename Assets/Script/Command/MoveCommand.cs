using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : VillageCommand
{
    [Header("현재 소모값")]
    public Status currentStatus;
    public Status MyStatus { get => currentStatus; set => currentStatus = value; }

    [Header("찾음 여부")]
    [SerializeField] private bool found;
    public bool Found { get => found; set => found = value; }

    [Header("보존 여부")]
    [SerializeField] private bool saveLocation;
    public bool SaveLocation { get => saveLocation;}

    [Header("자식 지역")]
    [SerializeField] private List<MoveCommand> childLocations = new List<MoveCommand>();
    public List<MoveCommand> ChildLocations { get => childLocations; }

    [Header("대체 지역")]
    public MoveCommand alternativeLocation;

    public string command2;

    public override bool IsCondition
    {
        get { return base.IsCondition && Found; }
        set { base.IsCondition = value; }
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

    public override void Interaction(MouseStatus mouseStatus)
    {
        base.Interaction(mouseStatus);

        if (mouseStatus == MouseStatus.Excute)
        {
            if (alternativeLocation)
                Player.instance.SetStatus(alternativeLocation.MyStatus);
            else
                Player.instance.SetStatus(MyStatus);
        }
    }

    private void OnMouseEnter()
    {
        if (alternativeLocation)
            Player.instance.ShowPreviewStatus(alternativeLocation.currentStatus, alternativeLocation.CommandName);
        else
            Player.instance.ShowPreviewStatus(currentStatus, CommandName);
    }

    private void OnMouseExit()
    {
        Player.instance.StopPreviewStatus();
    }
}
