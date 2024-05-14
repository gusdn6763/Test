using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommand : VillageCommand
{
    [Header("ã�� ����")]
    [SerializeField] private bool found;
    public bool Found { get => found; set => found = value; }

    [Header("���� ����")]
    [SerializeField] private bool saveLocation;
    public bool SaveLocation { get => saveLocation;}

    [Header("�ڽ� ����")]
    [SerializeField] private List<VillageMoveCommand> childLocations = new List<VillageMoveCommand>();
    public List<VillageMoveCommand> ChildLocations { get => childLocations; }

    [Header("��ü ����")]
    public VillageMoveCommand alternativeLocation;

    public override bool IsCondition
    {
        get { return base.IsCondition && Found; }
        set
        {
            base.IsCondition = value;
            if (IsFirstShow && base.IsCondition)    //���ʷ� Ȱ��ȭ�� �ɰ��
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
