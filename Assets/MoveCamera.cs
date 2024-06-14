using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject camera2;
    public GameObject canvas;
    public Area activeArea;
    public Area disActiveArea;
    public void MoveCamera2()
    {
        camera2.transform.position = new Vector3(0, 0, 0);
        canvas.gameObject.SetActive(false);
        disActiveArea.start = false;
        activeArea.start = true;

    }
}
