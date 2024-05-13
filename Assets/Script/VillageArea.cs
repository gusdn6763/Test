using Febucci.UI.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VillageArea : MonoBehaviour
{
    //생성된 아이템들
    private Dictionary<VillageMoveCommand, MultiTreeCommand> createList = new Dictionary<VillageMoveCommand, MultiTreeCommand>();

    [SerializeField] private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    [SerializeField] private VillageMoveCommand startPosition;

    #region 이동 및 상호작용 관련
    [SerializeField] protected float power = 10f;

    public MultiTreeCommand currentCommand;
    public Vector3 currentMousePosition;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    public Vector3 clickMousePosition;
    public bool click;
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
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            click = true;

        if (Input.GetMouseButtonUp(0) && click)
            click = false;

        CommandActivingCoroutine();
    }

    public void DisAbleAllVillageCommand()
    {
        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        foreach (MultiTreeCommand command in commands)
            if (command.ParentCommand == null)
                command.DisableAllCommandFromBottom();
    }

    public void CommandActivingCoroutine()
    {
        if (AnimationManager.instance.IsAnimation)
            return;

            //yield return new WaitUntil(() => AnimationManager.instance.IsAnimation == false);

            if (selectCommandStack.Count > 0)
                currentLayerMask = clickLayerMask;
            else
                currentLayerMask = defaultLayerMask;

            Vector3 mousePosition = Input.mousePosition;

            //마우스를 빠르게 움직이면 드래그 중인 행동을 놓침.
            if (clickCommand is IMoveable)
            {
                if (Input.GetMouseButton(0))
                {
                    (currentCommand as IMoveable).SetPower(mousePosition - currentMousePosition);
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

        currentMousePosition = Input.mousePosition;
        
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
                UIManager.instance.Blur(true);
                selectCommandStack.Push(multiTreeCommand);
                ChangeLayerRecursively(multiTreeCommand.RootCommand, clickLayerMask);
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
                ChangeLayerRecursively(multiTreeCommand.RootCommand, defaultLayerMask);
                UIManager.instance.Blur(false);
            }
        }
    }

    #region 레이어 변경
    private void ChangeLayerRecursively(MultiTreeCommand command, LayerMask layerMask)
    {
        int layer = LayerMaskExtensions.ToSingleLayer(layerMask);
        command.gameObject.layer = layer;

        for (int i = 0; i < command.transform.childCount; i++)
            command.transform.GetChild(i).gameObject.layer = layer;

        foreach (MultiTreeCommand child in command.ChildCommands)
            ChangeLayerRecursively(child, layerMask);
    }
    #endregion

    #region 애니메이션
    public void VillageSetting()
    {
        IMoveable[] commands = GetComponentsInChildren<IMoveable>(true);
        List<MultiTreeCommand> commandList = new List<MultiTreeCommand>();

        foreach (IMoveable moveable in commands)
            if (moveable is MultiTreeCommand command)
                commandList.Add(command);

        StartCoroutine(AnimationManager.instance.VillageSettingCoroutine(commandList));
    }
    #endregion
}