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

    #region 이동 및 상호작용 관련
    public MultiTreeCommand currentCommand;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    private Vector3 clickMousePosition;
    #endregion

    #region 레이어
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

        //마우스를 빠르게 움직이면 드래그 중인 행동을 놓침.
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
                multiTreeCommand.ChangeLayer(clickLayerMask);            }
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
