using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TextSeparator : MonoBehaviour
{
    private TextMeshPro text;

    public bool IsOn { get; set; }

    private int frequencyPerSecond;
    private float duration;
    private float xMax;
    private float yMax;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Init(string dividString, float fontSize, int frequency, float dur, float x, float y)
    {
        text.text = dividString;
        text.fontSize = fontSize;
        frequencyPerSecond = frequency;
        duration = dur;
        xMax = x;
        yMax = y;
    }

    private void Update()
    {
        if (IsOn)
        {
            if (Time.frameCount % (60f / frequencyPerSecond) == 0)
            {
                transform.localPosition = new Vector3(Random.Range(-xMax, xMax), Random.Range(-yMax, yMax), 0);
            }
        }
    }

    public IEnumerator ComeBack()
    {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, time / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
