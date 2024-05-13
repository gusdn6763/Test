using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [SerializeField] private RandomAnimation randomAnimationPrefab;

    private RandomAnimation currentAnimation;
    private List<object> animationStack = new List<object>();

    public bool IsAnimation { get { return animationStack.Count > 0; } }

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    public IEnumerator AnimationSequentialCoroutine(List<MultiTreeCommand> commandList, bool isOn)
    {
        animationStack.Add(commandList);

        if (isOn)
        {
            foreach (MultiTreeCommand command in commandList)
            {
                command.Appearance();
                yield return new WaitUntil(() => command.IsAppearanceStart == false);
            }
            animationStack.Remove(commandList);
        }
        else
        {
            foreach (MultiTreeCommand command in commandList)
            {
                command.DisAppearance();
                yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
            }
            animationStack.Remove(commandList);
            foreach (MultiTreeCommand command in commandList)
                command.gameObject.SetActive(false);
        }
    }

    public IEnumerator AnimationCoroutine(List<MultiTreeCommand> commandList, bool isOn)
    {
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
                command.gameObject.SetActive(false);
        }
    }

    public IEnumerator InitialAnimationCoroutine(List<MultiTreeCommand> commandList)
    {
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
        }

        foreach (MultiTreeCommand command in defaultList)
            command.Appearance();

        yield return new WaitUntil(() => defaultList.All(cmd => cmd.IsAppearanceStart == false));

        animationStack.Remove(commandList);
    }

    public IEnumerator IsBehaviorAnimationCoroutine(MultiTreeCommand command)
    {
        command.Behavior();

        if (command.IsLoop == false)
            animationStack.Add(command);

        yield return new WaitUntil(() => command.IsBehaviorStart == false);

        if (command.IsLoop == false)
            animationStack.Remove(command);
    }

    public IEnumerator SeparatorCoroutine(MultiTreeCommand command, bool isOn)
    {
        if (isOn)
        {
            command.Show(false);
            currentAnimation = Instantiate(randomAnimationPrefab, command.transform);
            currentAnimation.Init(command.CommandName, command.FontSize);
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

    public IEnumerator VillageSettingCoroutine(List<MultiTreeCommand> commands)
    {
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

        yield return new WaitUntil(() => IsAnimation == false);

        yield return StartCoroutine(AnimationCoroutine(defaultDisList, false));
        yield return StartCoroutine(InitialAnimationCoroutine(defaultList));
    }
}
