using Febucci.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommandController : MonoBehaviour
{
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
        Player.instance.CurrentLocation = command.CommandName;

        //이전 지역 정보 비활성화
        if (currentMoveCommand)
        {
            currentMoveCommand.IsCondition = true;

            Action_Condition tmp = currentMoveCommand.GetComponent<Action_Condition>();
            if (tmp)
                tmp.CommandListOnOff(false);
        }

        CaculateAllMoveCommandStatus(command);

        if (command.alternativeLocation)
        {
            MoveLocation(command.alternativeLocation);
        }
        else
        {
            if (command.IsDisable)
                command.IsCondition = false;

            currentMoveCommand = command;
            GameManager.instance.currentArea.Refresh();
        }
    }

    public void CaculateAllMoveCommandStatus(VillageMoveCommand command)
    {
        List<Tuple<VillageMoveCommand, Status>> status = locationList.CaculateAllPathsStatusFromName(command.CommandName);

        for (int i = 0; i < allChildCommands.Count; i++)
        {
            VillageMoveCommand childCommand = allChildCommands[i];

            if (childCommand.Found == false)
                continue;

            foreach (var tuple in status)
            {
                VillageMoveCommand location = tuple.Item1;
                Status locationStatus = tuple.Item2;

                if (childCommand.CommandName == location.CommandName)
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
                listTmp.Add("경로 발견 : " + list[j]);
        }

        if (listTmp.Count > 0)
            Player.instance.ShowIntroduce(listTmp);
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
