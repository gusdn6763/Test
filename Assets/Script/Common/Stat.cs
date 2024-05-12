using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private RectTransform arrowImage;

    private Image currentImage;

    private float fullValue;
    private float currentValue;
    private float previewValue;


    public float PreviewValue
    {
        get { return previewValue; }
        set
        {
            if (previewValue + value > fullValue)
                previewValue = fullValue;
            else
                previewValue = currentValue + value;

            if (value > 0)              //증가
                arrowImage.localScale = new Vector3(-1, 1, 1);
            else if (value < 0)          //감소
                arrowImage.localScale = new Vector3(1, 1, 1);
            else
                arrowImage.localScale = Vector3.zero;

            float fillValue = previewValue / currentValue;
            previewImage.fillAmount = fillValue;
        }
    }

    public float CurrentValue
    {
        get { return currentValue; }
        set
        {
            currentValue = value;

            arrowImage.localScale = Vector3.zero;

            currentImage.fillAmount = currentValue / fullValue;
        }
    }


    private void Awake()
    {
        currentImage = GetComponent<Image>();
    }


    public void Initialize(float maxValue)
    {
        fullValue = previewValue = currentValue = maxValue;
        arrowImage.localScale = new Vector3(0,0,0);
    }
}