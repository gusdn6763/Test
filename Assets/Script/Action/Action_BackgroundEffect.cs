using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BackgroundEffect : Action_Command
{
    public class CommandBackgroundEffect
    {
        public MultiTreeCommand multiTreeCommand;
        public List<string> commands;
        public float perSecond;
    }
}
