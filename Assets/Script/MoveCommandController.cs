using Febucci.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommandController : MonoBehaviour
{
    [SerializeField] private VillageArea area;

    private List<VillageMoveCommand> allChildCommands = new List<VillageMoveCommand>();
    private VillageMoveCommand currentMoveCommand;
    private LocationList locationList;

    protected void Awake()
    {
        locationList = GetComponent<LocationList>();
        VillageMoveCommand[] ChildCommands = GetComponentsInChildren<VillageMoveCommand>(true);

        foreach (VillageMoveCommand childCommand in ChildCommands)
        {
            allChildCommands.Add(childCommand);
            childCommand.onMouseEvent += (status) =>
            {
                if (status == MouseStatus.Excute)
                    MoveLocation(childCommand);
            };
        }
    }

    public void MoveLocation(VillageMoveCommand command)
    {
        //���� ���� ���� ��Ȱ��ȭ
        if (currentMoveCommand)
        {
            currentMoveCommand.IsCondition = true;
            currentMoveCommand.CommandListOnOff(false);
        }

        List<Tuple<VillageMoveData, Status>> status = locationList.CaculateAllPathsStatusFromName(command.CommandName);

        for (int i = 0; i < allChildCommands.Count; i++)
        {
            VillageMoveCommand childCommand = allChildCommands[i];

            foreach (var tuple in status)
            {
                VillageMoveData location = tuple.Item1;
                Status locationStatus = tuple.Item2;

                if (childCommand.CommandName == location.commandName)
                {
                    childCommand.MyStatus = locationStatus;
                    break;
                }
            }
        }

        List<string> listTmp = new List<string>();
        List<string> list = locationList.SearchNearLocationFromName(Player.instance.CurrentLocation);
        for (int j = 0; j < list.Count; j++)
        {
            if (FindLocation(list[j]))
                listTmp.Add("��� �߰� : " + list[j]);
        }
        
        if (listTmp.Count > 0)
            Player.instance.ShowIntroduce(listTmp);

        currentMoveCommand = command;
        area.VillageSetting(currentMoveCommand);
    }

    public bool FindLocation(string locationName)
    {
        foreach (VillageMoveCommand childCommand in allChildCommands)
        {
            if (childCommand.CommandName == locationName)
            {
                if (childCommand.Found)
                    return false;

                childCommand.Found = true;
                return true;
            }
        }
        return false;
    }
}
