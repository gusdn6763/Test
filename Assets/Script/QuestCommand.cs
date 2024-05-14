using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCommand : VillageCommand
{
    public override bool IsCondition { get => base.IsCondition && Player.instance.IsAdvanture; set => base.IsCondition = value; }
}
