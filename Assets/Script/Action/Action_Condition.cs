using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandOnOff
{
    public MultiTreeCommand multiTreeCommand;
    public MouseStatus mouseStatus;
    public bool isCondition;
}

public class Action_Condition : Action_Command
{
    [SerializeField] private List<CommandOnOff> commandOnOffList = new List<CommandOnOff>();

    public void CommandListOnOff(bool isOn)
    {
        for (int i = 0; i < commandOnOffList.Count; i++)
        {
            if (isOn)
                commandOnOffList[i].multiTreeCommand.IsCondition = commandOnOffList[i].isCondition;
            else
                commandOnOffList[i].multiTreeCommand.IsCondition = !commandOnOffList[i].isCondition;
        }
    }
}
