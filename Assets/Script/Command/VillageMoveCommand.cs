using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VillageMoveCommand : VillageCommand
{
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

            //if (string.IsNullOrEmpty(villageMoveData.showName))
                //text.text = villageMoveData.showName;

            Found = villageMoveData.found;
            ChildLocations = villageMoveData.childLocations;
        }
    }
}
