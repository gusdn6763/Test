using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommand : VillageCommand
{
    protected List<CommandOnOff> commandOnOffList = new List<CommandOnOff>();
    public List<VillageMoveData> ChildLocations { get; private set; }

    public bool Found { get; set; }

    public override bool IsCondition
    {
        get { return isCondition && Found; }
        set
        {
            isCondition = value;
            if (IsFirstShow && isCondition)    //최초로 활성화가 될경우
            {
                IsFirstShow = false;
                IsInitCircleEnabled = true;
            }
        }
    }

    public bool IsDisable { get { return ChildCommands.Count == 0; } }

    private static int vertexIndex = -1;

    private int vertex = -1;
    public int Vertex
    {
        get
        {
            if (vertex == -1)
            {
                vertexIndex++;
                vertex = vertexIndex;
            }
            return vertex;
        }
    }

    protected override void ScritibleInit()
    {
        base.ScritibleInit();

        if (multiTreeData is VillageMoveData)
        {
            VillageMoveData villageMoveData = multiTreeData as VillageMoveData;

            if (villageMoveData.alternative)
                MyStatus = villageMoveData.alternative.status;

            Found = villageMoveData.found;
            ChildLocations = villageMoveData.childLocations;
        }
    }

    public override void Excute()
    {
        Player.instance.CurrentLocation = CommandName;
        CommandListOnOff(true);
        base.Excute();

        if (IsDisable)
            IsCondition = false;
    }

    public void CommandListOnOff(bool isOn)
    {
        for (int i = 0; i < commandOnOffList.Count; i++)
        {
            if (isOn)
                commandOnOffList[i].multiTreeCommand.IsCondition = commandOnOffList[i].isCheck;
            else
                commandOnOffList[i].multiTreeCommand.IsCondition = !commandOnOffList[i].isCheck;
        }
    }
}
