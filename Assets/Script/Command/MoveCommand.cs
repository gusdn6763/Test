using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : VillageCommand
{
    [Header("�Ҹ�")]
    [SerializeField] private Status defaultStatus;
    private Status currentStatus = new Status();
    public Status DefaultStatus { get => defaultStatus; set => defaultStatus = value; }
    public Status MyStatus { get => DefaultStatus; set => currentStatus = value; }

    [Header("ã�� ����")]
    [SerializeField] private bool found;
    public bool Found { get => found; set => found = value; }

    [Header("���� ����")]
    [SerializeField] private bool saveLocation;
    public bool SaveLocation { get => saveLocation;}

    [Header("�ڽ� ����")]
    [SerializeField] private List<MoveCommand> childLocations = new List<MoveCommand>();
    public List<MoveCommand> ChildLocations { get => childLocations; }

    [Header("��ü ����")]
    public MoveCommand alternativeLocation;

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
        switch(mouseStatus)
        {
            case MouseStatus.Enter:
                ShowPlayerStatus(true);
                break;
            case MouseStatus.Exit:
                ShowPlayerStatus(false);
                break;
        }
        base.Interaction(mouseStatus);
    }
    public virtual void ShowPlayerStatus(bool isOn)
    {
        if (isOn)
        {
            if(alternativeLocation)
                Player.instance.ShowPreviewStatus(currentStatus, alternativeLocation.CommandName);
            else
                Player.instance.ShowPreviewStatus(currentStatus, CommandName);
        }
        else
            Player.instance.StopPreviewStatus();
    }
}
