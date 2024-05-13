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
    //������ �����۵�
    private Dictionary<VillageMoveCommand, MultiTreeCommand> createList = new Dictionary<VillageMoveCommand, MultiTreeCommand>();

    [SerializeField] private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    [SerializeField] private VillageMoveCommand startPosition;

    #region �̵� �� ��ȣ�ۿ� ����
    [SerializeField] protected float power = 10f;

    public MultiTreeCommand currentCommand;
    public Vector3 currentMousePosition;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    public Vector3 clickMousePosition;
    public bool click;
    #endregion

    #region ���̾�
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

            //���콺�� ������ �����̸� �巡�� ���� �ൿ�� ��ħ.
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

    //���ñ���
    public void Interaction(MultiTreeCommand multiTreeCommand, MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        Debug.Log(currentMouseStatus);

        if (mouseStatus == MouseStatus.Enter)
        {
            if (selectCommandStack.Count == 0)      //���ʷ� ���°��
            {
                UIManager.instance.Blur(true);
                selectCommandStack.Push(multiTreeCommand);
                ChangeLayerRecursively(multiTreeCommand.RootCommand, clickLayerMask);
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

    #region ���̾� ����
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

    #region �ִϸ��̼�
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