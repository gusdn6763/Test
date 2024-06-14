using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : VillageCommand
{
    [Header("���� �Ҹ�")]
    public Status currentStatus;
    public Status MyStatus { get => currentStatus; set => currentStatus = value; }

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

    protected override void Awake()
    {
        base.Awake();

        if (CurrentArea is IMoveableArea)
        {
            onAnimationEndEvent += (status) =>
            {
                if (status == MouseStatus.Excute)
                    (CurrentArea as IMoveableArea).MoveLocation(this);
            };
        }
    }

    public override void Interaction(MouseStatus mouseStatus)
    {
        base.Interaction(mouseStatus);

        if (mouseStatus == MouseStatus.Enter)
        {
            if (alternativeLocation)
                Player.instance.ShowPreviewStatus(alternativeLocation.currentStatus, alternativeLocation.CommandName);
            else
                Player.instance.ShowPreviewStatus(currentStatus, CommandName);
        }
        else if (mouseStatus == MouseStatus.Exit)
        {
            Player.instance.StopPreviewStatus();
        }
        else if (mouseStatus == MouseStatus.Excute)
        {
            if (alternativeLocation)
                Player.instance.SetStatus(alternativeLocation.MyStatus);
            else
                Player.instance.SetStatus(MyStatus);
        }
    }
}
