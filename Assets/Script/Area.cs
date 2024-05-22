using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public bool isInteraction = false;

    public Vector3 FindSpawnPosition(MultiTreeCommand multiTreeCommand)
    {
        Vector3 position = multiTreeCommand.transform.position;
        Vector3 size = multiTreeCommand.GetSize();

        // 중복되지 않는 위치 찾기
        do
        {
            position = GetRandomPosition();
        } while (CheckOverlap(position, size));

        return position;
    }

    public Vector3 GetRandomPosition()
    {
        float z = transform.position.z;

        Vector3 bottomLeft = CameraExtensions.GetBottomLeftPosition(z);
        Vector3 topRight = CameraExtensions.GetTopRightPosition(z);

        float randomX = Random.Range(bottomLeft.x, topRight.x);
        float randomY = Random.Range(bottomLeft.y, topRight.y);

        return new Vector3(randomX, randomY, z);
    }

    public bool CheckOverlap(Vector3 position, Vector3 size)
    {
        Collider[] colliders = Physics.OverlapBox(position, size / 2f);

        if (colliders.Length == 0)
            return true;
        
        return false;
    }
}
