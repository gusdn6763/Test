using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TextSeparator2 : MonoBehaviour
{
    private TextMeshPro text;

    public bool IsOn { get; set; }

    private float duration;
    private float durationReduce;
    private float xMax;
    private float yMax;
    private float moveSpeed;
    private bool isArrived = true;
    private Vector3 startPos;
    private Vector3 arrivedPos;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Init(string dividString, float fontSize, float speed, float dur, float durReduce, float x, float y)
    {
        text.text = dividString;
        text.fontSize = fontSize;
        moveSpeed = speed;
        durationReduce = durReduce;
        duration = dur;
        xMax = x;
        yMax = y;
    }

    private void Update()
    {
        if (IsOn)
        {
            if (isArrived)
            {
                startPos = transform.localPosition;
                arrivedPos = new Vector3(Random.Range(-xMax, xMax), Random.Range(-yMax, yMax), 0);
                isArrived = false;
            }

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, arrivedPos, moveSpeed);

            if (transform.localPosition == arrivedPos)
            {
                isArrived = true;
            }
        }
    }

    public IEnumerator ComeBack()
    {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        while (time < duration - durationReduce)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, time / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
