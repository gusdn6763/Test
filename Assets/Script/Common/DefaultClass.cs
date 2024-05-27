using JetBrains.Annotations;
using System;
using System.Collections.Generic;

[Serializable]
public class MultiTreeCommandBool
{
    public MultiTreeCommand multiTreeCommand;
    public bool isDestory;
}


    [Serializable]
public class LocationCommandList
{
    public MoveCommand moveCommand;
    public Status status;
    public List<MultiTreeCommandBool> multiTreeCommands;
}