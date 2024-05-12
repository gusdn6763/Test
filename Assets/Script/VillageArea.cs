using Febucci.UI.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class VillageArea : MonoBehaviour
{
    //������ �����۵�
    private Dictionary<VillageMoveCommand, MultiTreeCommand> createList = new Dictionary<VillageMoveCommand, MultiTreeCommand>();

    [SerializeField] private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    [SerializeField] private VillageMoveCommand startPosition;

    #region �̵� �� ��ȣ�ۿ� ����
    public MultiTreeCommand currentCommand;
    public Vector3 currentMousePosition;
    public MouseStatus currentMouseStatus;
    [SerializeField] protected float power = 10f;
    public Vector3 clickMousePosition;
    public bool dragable = false;
    #endregion

    #region ���̾�
    [SerializeField] private LayerMask clickLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private LayerMask currentLayerMask;
    #endregion


    private void Start()
    {
        DisAbleAllVillageCommand();

        startPosition.Interaction(MouseStatus.Excute);

        StartCoroutine(CommandActivingCoroutine());
    }

    public void DisAbleAllVillageCommand()
    {
        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        foreach (MultiTreeCommand command in commands)
            if (command.ParentCommand == null)
                command.DisableAllCommandFromBottom();
    }

    IEnumerator CommandActivingCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => AnimationManager.instance.IsAnimation == false);

            if (selectCommandStack.Count > 0)
                currentLayerMask = clickLayerMask;
            else
                currentLayerMask = defaultLayerMask;

            Vector3 mousePosition = Input.mousePosition;

            //���콺�� ������ �����̸� �巡�� ���� �ൿ�� ��ħ.
            //1. IMoveable 2.�巡�� ����
            if (dragable)
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
                            else if (Input.GetMouseButton(0) && currentMouseStatus != MouseStatus.Down)
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

            yield return null;
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

        if (mouseStatus == MouseStatus.Enter)
        {
            if (selectCommandStack.Count == 0)      //���ʷ� ���°��
            {
                UIManager.instance.Blur(true);
                selectCommandStack.Push(multiTreeCommand);
                ChangeLayerRecursively(multiTreeCommand, clickLayerMask);
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
            dragable = true;
            clickMousePosition = Input.mousePosition;
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Drag)
        {
            if (Input.mousePosition != currentMousePosition)
                dragable = false;

            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Up)
        {
            multiTreeCommand.Interaction(mouseStatus);

            if (dragable)
                Interaction(multiTreeCommand, MouseStatus.Excute);

            dragable = false;
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
                ChangeLayerRecursively(multiTreeCommand.GetRootCommand, defaultLayerMask);
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
    //��... �ð��� ����.
    private Queue<IMoveable> revertList = new Queue<IMoveable>();
    private Queue<int> onList = new Queue<int>();
    private Queue<int> offList = new Queue<int>();
    public void VillageSetting(VillageMoveCommand villageMoveCommand)
    {
        StartCoroutine(VillageSettingCoroutine());
    }
    IEnumerator VillageSettingCoroutine()
    {
        List<MultiTreeCommand> parentList = new List<MultiTreeCommand>();

        MultiTreeCommand[] commands = GetComponentsInChildren<MultiTreeCommand>(true);

        List<MultiTreeCommand> defaultList = new List<MultiTreeCommand>();
        List<MultiTreeCommand> defaultDisList = new List<MultiTreeCommand>();

        foreach (MultiTreeCommand command in commands)
            if (command.ParentCommand == null)
                parentList.Add(command);

        foreach (MultiTreeCommand command in parentList)
        {
            if (command.IsCondition && command.isActiveAndEnabled == false)
                defaultList.Add(command);
            else if (command.IsCondition == false && command.isActiveAndEnabled)
                defaultDisList.Add(command);
        }

        yield return new WaitUntil(() => AnimationManager.instance.IsAnimation == false);

        AnimationManager.instance.Animation(defaultDisList, false);
        AnimationManager.instance.InitialAnimation(defaultList);
    }
    #endregion
}