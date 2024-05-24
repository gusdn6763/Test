using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class VillageArea : Area
{
    [SerializeField] private List<MultiTreeCommand> defaultCommands = new List<MultiTreeCommand>();
    [SerializeField] private BackgroundSpeechArea backgroundSpeechArea;
    [SerializeField] private MoveCommand startPosition;

    //������ �����۵�
    private Dictionary<MoveCommand, MultiTreeCommand> createList = new Dictionary<MoveCommand, MultiTreeCommand>();
    private List<MoveCommand> moveCommands = new List<MoveCommand>();
    private MoveCommand currentMoveCommand;

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    private LocationList locationList;

    #region �̵� �� ��ȣ�ۿ� ����
    private MultiTreeCommand currentCommand;
    private MouseStatus currentMouseStatus;

    private MultiTreeCommand clickCommand;
    private Vector3 clickMousePosition;
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

        startPosition.onMouseEvent.Invoke(MouseStatus.Excute);
        startPosition.onAnimationEndEvent.Invoke(MouseStatus.Excute);
    }

    private void Update()
    {
        if (AnimationManager.instance.IsAnimation || BlurManager.instance.BlurStart || IsWait)
            return;

        CommandActiving();
    }

    public void DisAbleAllCommand()
    {
        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        foreach (MultiTreeCommand command in commands)
        {
            if (command.IsRootCommand)
                command.DisableAllCommandFromBottom();

            if (command is MoveCommand)
            {
                MoveCommand moveCommand = command as MoveCommand;

                moveCommands.Add(moveCommand);

                moveCommand.onAnimationEndEvent += (status) =>
                {
                    if (status == MouseStatus.Excute)
                        StartCoroutine(MoveLocationCoroutine(moveCommand));
                };
            }
        }
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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, currentLayerMask))
            {
                MultiTreeCommand command = hit.collider.GetComponent<MultiTreeCommand>();
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
                        else if (Input.GetMouseButton(0) && clickCommand == null)
                            Interaction(currentCommand, MouseStatus.Down);
                        else if (Input.GetMouseButton(0))
                            Interaction(currentCommand, MouseStatus.Drag);
                    }
                }
            }
            else if (currentCommand)
            {
                // ���콺�� � ������Ʈ�͵� �浹���� �ʾ��� ��
                StackExit();
                currentCommand = null;
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

    //���ñ���
    public void Interaction(MultiTreeCommand multiTreeCommand, MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        Debug.Log(currentMouseStatus);

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
        else if (mouseStatus == MouseStatus.Down)
        {
            clickCommand = multiTreeCommand;
            clickMousePosition = Input.mousePosition;
            multiTreeCommand.Interaction(mouseStatus);
            StartCoroutine(BlurManager.instance.BlurCoroutine(false));
        }
        else if (mouseStatus == MouseStatus.Drag)
        {
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Up)
        {
            clickCommand = null;
            multiTreeCommand.Interaction(mouseStatus);

            if (clickMousePosition == Input.mousePosition)
                Interaction(multiTreeCommand, MouseStatus.Excute);
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

    public IEnumerator MoveLocationCoroutine(MoveCommand command)
    {
        if (currentMoveCommand == command)
            yield break;

        IsWait = true;

        //�׽�Ʈ��
        CreateItem(command);

        if (command.alternativeLocation)
        {
            yield return MoveLocationCoroutine(command.alternativeLocation);
        }
        else
        {
            if (command.IsDisable)
                command.IsCondition = false;
        }

        locationList.CaculateAllMoveCommandStatus(moveCommands, command);

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
            //���� ��� ���� ������Ʈ ��Ȱ��ȭ
            yield return StartCoroutine(DisActiveCommandCoroutine(currentMoveCommand));

            //���� ��� ��׶��� ���ȿ�� ��Ȱ��ȭ
            yield return(StartCoroutine(backgroundSpeechArea.DisableSpeech()));
        }

        //��� �̵�
        Player.instance.CurrentLocation = command.CommandName;

        //��� �̵��� ������Ʈ Ȱ��ȭ
        {
            List<MultiTreeCommand> commands = new List<MultiTreeCommand>();

            foreach (var entry in createList.ToList())
                if (entry.Key == command)
                    commands.Add(entry.Value);

            yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(commands));
        }
        AnimationManager.instance.DestoryCommand();
        //��� ��׶��� ���ȿ�� Ȱ��ȭ
         backgroundSpeechArea.SpeechStart(command);

        currentMoveCommand = command;
        IsWait = false;
    }

    private IEnumerator DisActiveCommandCoroutine(MoveCommand moveCommand)
    {
        List<MultiTreeCommand> destroyCommands = new List<MultiTreeCommand>();

        foreach (var entry in createList.ToList())
        {
            if (entry.Key == moveCommand)
            {
                entry.Value.IsCondition = false;
                destroyCommands.Add(entry.Value);

                if (moveCommand.SaveLocation == false)
                    createList.Remove(moveCommand);
            }
        }

        yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(destroyCommands));

        foreach (var command in destroyCommands)
        {
            if (moveCommand.SaveLocation == false)
                Destroy(command.gameObject);
        }
    }

    [SerializeField] private ItemCommand test;
    public void CreateItem(MoveCommand villageMoveCommand)
    {
        MultiTreeCommand command = Instantiate(test, transform);
        command.transform.localPosition = FindSpawnPosition(command);
        command.gameObject.SetActive(false);
        createList.Add(villageMoveCommand, command);
    }
}