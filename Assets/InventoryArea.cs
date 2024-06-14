using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryArea : Area
{
    public static InventoryArea instance;

    [SerializeField] protected TextMeshProUGUI weightText;
    [SerializeField] protected float fullWeight = 100f;
    [SerializeField] protected float currentWeight;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();

    #region �̵� �� ��ȣ�ۿ� ����
    public MultiTreeCommand currentCommand;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    private Vector3 clickMousePosition;
    #endregion

    #region ���̾�
    [SerializeField] private LayerMask clickLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private LayerMask currentLayerMask;
    #endregion

    private void Update()
    {
        if (start)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentMouseStatus == MouseStatus.Enter && currentCommand)
                    Interaction(currentCommand, MouseStatus.DownWait);
            }
            if (AnimationManager.instance.IsAnimation || IsWait)
                return;

            CommandActiving();
        }
    }

    public override Vector3 GetRandomPosition(Vector3 size)
    {
        float z = transform.position.z;

        Vector3 bottomLeft = Utils.GetBottomLeftPosition(z);
        Vector3 topRight = Utils.GetTopRightPosition(z);

        float randomX = Random.Range(bottomLeft.x + size.x, topRight.x - size.x);
        float randomY = Random.Range(bottomLeft.y + size.y, topRight.y - size.y);

        return new Vector3(randomX + transform.position.x, randomY + transform.position.y, z);
    }

    public void CaculateWeight()
    {
        currentWeight = 0;
        foreach (Transform child in transform)
        {
            ItemCommand tmp = child.GetComponent<ItemCommand>();
            if (tmp)
                currentWeight += tmp.Weight;
        }

        weightText.text = currentWeight + " / " + fullWeight;
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

            return;
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
                    if (AnimationManager.instance.IsAnimation || IsWait)
                    {

                    }
                    else
                        Interaction(currentCommand, MouseStatus.Down);
                }
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
                multiTreeCommand.ChangeLayer(clickLayerMask);            }
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
            clickMousePosition = Input.mousePosition;
            multiTreeCommand.Interaction(mouseStatus);
        }
        else if (mouseStatus == MouseStatus.Down)
        {
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
            multiTreeCommand.Interaction(mouseStatus);

            if (clickMousePosition == Input.mousePosition)
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
                multiTreeCommand.ChangeLayer(defaultLayerMask);
            }
        }
    }
}
