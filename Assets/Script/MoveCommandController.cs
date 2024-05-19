using Febucci.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommandController : MonoBehaviour
{
    private List<MoveCommand> allChildCommands = new List<MoveCommand>();
    private MoveCommand currentMoveCommand;
    private LocationList locationList;

    protected void Awake()
    {
        locationList = GetComponent<LocationList>();
        MoveCommand[] ChildCommands = GetComponentsInChildren<MoveCommand>(true);

        foreach (MoveCommand childCommand in ChildCommands)
        {
            allChildCommands.Add(childCommand);
            childCommand.onAnimationEndEvent.AddListener((status) =>
            {
                if (status == MouseStatus.Excute)
                    MoveLocation(childCommand);
            });
        }
    }

    public void MoveLocation(MoveCommand command)
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
            GameManager.instance.currentArea.Refresh(currentMoveCommand);
        }
    }

    public void CaculateAllMoveCommandStatus(MoveCommand command)
    {
        List<Tuple<MoveCommand, Status>> status = locationList.CaculateAllPathsStatusFromName(command.CommandName);

        for (int i = 0; i < allChildCommands.Count; i++)
        {
            MoveCommand childCommand = allChildCommands[i];

            if (childCommand.Found == false)
                continue;

            foreach (var tuple in status)
            {
                MoveCommand location = tuple.Item1;
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
        foreach (MoveCommand childCommand in allChildCommands)
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
