using UnityEngine;

public static class Utils
{
    private static void CalculateCameraExtents(float zPosition, out float halfWidth, out float halfHeight)
    {
        float halfFOV = Camera.main.fieldOfView * 0.5f;
        float aspect = Camera.main.aspect;
        halfHeight = zPosition * Mathf.Tan(halfFOV * Mathf.Deg2Rad);
        halfWidth = halfHeight * aspect;
    }
    public static Vector3 GetBottomLeftPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 bottomLeft = Camera.main.transform.position - new Vector3(halfWidth, halfHeight, 0f);
        bottomLeft.z = zPosition;
        return bottomLeft;
    }
    public static Vector3 GetBottomRightPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 bottomRight = Camera.main.transform.position + new Vector3(halfWidth, -halfHeight, 0f);
        bottomRight.z = zPosition;
        return bottomRight;
    }
    public static Vector3 GetTopLeftPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 topLeft = Camera.main.transform.position + new Vector3(-halfWidth, halfHeight, 0f);
        topLeft.z = zPosition;
        return topLeft;
    }
    public static Vector3 GetTopRightPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 topRight = Camera.main.transform.position + new Vector3(halfWidth, halfHeight, 0f);
        topRight.z = zPosition;
        return topRight;
    }
}