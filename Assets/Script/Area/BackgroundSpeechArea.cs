using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    private List<MultiTreeCommand> poolList = new List<MultiTreeCommand>();
    private Coroutine currentCoroutine;

    [SerializeField] private int perSecond = 3;
    [SerializeField] private int maxCount = 3;
    [SerializeField] private int currentCount = 0;
    [SerializeField] private int deleteTime = 0;
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
                if (currentCount < maxCount)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, poolList.Count - 1);
                    } while (poolList[index].isActiveAndEnabled);

                    SpawnCommand(poolList[index]);
                }
                yield return new WaitForSeconds(perSecond);
            }
        }
    }
    public void SpawnCommand(MultiTreeCommand multiTreeCommand)
    {
        multiTreeCommand.transform.localPosition = FindSpawnPosition(multiTreeCommand);

        currentCount++;

        multiTreeCommand.Appearance();
        multiTreeCommand.Behavior();

        StartCoroutine(DisableCommandAfterDelay(multiTreeCommand));
    }

    private IEnumerator DisableCommandAfterDelay(MultiTreeCommand command)
    {
        yield return new WaitUntil(() => command.IsAppearanceStart == false);
        yield return new WaitForSeconds(deleteTime);

        command.DisAppearance();
        yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
        command.gameObject.SetActive(false);
        currentCount--;
    }


    public IEnumerator DisableSpeech()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        foreach (MultiTreeCommand command in poolList)
            if (command.isActiveAndEnabled)
            {
                command.StopAllCoroutines();
                command.DisAppearance();
            }

        yield return new WaitUntil(() => poolList.All(cmd => cmd.IsDisAppearanceStart == false));
        currentCount = 0;
        poolList.Clear();
    }
}
