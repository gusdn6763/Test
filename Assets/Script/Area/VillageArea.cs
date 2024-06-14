using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableArea
{
    public void MoveLocation(MoveCommand command);
    public IEnumerator MoveLocationCoroutine(MoveCommand command);
}

public class VillageArea : Area, IMoveableArea
{
    //�⺻ �ֻ��� �ൿ
    [SerializeField] private List<MultiTreeCommand> defaultCommands = new List<MultiTreeCommand>();

    //��׶��� ��� ȿ��
    [SerializeField] private BackgroundSpeechArea backgroundSpeechArea;

    [SerializeField] private MoveCommand startPosition;

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    public int stackCount = 0;
    private MoveCommand currentMoveCommand;
    private LocationList locationList;

    #region �̵� �� ��ȣ�ۿ� ����
    public MultiTreeCommand currentCommand;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    public Vector3 clickMousePosition;
    #endregion

    #region ���̾�
    [SerializeField] private LayerMask clickLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private LayerMask currentLayerMask;
    #endregion

    private void Awake()
    {
        locationList = GetComponent<LocationList>();
    }

    private void Start()
    {
        DisAbleAllCommand();

        if (startPosition)
        {
            startPosition.onMouseEvent?.Invoke(MouseStatus.Excute);
            startPosition.onAnimationEndEvent?.Invoke(MouseStatus.Excute);
        }
        else
            StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(defaultCommands));
    }

    private void Update()
    {
        if (start)
        {
            stackCount = selectCommandStack.Count;
            if (Input.GetMouseButtonDown(0))
            {
                if (currentCommand)
                    Interaction(currentCommand, MouseStatus.DownWait);
            }
            if (AnimationManager.instance.IsAnimation || BlurManager.instance.BlurStart || IsWait)
                return;
            CommandActiving();
        }
    }

    public void DisAbleAllCommand()
    {
        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        foreach (MultiTreeCommand command in commands)
        {
            command.gameObject.SetActive(false);
        }
    }

    public MultiTreeCommand FindCommand(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, currentLayerMask);

        foreach (RaycastHit hit in hits)
        {
            Interaction interaction = hit.collider.GetComponent<Interaction>();
            if (interaction)
            {
                return interaction.command;
            }
        }
        return null;
    }

    public void CommandActiving()
    {
        if (selectCommandStack.Count > 0)
            currentLayerMask = clickLayerMask;
        else
            currentLayerMask = defaultLayerMask;

        Vector3 mousePosition = Input.mousePosition;

        //���콺�� ������ �����̸� �巡�� ���� �ൿ�� ��ħ.
        if (clickCommand)
        {
            if (Input.GetMouseButton(0))
            {
                Interaction(currentCommand, MouseStatus.Drag);
            }
            else
                Interaction(currentCommand, MouseStatus.Up);
        }
        else
        {
            MultiTreeCommand command = FindCommand(mousePosition);

            if (command)
            {
                //���� �ൿ�� �ٸ����
                if (command != currentCommand)
                {
                    Interaction(command, MouseStatus.Enter);    //�ϴ� Enter�� ���� ���� ������ ���δ�.

                    if (currentCommand) //���� �ൿ�� �ٸ��� ���� ������
                        Interaction(currentCommand, MouseStatus.Exit);

                    currentCommand = command;
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                        Interaction(currentCommand, MouseStatus.Up);
                    else if (currentMouseStatus == MouseStatus.DownWait)
                        Interaction(currentCommand, MouseStatus.Down);
                    else if (Input.GetMouseButton(0))
                        Interaction(currentCommand, MouseStatus.Drag);
                }
            }
            else if (currentCommand)
            {
                if (currentMouseStatus != MouseStatus.DownWait)
                {
                    // ���콺�� � ������Ʈ�͵� �浹���� �ʾ��� ��
                    StackExit();
                    currentCommand = null;
                }
                else
                {
                    if (AnimationManager.instance.IsAnimation || BlurManager.instance.BlurStart || IsWait)
                    {

                    }
                    else
                        Interaction(currentCommand, MouseStatus.Down);
                }
            }
            else
            {
                StackExit();
            }

        }
    }
    public void StackExit()
    {
        while (selectCommandStack.Count > 0)
        {
            MultiTreeCommand command = selectCommandStack.Peek();
            Interaction(command, MouseStatus.Exit);
        }
    }

    public void ActiveOutLine(MouseStatus mouseStatus)
    {
        Sprite_OutLine[] outLines = GetComponentsInChildren<Sprite_OutLine>();

        for (int i = 0; i < outLines.Length; i++)
            outLines[i].MouseEvent(mouseStatus);
    }
    //���ñ���
    public void Interaction(MultiTreeCommand multiTreeCommand, MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        //Debug.Log(currentMouseStatus);

        if (mouseStatus == MouseStatus.Enter)
        {
            if (selectCommandStack.Count == 0)      //���ʷ� ���°��
            {
                selectCommandStack.Push(multiTreeCommand);
                multiTreeCommand.ChangeLayer(clickLayerMask);
                StartCoroutine(BlurManager.instance.BlurCoroutine(true));
            }
            else
            {
                //����) �θ�Enter, �ڽ�Enter, �θ�Exit
                MultiTreeCommand topCommand = selectCommandStack.Peek();

                if (topCommand.IsChildCommand(multiTreeCommand)) //�ڽ��� ���
                {
                    selectCommandStack.Push(multiTreeCommand);
                }
                else if (topCommand.IsSiblingCommand(multiTreeCommand)) //���� �����ΰ��    
                {
                    MultiTreeCommand siblingCommand = selectCommandStack.Pop();
                    siblingCommand.Interaction(MouseStatus.Exit);

                    selectCommandStack.Push(multiTreeCommand);
                }
            }
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.DownWait)
        {
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Down)
        {
            clickMousePosition = Input.mousePosition;
            clickCommand = multiTreeCommand;
            multiTreeCommand.Interaction(mouseStatus);
            StartCoroutine(BlurManager.instance.BlurCoroutine(false));

            if (multiTreeCommand.IsRootCommand)
                ActiveOutLine(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Drag)
        {
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Up)
        {
            clickCommand = null;
            currentCommand = null;
            multiTreeCommand.Interaction(mouseStatus);

            if (Mathf.FloorToInt(clickMousePosition.x) == Mathf.FloorToInt(Input.mousePosition.x) &&
                Mathf.FloorToInt(clickMousePosition.y) == Mathf.FloorToInt(Input.mousePosition.y) &&
                Mathf.FloorToInt(clickMousePosition.z) == Mathf.FloorToInt(Input.mousePosition.z))
                Interaction(multiTreeCommand, MouseStatus.Excute);

            if (multiTreeCommand.IsRootCommand)
                ActiveOutLine(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Excute)
        {
            multiTreeCommand.Interaction(mouseStatus);
            StackExit();
            currentCommand = null;
        }
        else if (mouseStatus == MouseStatus.Exit)
        {
            if (selectCommandStack.Count > 0)
            {
                MultiTreeCommand lastCommand = selectCommandStack.Pop();
                if (lastCommand.IsParentCommand(multiTreeCommand) || lastCommand.IsSiblingCommand(multiTreeCommand))
                {
                    selectCommandStack.Push(lastCommand);
                }
                else
                {
                    lastCommand.Interaction(mouseStatus);
                }
            }
            if (selectCommandStack.Count == 0)
            {
                StartCoroutine(BlurChangeCoroutine(multiTreeCommand));
            }
        }
    }

    IEnumerator BlurChangeCoroutine(MultiTreeCommand multiTreeCommand)
    {
        yield return StartCoroutine(BlurManager.instance.BlurCoroutine(false));
        multiTreeCommand.ChangeLayer(defaultLayerMask);
    }

    public void MoveLocation(MoveCommand command)
    {
        StartCoroutine(MoveLocationCoroutine(command));
    }

    public IEnumerator MoveLocationCoroutine(MoveCommand command)
    {
        if (currentMoveCommand == command)
            yield break;

        IsWait = true;

        if (command.alternativeLocation)
        {
            yield return MoveLocationCoroutine(command.alternativeLocation);
            IsWait = false;
            yield break;
        }
        else
        {
            if (command.IsDisable)
                command.IsCondition = false;
        }

        locationList.CaculateAllMoveCommandStatus(command);

        //���� ���� ���� ��Ȱ��ȭ
        if (currentMoveCommand)
        {
            currentMoveCommand.IsCondition = true;

            Action_Condition tmp = currentMoveCommand.GetComponent<Action_Condition>();
            if (tmp)
                tmp.CommandListOnOff(false);
        }

        //�⺻ �ֻ��� �ൿ ���� ����
        yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(defaultCommands));

        if (currentMoveCommand)
        {
            //���� ��� ���� ������Ʈ And �����۷� ��Ȱ��ȭ
            yield return StartCoroutine(DisActiveCommandCoroutine(currentMoveCommand));

            //���� ��� ��׶��� ���ȿ�� ��Ȱ��ȭ
            yield return (StartCoroutine(backgroundSpeechArea.DisableSpeech()));
        }

        //��� �̵�
        Player.instance.CurrentLocation = command.CommandName;

        //�ൿ Ȱ��ȭ
        Action_Condition tt = command.GetComponent<Action_Condition>();
        if (tt)
            tt.CommandListOnOff(true);

        //��Ÿ ������Ʈ Ȱ��ȭ
        {
            LocationCommandList locationCommandList = locationList.GetLocationCommandList(command);

            List<MultiTreeCommand> commands = new List<MultiTreeCommand>();

            //�׽�Ʈ��
            CreateItem(command);

            //��� ���� ������Ʈ ã��
            foreach (MultiTreeCommandBool multiTreeCommandBool in locationCommandList.multiTreeCommands)
            {
                multiTreeCommandBool.multiTreeCommand.IsCondition = true;
                commands.Add(multiTreeCommandBool.multiTreeCommand);
            }

            yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(commands));
        }
        //��� ��׶��� ���ȿ�� Ȱ��ȭ
        backgroundSpeechArea.SpeechStart(command);

        currentMoveCommand = command;
        IsWait = false;
    }

    private IEnumerator DisActiveCommandCoroutine(MoveCommand moveCommand)
    {
        List<MultiTreeCommand> destroyCommands = new List<MultiTreeCommand>();
        List<MultiTreeCommand> disableCommands = new List<MultiTreeCommand>();

        LocationCommandList locationCommandList = locationList.GetLocationCommandList(moveCommand);

        for (int i = 0; i < locationCommandList.multiTreeCommands.Count; i++)
        {
            MultiTreeCommandBool multiTreeCommandBool = locationCommandList.multiTreeCommands[i];

            multiTreeCommandBool.multiTreeCommand.IsCondition = false;

            disableCommands.Add(multiTreeCommandBool.multiTreeCommand);

            if (moveCommand.SaveLocation == false && multiTreeCommandBool.isDestory)
            {
                locationCommandList.multiTreeCommands.Remove(multiTreeCommandBool);
                destroyCommands.Add(multiTreeCommandBool.multiTreeCommand);
            }
        }

        yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(disableCommands));
        AnimationManager.instance.DisableCommand();

        foreach (var command in destroyCommands)
            Destroy(command.gameObject);
    }

    [SerializeField] private ItemCommand test;
    public void CreateItem(MoveCommand villageMoveCommand)
    {
        MultiTreeCommand command = Instantiate(test, transform);
        command.transform.position = FindSpawnPosition(command);
        command.gameObject.SetActive(false);

        LocationCommandList locationCommandList = locationList.GetLocationCommandList(villageMoveCommand);
        if (locationCommandList != null)
        {
            MultiTreeCommandBool multiTreeCommandBool = new MultiTreeCommandBool();
            multiTreeCommandBool.multiTreeCommand = command;
            multiTreeCommandBool.isDestory = true;
            locationCommandList.multiTreeCommands.Add(multiTreeCommandBool);
        }
    }
}