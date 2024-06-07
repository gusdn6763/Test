using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : UIScript
{
    [SerializeField] private IntroduceText messagePrefab;
    [SerializeField] private Transform messagePosition;

    [SerializeField] private Stat hpStat;
    [SerializeField] private Stat hungryStat;
    [SerializeField] private Stat fatigueStat;

    [SerializeField] private TextMeshProUGUI currentLocation;
    [SerializeField] private Image currentImage;
    [SerializeField] private TextMeshProUGUI currentTime;

    [SerializeField] private TextMeshProUGUI previewLocation;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI previewTime;

    private List<IntroduceText> previewList = new List<IntroduceText>();

    public void InitSetting(Status status)
    {
        currentLocation.text = Player.instance.CurrentLocation;
        currentTime.text = TimeConverter.AddTime(0);

        hpStat.Initialize(status.hp);
        hungryStat.Initialize(status.hungry);
        fatigueStat.Initialize(status.fatigue);
        ClearPreview();
    }

    public void MoveLocation(string location)
    {
        currentLocation.text = location;
    }

    public void ShowPreview(Status status, string location = "")
    {
        if (string.IsNullOrEmpty(location))
            location = Player.instance.CurrentLocation;

        previewLocation.text = location;
        previewImage.enabled = true;
        previewTime.text = "+" + TimeConverter.CalculateHourTime(status.time);

        hpStat.PreviewValue = status.hp;
        hungryStat.PreviewValue = status.hungry;
        fatigueStat.PreviewValue = status.fatigue;

        Debug.Log(status.hp + "/" + status.hungry + "/" + status.fatigue);
    }
    public void ClearPreview()
    {
        previewLocation.text = string.Empty;
        previewImage.enabled = false;
        previewTime.text = string.Empty;

        hpStat.PreviewValue = 0;
        hungryStat.PreviewValue = 0;
        fatigueStat.PreviewValue = 0;
    }

    public void ShowStatus(Status status)
    {
        hpStat.CurrentValue = status.hp;
        hungryStat.CurrentValue = status.hungry;
        fatigueStat.CurrentValue = status.fatigue;
    }

    public void CreateMessage(string message)
    {
        ChangeColor();

        IntroduceText introduceText = Instantiate(messagePrefab, messagePosition);
        introduceText.text.text = message;

        previewList.Add(introduceText);
    }
    public void CreateMessage(List<string> messages)
    {
        if (messages.Count > 0)
        {
            ChangeColor();

            foreach (string message in messages)
            {
                IntroduceText introduceText = Instantiate(messagePrefab, messagePosition);
                introduceText.text.text = message;
                previewList.Add(introduceText);
            }
        }
    }
    public void ChangeColor()
    {
        if (previewList.Count > 0)
            foreach (IntroduceText text in previewList)
                text.ColorChange();

        previewList.Clear();
    }

}
