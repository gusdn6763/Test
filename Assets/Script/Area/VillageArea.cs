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
    //기본 최상위 행동
    [SerializeField] private List<MultiTreeCommand> defaultCommands = new List<MultiTreeCommand>();

    //백그라운드 대사 효과
    [SerializeField] private BackgroundSpeechArea backgroundSpeechArea;

    [SerializeField] private MoveCommand startPosition;

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    public int stackCount = 0;
    private MoveCommand currentMoveCommand;
    private LocationList locationList;

    #region 이동 및 상호작용 관련
    public MultiTreeCommand currentCommand;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    public Vector3 clickMousePosition;
    #endregion

    #region 레이어
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

        //마우스를 빠르게 움직이면 드래그 중인 행동을 놓침.
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
                //이전 행동과 다를경우
                if (command != currentCommand)
                {
                    Interaction(command, MouseStatus.Enter);    //일단 Enter를 먼저 들어가야 스택이 쌓인다.

                    if (currentCommand) //이전 행동과 다르고 값이 있으면
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
                    // 마우스가 어떤 오브젝트와도 충돌하지 않았을 때
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
    //스택구조
    public void Interaction(MultiTreeCommand multiTreeCommand, MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        //Debug.Log(currentMouseStatus);

        if (mouseStatus == MouseStatus.Enter)
        {
            if (selectCommandStack.Count == 0)      //최초로 들어온경우
            {
                selectCommandStack.Push(multiTreeCommand);
                multiTreeCommand.ChangeLayer(clickLayerMask);
                StartCoroutine(BlurManager.instance.BlurCoroutine(true));
            }
            else
            {
                //예시) 부모Enter, 자식Enter, 부모Exit
                MultiTreeCommand topCommand = selectCommandStack.Peek();

                if (topCommand.IsChildCommand(multiTreeCommand)) //자식인 경우
                {
                    selectCommandStack.Push(multiTreeCommand);
                }
                else if (topCommand.IsSiblingCommand(multiTreeCommand)) //동일 형제인경우    
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

        //이전 지역 정보 비활성화
        if (currentMoveCommand)
        {
            currentMoveCommand.IsCondition = true;

            Action_Condition tmp = currentMoveCommand.GetComponent<Action_Condition>();
            if (tmp)
                tmp.CommandListOnOff(false);
        }

        //기본 최상위 행동 먼저 실행
        yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(defaultCommands));

        if (currentMoveCommand)
        {
            //이전 장소 고유 오브젝트 And 아이템류 비활성화
            yield return StartCoroutine(DisActiveCommandCoroutine(currentMoveCommand));

            //이전 장소 백그라운드 대사효과 비활성화
            yield return (StartCoroutine(backgroundSpeechArea.DisableSpeech()));
        }

        //장소 이동
        Player.instance.CurrentLocation = command.CommandName;

        //행동 활성화
        Action_Condition tt = command.GetComponent<Action_Condition>();
        if (tt)
            tt.CommandListOnOff(true);

        //기타 오브젝트 활성화
        {
            LocationCommandList locationCommandList = locationList.GetLocationCommandList(command);

            List<MultiTreeCommand> commands = new List<MultiTreeCommand>();

            //테스트용
            CreateItem(command);

            //장소 고유 오브젝트 찾기
            foreach (MultiTreeCommandBool multiTreeCommandBool in locationCommandList.multiTreeCommands)
            {
                multiTreeCommandBool.multiTreeCommand.IsCondition = true;
                commands.Add(multiTreeCommandBool.multiTreeCommand);
            }

            yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(commands));
        }
        //장소 백그라운드 대사효과 활성화
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