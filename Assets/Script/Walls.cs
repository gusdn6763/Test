using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public BoxCollider leftWall;
    public BoxCollider rightWall;
    public BoxCollider topWall;
    public BoxCollider bottomWall;

    public float zOffset = 1f;

    public void SetBoundaryColliderPositions()
    {
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

        // ��� �ݶ��̴� ��ġ �� ũ�� ����
        topWall.transform.localPosition = new Vector3(0f, topBoundary - transform.position.y + zOffset, 0);
        topWall.size = new Vector3(2f * halfWidth, zOffset * 2, zOffset);

        // �ϴ� �ݶ��̴� ��ġ �� ũ�� ����
        bottomWall.transform.localPosition = new Vector3(0f, bottomBoundary - transform.position.y - zOffset, 0);
        bottomWall.size = new Vector3(2f * halfWidth, zOffset * 2, zOffset);

        // ���� �ݶ��̴� ��ġ �� ũ�� ����
        leftWall.transform.localPosition = new Vector3(leftBoundary - transform.position.x - zOffset, 0f, 0);
        leftWall.size = new Vector3(zOffset * 2, 2f * halfHeight, zOffset);

        // ���� �ݶ��̴� ��ġ �� ũ�� ����
        rightWall.transform.localPosition = new Vector3(rightBoundary - transform.position.x + zOffset, 0f, 0);
        rightWall.size = new Vector3(zOffset * 2, 2f * halfHeight, zOffset);
    }
}
