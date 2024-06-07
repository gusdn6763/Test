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
            if (CurrentValue + value >= fullValue)
                previewValue = fullValue;
            else
                previewValue = CurrentValue + value;

            if (previewValue > CurrentValue)              //증가
                arrowImage.localScale = new Vector3(-1, 1, 1);
            else if (previewValue < CurrentValue)          //감소
                arrowImage.localScale = new Vector3(1, 1, 1);
            else
                arrowImage.localScale = Vector3.zero;

            float fillValue = previewValue / fullValue;
            previewImage.fillAmount = fillValue;
        }
    }

    public float CurrentValue
    {
        get { return currentValue; }
        set
        {
            currentValue = value;
            //previewValue = value;

            arrowImage.localScale = Vector3.zero;

            currentImage.fillAmount = currentValue / fullValue;
            previewImage.fillAmount = currentImage.fillAmount;
        }
    }


    private void Awake()
    {
        currentImage = GetComponent<Image>();
    }


    public void Initialize(float maxValue)
    {
        CurrentValue = fullValue = maxValue;
        arrowImage.localScale = new Vector3(0,0,0);
    }
}