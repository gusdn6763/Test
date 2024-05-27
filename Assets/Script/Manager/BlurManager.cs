using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;



public class BlurManager : MonoBehaviour
{
    public static BlurManager instance;

    public bool BlurStart { get; private set; }

    [Header("증가별 대기시간")]
    [SerializeField] private float waitTime;

    [Header("Weight 증가량")]
    [SerializeField] private float value;

    private DepthOfField field;
    private Volume volume;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        volume = GetComponent<Volume>();
    }

    public void Blur(bool isOn)
    {
        if (isOn)
            volume.weight = 1f;
        else
            volume.weight = 0f;
    }

    public IEnumerator BlurCoroutine(bool isOn)
    {
        BlurStart = true;

        if (isOn)
        {
            while (volume.weight < 1f)
            {
                volume.weight += value;
                yield return new WaitForSeconds(waitTime);
            }
            volume.weight = 1f;
        }
        else
        {
            while (volume.weight > 0f)
            {
                volume.weight -= value;
                yield return new WaitForSeconds(waitTime);
            }
            volume.weight = 0f;
        }
        BlurStart = false;
    }
}