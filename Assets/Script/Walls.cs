using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public BoxCollider leftWall;
    public BoxCollider rightWall;
    public BoxCollider topWall;
    public BoxCollider bottomWall;

    public float OffsetSize = 1f;

    //public void SetBoundaryColliderPositions()
    //{
    //    // 카메라의 수직 시야각 계산
    //    float halfFOV = Camera.main.fieldOfView * 0.5f;

    //    // 카메라의 수평 시야각 계산
    //    float aspect = Camera.main.aspect;
    //    float halfHorizontalFOV = Mathf.Atan(Mathf.Tan(halfFOV * Mathf.Deg2Rad) * aspect) * Mathf.Rad2Deg;

    //    // 경계 거리에서의 상하좌우 범위 계산
    //    float halfHeight = transform.position.z * Mathf.Tan(halfFOV * Mathf.Deg2Rad);
    //    float halfWidth = transform.position.z * Mathf.Tan(halfHorizontalFOV * Mathf.Deg2Rad);

    //    // 카메라 위치를 기준으로 경계 좌표 계산
    //    Vector3 cameraPosition = Camera.main.transform.position;
    //    float topBoundary = cameraPosition.y + halfHeight;
    //    float bottomBoundary = cameraPosition.y - halfHeight;
    //    float leftBoundary = cameraPosition.x - halfWidth;
    //    float rightBoundary = cameraPosition.x + halfWidth;

    //    // 상단 콜라이더 위치 및 크기 설정
    //    topWall.transform.localPosition = new Vector3(0f, topBoundary - transform.position.y + zOffset, 0);
    //    topWall.size = new Vector3(2f * halfWidth, zOffset * 2, zOffset);

    //    // 하단 콜라이더 위치 및 크기 설정
    //    bottomWall.transform.localPosition = new Vector3(0f, bottomBoundary - transform.position.y - zOffset, 0);
    //    bottomWall.size = new Vector3(2f * halfWidth, zOffset * 2, zOffset);

    //    // 좌측 콜라이더 위치 및 크기 설정
    //    leftWall.transform.localPosition = new Vector3(leftBoundary - transform.position.x - zOffset, 0f, 0);
    //    leftWall.size = new Vector3(zOffset * 2, 2f * halfHeight, zOffset);

    //    // 우측 콜라이더 위치 및 크기 설정
    //    rightWall.transform.localPosition = new Vector3(rightBoundary - transform.position.x + zOffset, 0f, 0);
    //    rightWall.size = new Vector3(zOffset * 2, 2f * halfHeight, zOffset);
    //}

    public void SetBoundaryColliderPositions()
    {
        // 카메라 위치를 기준으로 경계 좌표 계산
        Vector3 bottomLeft = CameraExtensions.GetBottomLeftPosition(transform.position.z);
        Vector3 bottomRight = CameraExtensions.GetBottomRightPosition(transform.position.z);
        Vector3 topLeft = CameraExtensions.GetTopLeftPosition(transform.position.z);
        Vector3 topRight = CameraExtensions.GetTopRightPosition(transform.position.z);

        // 상단 콜라이더 위치 및 크기 설정
        topWall.transform.localPosition = new Vector3(0f, topRight.y - transform.position.y + OffsetSize, 0f);
        topWall.size = new Vector3(topRight.x - topLeft.x, OffsetSize * 2f, OffsetSize);

        // 하단 콜라이더 위치 및 크기 설정
        bottomWall.transform.localPosition = new Vector3(0f, bottomLeft.y - transform.position.y - OffsetSize, 0f);
        bottomWall.size = new Vector3(bottomRight.x - bottomLeft.x, OffsetSize * 2f, OffsetSize);

        //모서리 부분 확장
        float cornerOffset = OffsetSize * 4;

        // 좌측 콜라이더 위치 및 크기 설정
        leftWall.transform.localPosition = new Vector3(bottomLeft.x - transform.position.x - OffsetSize, 0f, 0f);
        leftWall.size = new Vector3(OffsetSize * 2f, topLeft.y - bottomLeft.y + cornerOffset, OffsetSize);

        // 우측 콜라이더 위치 및 크기 설정
        rightWall.transform.localPosition = new Vector3(bottomRight.x - transform.position.x + OffsetSize, 0f, 0f);
        rightWall.size = new Vector3(OffsetSize * 2f, topRight.y - bottomRight.y + cornerOffset, OffsetSize);
    }
}
