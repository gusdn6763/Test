using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BackgroundData
{
    public MultiTreeCommand excuteCommand;
    public List<MultiTreeCommand> sppechList = new List<MultiTreeCommand>();

    public bool isLoop;
}

public class BackgroundSpeechArea : Area
{
    [SerializeField] private List<BackgroundData> backgroundDatas;
    [SerializeField] private MultiTreeCommand dummyPrefab;

    private List<MultiTreeCommand> poolList = new List<MultiTreeCommand>();
    private Coroutine currentCoroutine;

    [Header("대사효과 출력 시간")]
    [SerializeField] private int perSecond = 3;

    [Header("최대 갯수")]
    [SerializeField] private int maxCount = 3;

    [Header("확인용 - 현재갯수")]
    [SerializeField] private int currentCount = 0;

    public void SpeechStart(MultiTreeCommand multiTreeCommand)
    {
        foreach (BackgroundData backgroundData in backgroundDatas)
        {
            if (multiTreeCommand == backgroundData.excuteCommand)
            {
                currentCoroutine = StartCoroutine(SpeechCoroutine(backgroundData));
                break;
            }
        }
    }

    public IEnumerator SpeechCoroutine(BackgroundData backgroundData)
    {
        for (int i = 0; i < backgroundData.sppechList.Count; i++)
        {
            MultiTreeCommand command = Instantiate(backgroundData.sppechList[i], transform);
            command.gameObject.SetActive(false);
            poolList.Add(command);
        }

        if (backgroundData.isLoop)
        {
            while (true)
            {
                yield return new WaitUntil(() => currentCount < maxCount);
                if (currentCount < maxCount)
                {
                    List<MultiTreeCommand> availableCommands = poolList.Where(cmd => !cmd.isActiveAndEnabled).ToList();
                    if (availableCommands.Count > 0)
                    {
                        int index = UnityEngine.Random.Range(0, availableCommands.Count);
                        SpawnCommand(availableCommands[index]);
                        DisableCommandAfterDelay(availableCommands[index]);
                    }
                    else
                    {
                        MultiTreeCommand dummy = Instantiate(dummyPrefab, transform); 
                        SpawnCommand(dummy);
                        DestoryCommandAfterDelay(dummy);
                    }
                }
                yield return new WaitForSeconds(perSecond);
            }
        }
    }
    public void SpawnCommand(MultiTreeCommand multiTreeCommand)
    {
        multiTreeCommand.transform.position = FindSpawnPosition(multiTreeCommand);

        currentCount++;

        multiTreeCommand.Appearance();
        multiTreeCommand.Behavior();
    }

    public void DisableCommandAfterDelay(MultiTreeCommand command)
    {
        StartCoroutine(DisableCommandAfterDelayCoroutine(command));
    }
    private IEnumerator DisableCommandAfterDelayCoroutine(MultiTreeCommand command)
    {
        yield return new WaitUntil(() => command.IsAppearanceStart == false);

        command.DisAppearance();
        yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
        command.gameObject.SetActive(false);
        currentCount--;
    }

    public void DestoryCommandAfterDelay(MultiTreeCommand command)
    {
        StartCoroutine(DestoryCommandAfterDelayCoroutine(command));
    }

    private IEnumerator DestoryCommandAfterDelayCoroutine(MultiTreeCommand command)
    {
        yield return new WaitUntil(() => command.IsAppearanceStart == false);

        command.DisAppearance();
        yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
        Destroy(command.gameObject);
        currentCount--;
    }

    public IEnumerator DisableSpeech()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        foreach (MultiTreeCommand command in poolList)
        {
            if (command.isActiveAndEnabled)
            {
                command.StopAllCoroutines();
                command.DisAppearance();
            }
        }

        yield return new WaitUntil(() => poolList.All(cmd => cmd.IsDisAppearanceStart == false));

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        currentCount = 0;
        poolList.Clear();
    }
}
