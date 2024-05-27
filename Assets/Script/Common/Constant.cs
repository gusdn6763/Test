using UnityEngine;
public static class LayerMaskExtensions
{
    public static int ToSingleLayer(this LayerMask mask)
    {
        int value = mask.value;
        if (value == 0) return 0;  // Early out
        for (int l = 1; l < 32; l++)
            if ((value & (1 << l)) != 0) return l;  // Bitwise
        return -1;  // This line won't ever be reached but the compiler needs it
    }
}


[System.Serializable]
public class Constant
{
    //태그
    public const string Command = "Command";
    public const string SelectCommand = "SelectCommand";


    //아이템
    //public const string info = "정보보기";
    //public const string obser = "관찰하기";
    //public const string use = "사용하기";
    //public const string get = "줍기";
    //public const string delete = "버리기";
    //public const string full = "용량이 꽉찼다";
    //public const string buy = "구매하기";
    //public const string sell = "판매하기";

    ////장비
    //public const string equip = "장착하기";
    //public const string unequip = "해제하기";

    ////몹
    //public const string attack = "공격하기";
    //public const string looting = "루팅하기";

    ////플레이어 스탯 정보
    //public const string totalHp = "전체 체력 : ";
    //public const string hungry = "허기 : ";
    //public const string fatigue = "수먼 : ";
    //public const string health = "건강 : ";
    //public const string strength = "힘";
    //public const string intellect = "지능 : ";
    //public const string agility = "민첩 : ";

    ////퀘스트
    //public const string questDoing = "진행중";
    //public const string questAccept = "퀘스트 수락";
    //public const string notAcceptQuest = "수락하지 않은 퀘스트 입니다";

    ////위치
    //public const string room = "여관 방";
    //public const string defaultPosition = "여관 방";
    //public const string town = "광장";

    ////etc
    //public const string defaultRank = "없음";
    //public const string nothingFound = "아무것도 발견하지 못했다";
    //public const string findLocation = "의 경로를 찾았다";
    //public const string die = "당신은 죽었습니다";
    //public const string fullWeight = "용량이 꽉찼다";
    //public const string guildFirst = "길드 물품 받음";
    //public const string notRun = "아직도주할 수 없다";
}

