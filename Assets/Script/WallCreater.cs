using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCreater : MonoBehaviour
{
    private void Awake()
    {
        CreateBoundaryColliders();
    }

    private void CreateBoundaryColliders()
    {
        Walls walls = Instantiate(PrefabManager.instance.walls, transform);
        walls.SetBoundaryColliderPositions();
    }
}