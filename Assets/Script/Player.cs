using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("플레이어UI")]
    [SerializeField] private PlayerUi playerUi;

    [Header("플레이어")]
    [SerializeField] private float hp;
    [SerializeField] private float fatigue;
    [SerializeField] private float hungry;
    [SerializeField] private int money;
    [SerializeField] private string currentLocation;

    public float Hp { get { return hp; }  set  { hp = value; } }
    public float Hungry { get { return hungry; } set { hungry = value; } }
    public float Fatigue { get { return fatigue; } set { fatigue = value; } }
    public int Money { get { return money; } set { money = value; } }
    public string CurrentLocation{ get { return currentLocation; } set { currentLocation = value; playerUi.MoveLocation(currentLocation); } }

    public bool IsAdvanture { get; set; } = false;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private void Start()
    {
        playerUi.InitSetting(new Status(Hp, Hungry, Fatigue));
    }

    //UI 및 상태바 부분
    public void ShowPreviewStatus(Status status, string location = "")
    {
        playerUi.ShowPreview(new Status(status.hp, status.fatigue, status.hungry, status.time), location);
    }
    public void StopPreviewStatus()
    {
        playerUi.ClearPreview();
    }
    public void SetStatus(Status status, bool isDungeon = false)
    {
        Hp += status.hp;
        Fatigue += status.fatigue;
        Hungry += status.hungry;

        playerUi.ShowStatus(new Status(Hp, Fatigue, Hungry, status.time));
    }

    public void ShowIntroduce(string introduce)
    {
        playerUi.CreateMessage(introduce);
    }

    public void ShowIntroduce(List<string> introduce)
    {
        playerUi.CreateMessage(introduce);
    }
}
