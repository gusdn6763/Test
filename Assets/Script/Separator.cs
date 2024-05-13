using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Separator : MonoBehaviour
{
    private TextMeshPro text;

    public bool IsOn { get; set; }

    [Range(0,60)] public int frequencyPerSecond;

    public float duration = 1f;
    public float xMax;
    public float yMax;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
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

    public void GetInfo(string dividString, float fontSize)
    {
        text.text = dividString;
        text.fontSize = fontSize;
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
