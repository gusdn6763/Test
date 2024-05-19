using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VillageArea : MonoBehaviour
{
    //������ �����۵�
    private Dictionary<MoveCommand, MultiTreeCommand> createList = new Dictionary<MoveCommand, MultiTreeCommand>();
    private MoveCommand currentLocation;

    private Stack<MultiTreeCommand> selectCommandStack = new Stack<MultiTreeCommand>();
    [SerializeField] private MoveCommand startPosition;

    #region �̵� �� ��ȣ�ۿ� ����
    [SerializeField] private Transform wallTransform;
    [SerializeField] private GameObject wallPrefab;
    private List<BoxCollider> walls = new List<BoxCollider>();

    public MultiTreeCommand currentCommand;
    public Vector3 currentMousePosition;
    public MouseStatus currentMouseStatus;

    public MultiTreeCommand clickCommand;
    public Vector3 clickMousePosition;
    #endregion

    #region ���̾�
    [SerializeField] private LayerMask clickLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    private LayerMask currentLayerMask;
    #endregion

    private void Start()
    {
        DisAbleAllVillageCommand();
        CreateBoundaryColliders();
        startPosition.onMouseEvent.Invoke(MouseStatus.Excute);
        startPosition.onAnimationEndEvent.Invoke(MouseStatus.Excute);
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
    private void CreateBoundaryColliders()
    {
        // �����¿� ��� �ݶ��̴� ����
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallTransform);
            walls.Add(wall.GetComponent<BoxCollider>());
        }

        // �ݶ��̴� ��ġ �� ũ�� ����
        SetBoundaryColliderPositions();
    }
    private void SetBoundaryColliderPositions()
    {
        Vector2 Right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 0.5f, transform.position.z));
        Vector2 Left = -Right;
        Vector2 Top = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height));
        Vector2 Bottom = -Top;

        // ī�޶��� ���� �þ߰� ���
        float halfFOV = Camera.main.fieldOfView * 0.5f;

        // ī�޶��� ���� �þ߰� ���
        float aspect = Camera.main.aspect;
        float halfHorizontalFOV = Mathf.Atan(Mathf.Tan(halfFOV * Mathf.Deg2Rad) * aspect) * Mathf.Rad2Deg;

        // ��� �Ÿ������� �����¿� ���� ���
        float halfHeight = transform.position.z * Mathf.Tan(halfFOV * Mathf.Deg2Rad);
        float halfWidth = transform.position.z * Mathf.Tan(halfHorizontalFOV * Mathf.Deg2Rad);

        // ī�޶� ��ġ�� �������� ��� ��ǥ ���
        Vector3 cameraPosition = Camera.main.transform.position;
        float topBoundary = cameraPosition.y + halfHeight;
        float bottomBoundary = cameraPosition.y - halfHeight;
        float leftBoundary = cameraPosition.x - halfWidth;
        float rightBoundary = cameraPosition.x + halfWidth;

        int offset = 1;
        // ��� �ݶ��̴� ��ġ �� ũ�� ����
        walls[0].transform.localPosition = new Vector3(0f, topBoundary - transform.position.y + offset, 0);
        walls[0].size = new Vector3(2f * halfWidth, offset * 2, offset);

        // �ϴ� �ݶ��̴� ��ġ �� ũ�� ����
        walls[1].transform.localPosition = new Vector3(0f, bottomBoundary - transform.position.y - offset, 0);
        walls[1].size = new Vector3(2f * halfWidth, offset * 2, offset);

        // ���� �ݶ��̴� ��ġ �� ũ�� ����
        walls[2].transform.localPosition = new Vector3(leftBoundary - transform.position.x - offset, 0f, 0);
        walls[2].size = new Vector3(offset * 2, 2f * halfHeight, offset);

        // ���� �ݶ��̴� ��ġ �� ũ�� ����
        walls[3].transform.localPosition = new Vector3(rightBoundary - transform.position.x + offset, 0f, 0);
        walls[3].size = new Vector3(offset * 2, 2f * halfHeight, offset);
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
                Vector3 direction = mousePosition - currentMousePosition;

                //(currentCommand as IRootCommand).SetPower(direction.normalized);
                //(currentCommand as IRootCommand).SetPower(direction);
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
                Debug.Log(hit.collider.name);
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


    #region �ִϸ��̼�
    public void Refresh(MoveCommand villageMoveCommand)
    {
        currentLocation = villageMoveCommand;

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
            if (entry.Key != currentLocation && !entry.Key.SaveLocation)
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

    public void CreateItem(MoveCommand villageMoveCommand, ItemCommand itemRootCommand)
    {
        MultiTreeCommand command = Instantiate(itemRootCommand, transform);
        command.transform.localPosition = Vector3.zero;
        createList.Add(villageMoveCommand, command);
        command.gameObject.SetActive(false);
    }

    IEnumerator VillageSetting(MultiTreeCommand multiTreeCommand)
    {
        yield return StartCoroutine(BlurManager.instance.BlurCoroutine(false));
        multiTreeCommand.ChangeLayer(defaultLayerMask);
    }
}