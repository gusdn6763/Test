using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillageArea : MonoBehaviour
{
    //생성된 아이템들
    private Dictionary<MoveCommand, MultiTreeCommand> createList = new Dictionary<MoveCommand, MultiTreeCommand>();
    private MoveCommand currentMoveCommand;

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    [SerializeField] private MoveCommand startPosition;
    [SerializeField] private LocationList locationList;

    #region 이동 및 상호작용 관련
    private MultiTreeCommand currentCommand;
    private MouseStatus currentMouseStatus;

    private MultiTreeCommand clickCommand;
    private Vector3 clickMousePosition;
    #endregion

    #region 레이어
    [SerializeField] private LayerMask clickLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private LayerMask currentLayerMask;
    #endregion

    private void Start()
    {
        DisAbleAllVillageCommand();

        startPosition.onMouseEvent.Invoke(MouseStatus.Excute);
        startPosition.onAnimationEndEvent.Invoke(MouseStatus.Excute);
        MoveLocation(startPosition);
    }

    private void Update()
    {
        if (AnimationManager.instance.IsAnimation || BlurManager.instance.BlurStart)
            return;

        CommandActiving();
    }

    public void DisAbleAllVillageCommand()
    {
        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        foreach (MultiTreeCommand command in commands)
        {
            if (command.IsRootCommand)
                command.DisableAllCommandFromBottom();
        }

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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, currentLayerMask))
            {
                MultiTreeCommand command = hit.collider.GetComponent<MultiTreeCommand>();
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
                        else if (Input.GetMouseButton(0) && clickCommand == null)
                            Interaction(currentCommand, MouseStatus.Down);
                        else if (Input.GetMouseButton(0))
                            Interaction(currentCommand, MouseStatus.Drag);
                    }
                }
            }
            else if (currentCommand)
            {
                // 마우스가 어떤 오브젝트와도 충돌하지 않았을 때
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

    //스택구조
    public void Interaction(MultiTreeCommand multiTreeCommand, MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        Debug.Log(currentMouseStatus);

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
                StartCoroutine(VillageSetting(multiTreeCommand));
            }
        }
    }


    #region 애니메이션
    public void Refresh(MoveCommand villageMoveCommand)
    {
        currentMoveCommand = villageMoveCommand;

        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);
        List<MultiTreeCommand> commandList = new List<MultiTreeCommand>();

        foreach (MultiTreeCommand command in commands)
            if (command.IsRootCommand)
                commandList.Add(command);

        StartCoroutine(RefreshCoroutine(commandList));
    }

    private IEnumerator RefreshCoroutine(List<MultiTreeCommand> commandList)
    {
        List<MultiTreeCommand> destroyableCommands = new List<MultiTreeCommand>();

        foreach (var entry in createList.ToList())
        {
            if (entry.Key != currentMoveCommand && !entry.Key.SaveLocation)
            {
                entry.Value.IsCondition = false;
                destroyableCommands.Add(entry.Value);
                createList.Remove(entry.Key);
            }
        }

        yield return StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(commandList));

        foreach (var command in destroyableCommands)
            Destroy(command.gameObject);
    }

    #endregion
    IEnumerator VillageSetting(MultiTreeCommand multiTreeCommand)
    {
        yield return StartCoroutine(BlurManager.instance.BlurCoroutine(false));
        multiTreeCommand.ChangeLayer(defaultLayerMask);

        if (multiTreeCommand is MoveCommand)
            MoveLocation(multiTreeCommand as MoveCommand);
    }

    public void MoveLocation(MoveCommand command)
    {
        Player.instance.CurrentLocation = command.CommandName;

        //이전 지역 정보 비활성화
        if (currentMoveCommand)
        {
            currentMoveCommand.IsCondition = true;

            Action_Condition tmp = currentMoveCommand.GetComponent<Action_Condition>();
            if (tmp)
                tmp.CommandListOnOff(false);
        }

        locationList.CaculateAllMoveCommandStatus(command);

        if (command.alternativeLocation)
        {
            MoveLocation(command.alternativeLocation);
        }
        else
        {
            if (command.IsDisable)
                command.IsCondition = false;

            currentMoveCommand = command;
            Refresh(currentMoveCommand);
        }
    }

    public void CreateItem(MoveCommand villageMoveCommand, ItemCommand itemRootCommand)
    {
        MultiTreeCommand command = Instantiate(itemRootCommand, transform);
        command.transform.localPosition = Vector3.zero;
        createList.Add(villageMoveCommand, command);
        command.gameObject.SetActive(false);
    }
}