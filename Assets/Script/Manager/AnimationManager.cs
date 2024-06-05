using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RandomAnimation
{
    public RandomTextAnimation randomAnimationPrefab;
    [Range(1, 240)] public int frequencyPerFrame;

    public float duration;
    public float durationReduce;
    public float xMax;
    public float yMax;
}

[System.Serializable]
public class RandomAnimation2
{
    public RandomTextAnimation2 randomAnimationPrefab;
    public float speed;

    public float duration;
    public float durationReduce;
    public float xMax;
    public float yMax;
}


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [SerializeField] private RandomAnimation randomAnimation;
    private RandomTextAnimation currentAnimation;

    [SerializeField] private RandomAnimation2 randomAnimation2;
    private RandomTextAnimation2 currentAnimation2;

    private List<object> animationStack = new List<object>();

    public bool IsAnimation { get { return animationStack.Count > 0; } }
    private List<MultiTreeCommand> disableList = new List<MultiTreeCommand>();

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    public IEnumerator AnimationCoroutine(List<MultiTreeCommand> commandList, bool isOn)
    {
        if (animationStack.Contains(commandList))
            yield return new WaitUntil(() => animationStack.Contains(commandList) == false);

        animationStack.Add(commandList);

        if (isOn)
        {
            List<MultiTreeCommand> defaultList = new List<MultiTreeCommand>();

            foreach (MultiTreeCommand command in commandList)
                if (command.IsCondition && command.isActiveAndEnabled == false)
                    defaultList.Add(command);

            foreach (MultiTreeCommand command in defaultList)
                command.Appearance();

            foreach (MultiTreeCommand command in defaultList)
                if (command.IsCondition)
                    yield return new WaitUntil(() => command.IsAppearanceStart == false);

            animationStack.Remove(commandList);
        }
        else
        {
            List<MultiTreeCommand> defaultDisList = new List<MultiTreeCommand>();

            foreach (MultiTreeCommand command in commandList)
                if (command.isActiveAndEnabled)
                    defaultDisList.Add(command);

            foreach (MultiTreeCommand command in defaultDisList)
                command.DisAppearance();

            yield return new WaitUntil(() => defaultDisList.All(cmd => cmd.IsDisAppearanceStart == false));

            animationStack.Remove(commandList);

            foreach (MultiTreeCommand command in defaultDisList)
                disableList.Add(command);
        }
    }

    public IEnumerator InitialAnimationCoroutine(List<MultiTreeCommand> commandList)
    {
        if (animationStack.Contains(commandList))
            yield return new WaitUntil(() => animationStack.Contains(commandList) == false);

        animationStack.Add(commandList);

        List<MultiTreeCommand> firstList = new List<MultiTreeCommand>();
        List<MultiTreeCommand> defaultList = new List<MultiTreeCommand>();

        foreach (MultiTreeCommand command in commandList)
        {
            if (command.IsCondition && command.isActiveAndEnabled == false)
            {
                if (command.IsFirstAppearance)
                    firstList.Add(command);
                else
                    defaultList.Add(command);
            }
        }

        foreach (MultiTreeCommand command in firstList)
        {
            command.Appearance();
            yield return new WaitUntil(() => command.IsAppearanceStart == false);
            command.Behavior();
        }

        foreach (MultiTreeCommand command in defaultList)
            command.Appearance();

        yield return new WaitUntil(() => defaultList.All(cmd => cmd.IsAppearanceStart == false));

        foreach (MultiTreeCommand command in defaultList)
            command.Behavior();

        animationStack.Remove(commandList);
    }

    public IEnumerator IsBehaviorAnimationCoroutine(MultiTreeCommand command)
    {
        if (animationStack.Contains(command))
            yield return new WaitUntil(() => animationStack.Contains(command) == false);

        command.Behavior();

        if (command.IsLoop == false)
        {
            animationStack.Add(command);
            yield return new WaitUntil(() => command.IsBehaviorStart == false);
        }

        if (command.IsLoop == false)
            animationStack.Remove(command);
    }

    public IEnumerator SeparatorCoroutine(MultiTreeCommand command, bool isOn)
    {
        if (animationStack.Contains(command))
            yield return new WaitUntil(() => animationStack.Contains(command) == false);

        if (isOn)
        {
            command.Show(false);
            currentAnimation = Instantiate(randomAnimation.randomAnimationPrefab, command.transform);
            currentAnimation.Init(command.CommandName, command.FontSize,
                                  randomAnimation.frequencyPerFrame, randomAnimation.duration,
                                  randomAnimation.durationReduce, randomAnimation.xMax,
                                  randomAnimation.yMax);
            currentAnimation.Active(true);
        }
        else
        {
            if (currentAnimation)
            {
                animationStack.Add(currentAnimation);
                currentAnimation.Active(false);
                yield return new WaitUntil(() => currentAnimation.IsAnimation == false);
                animationStack.Remove(currentAnimation);
                Destroy(currentAnimation.gameObject);
                currentAnimation = null;
                command.Show(true);
            }
        }
        
        yield return null;
    }

    public IEnumerator SeparatorCoroutine2(MultiTreeCommand command, bool isOn)
    {
        if (animationStack.Contains(command))
            yield return new WaitUntil(() => animationStack.Contains(command) == false);

        if (isOn)
        {
            command.Show(false);
            currentAnimation2 = Instantiate(randomAnimation2.randomAnimationPrefab, command.transform);
            currentAnimation2.Init(command.CommandName, command.FontSize,
                                  randomAnimation2.speed, randomAnimation2.duration,
                                  randomAnimation2.durationReduce, randomAnimation2.xMax,
                                  randomAnimation2.yMax);
            currentAnimation2.Active(true);
        }
        else
        {
            if (currentAnimation2)
            {
                animationStack.Add(currentAnimation2);
                currentAnimation2.Active(false);
                yield return new WaitUntil(() => currentAnimation2.IsAnimation == false);
                animationStack.Remove(currentAnimation2);
                Destroy(currentAnimation2.gameObject);
                currentAnimation2 = null;
                command.Show(true);
            }
        }

        yield return null;
    }

    public IEnumerator VillageSettingCoroutine(List<MultiTreeCommand> commands)
    {
        yield return new WaitUntil(() => FadeManager.instance.IsFade == false);

        List<MultiTreeCommand> defaultList = new List<MultiTreeCommand>();
        List<MultiTreeCommand> defaultDisList = new List<MultiTreeCommand>();

        foreach (MultiTreeCommand command in commands)
        {
            if (command.IsCondition)
            {
                if (command.isActiveAndEnabled == false)
                    defaultList.Add(command);
            }
            else
            {
                if (command.isActiveAndEnabled)
                    defaultDisList.Add(command);
            }
        }

        yield return StartCoroutine(InitialAnimationCoroutine(defaultList));
        yield return StartCoroutine(AnimationCoroutine(defaultDisList, false));
    }

    public IEnumerator CommandAllDisable(MultiTreeCommand command)
    {
        if (command.ChildCommands.Count > 0)
            StartCoroutine(AnimationCoroutine(command.ChildCommands, false));

        if (command.ParentCommand)
            StartCoroutine(CommandAllDisable(command.ParentCommand));

        yield return new WaitUntil(() => IsAnimation == false);
    }

    /// <summary>
    /// 코루틴 실행중인데 비활성화 하면 코루틴이 전체적으로 멈춘다. 따라서 비활성화 부분을 외부로 뺀다.
    /// </summary>
    public void DisableCommand()
    {
        foreach (MultiTreeCommand command in disableList)
            command.gameObject.SetActive(false);

        disableList.Clear();
    }

    public void DestoryCommand()
    {
        foreach (MultiTreeCommand command in disableList)
            Destroy(command.gameObject);

        disableList.Clear();
    }
}
