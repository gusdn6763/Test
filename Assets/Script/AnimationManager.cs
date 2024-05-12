using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public List<object> animationStack = new List<object>();
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
        }
        else
        {
            foreach (MultiTreeCommand command in commandList)
            {
                command.Appearance();
                yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
            }
        }
        animationStack.Remove(commandList);
    }

    public void Animation(List<MultiTreeCommand> commandList, bool isOn)
    {
        StartCoroutine(AnimationCoroutine(commandList, isOn));
    }
    IEnumerator AnimationCoroutine(List<MultiTreeCommand> commandList, bool isOn)
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
        }
        else
        {
            List<MultiTreeCommand> defaultDisList = new List<MultiTreeCommand>();

            foreach (MultiTreeCommand command in commandList)
                if (command.isActiveAndEnabled)
                    defaultDisList.Add(command);

            foreach (MultiTreeCommand command in defaultDisList)
                command.DisAppearance();
            foreach (MultiTreeCommand command in defaultDisList)
                yield return new WaitUntil(() => command.IsDisAppearanceStart == false);
        }

        animationStack.Remove(commandList);
    }

    public void InitialAnimation(List<MultiTreeCommand> commandList, bool isWait = false)
    {
        StartCoroutine(InitialAnimationCoroutine(commandList, isWait));
    }
    IEnumerator InitialAnimationCoroutine(List<MultiTreeCommand> commandList, bool isWait)
    {
        if (isWait)
            yield return new WaitUntil(() => animationStack.Count == 0);

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

        foreach (MultiTreeCommand command in defaultList)
            yield return new WaitUntil(() => command.IsAppearanceStart == false);

        animationStack.Remove(commandList);
    }

    public void IsBehaviorAnimation(MultiTreeCommand command)
    {
        StartCoroutine(IsBehaviorAnimationCoroutine(command));
    }

    IEnumerator IsBehaviorAnimationCoroutine(MultiTreeCommand command)
    {
        animationStack.Add(command);
        command.Behavior();
        yield return new WaitUntil(() => command.IsBehaviorStart == false);

        animationStack.Remove(command);
    }
}
