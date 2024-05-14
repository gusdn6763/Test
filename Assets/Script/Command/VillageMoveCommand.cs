using System.Collections.Generic;
using UnityEngine;

public class VillageMoveCommand : VillageCommand
{
    public VillageMoveCommand alternativeLocation;
    public List<VillageMoveData> ChildLocations { get; private set; }

    public bool Found { get; set; }

    public bool SaveLocation { get; set; }

    public override bool IsCondition
    {
        get { return isCondition && Found; }
        set
        {
            isCondition = value;
            if (IsFirstShow && isCondition)    //���ʷ� Ȱ��ȭ�� �ɰ��
            {
                IsFirstShow = false;
                IsInitCircleEnabled = true;
            }
        }
    }

    public bool IsDisable { get { return ChildCommands.Count == 0; } }

    protected override void ScritibleInit()
    {
        base.ScritibleInit();

        if (multiTreeData is VillageMoveData)
        {
            VillageMoveData villageMoveData = multiTreeData as VillageMoveData;

            Found = villageMoveData.found;
            ChildLocations = villageMoveData.childLocations;
            SaveLocation = villageMoveData.saveLocation;
        }
    }
}
