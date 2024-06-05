using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private List<MultiTreeCommand> commands = new List<MultiTreeCommand>();

    public bool IsWait = false;

    public virtual void AddCommand(MultiTreeCommand command)
    {
        if (!commands.Contains(command))
        {
            commands.Add(command);
            // �ʿ��� ó�� ����
        }
    }

    public virtual void RemoveCommand(MultiTreeCommand command)
    {
        if (commands.Contains(command))
        {
            commands.Remove(command);
            // �ʿ��� ó�� ����
        }
    }

    public Vector3 FindSpawnPosition(MultiTreeCommand multiTreeCommand)
    {
        Vector3 position;
        Vector3 size = multiTreeCommand.GetSize();

        // �ߺ����� �ʴ� ��ġ ã��
        do
        {
            position = GetRandomPosition(size);
        } while (CheckOverlap(position, size));

        return position;
    }
    public Vector3 GetRandomPosition(Vector3 size)
    {
        float z = transform.position.z;

        Vector3 bottomLeft = Utils.GetBottomLeftPosition(z);
        Vector3 topRight = Utils.GetTopRightPosition(z);

        float randomX = Random.Range(bottomLeft.x + size.x, topRight.x - size.x);
        float randomY = Random.Range(bottomLeft.y + size.y, topRight.y - size.y);

        return new Vector3(randomX, randomY, z);
    }
    public bool CheckOverlap(Vector3 position, Vector3 size)
    {
        Collider[] colliders = Physics.OverlapBox(position, size / 2f);

        return colliders.Length > 0;
    }
}
