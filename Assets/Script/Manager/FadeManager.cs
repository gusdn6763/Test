using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : UIScript
{
    public static FadeManager instance;

    public bool IsFade { get; private set; }

    [SerializeField] private float fadeSpeed = 0.01f;
    [SerializeField] private Color imageColor;
    [SerializeField] private Color textColor;

    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI text;

    private List<string> currentTexts;
    private int count = 0;

    protected override void Awake()
    {
        base.Awake();

        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        textColor = Color.white;
        imageColor = Color.black;
    }

    public void SkipButton()
    {
        currentTexts = null;
        imageColor.a = 0;
        textColor.a = 0;

        StopAllCoroutines();
        OpenClose(false);
    }

    public void FadeInImmediately(string textName)
    {
        IsFade = true;

        OpenClose(true);
        text.text = textName;

        imageColor.a = 1;
        textColor.a = 1;
        fadeImage.color = imageColor;
        text.color = textColor;
    }

    public void FadeInImmediately(List<string> texts)
    {
        IsFade = true;

        OpenClose(true);

        count = 0;
        currentTexts = texts;
        FadeInImmediately(currentTexts[count]);
    }

    public void NextShow()
    {
        if (currentTexts != null)
        {
            if (currentTexts.Count > count + 1)
                FadeInImmediately(currentTexts[++count]);
            else
                SkipButton();
        }
    }

    public void PreviewShow()
    {
        if (count > 0)
        {
            count--;
            FadeInImmediately(currentTexts[count]);
        }
    }

    public void FadeIn(string textName, float time = 3)
    {
        OpenClose(true);
        text.text = textName;

        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(time));
    }

    public IEnumerator FadeInCoroutine(float time)
    {

        while (imageColor.a < 1f)
        {
            imageColor.a += 0.01f;
            fadeImage.color = imageColor;

            textColor.a += 0.01f;
            text.color = textColor;

            yield return new WaitForSeconds(fadeSpeed);
        }
        yield return new WaitUntil(() => imageColor.a >= 1f);
        yield return new WaitForSeconds(time);
        FadeOut(time);
    }
    public void FadeOut(float time = 2)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(time));
    }
    IEnumerator FadeOutCoroutine(float time)
    {
        while (imageColor.a > 0f)
        {
            imageColor.a -= 0.01f;
            fadeImage.color = imageColor;

            textColor.a -= 0.01f;
            text.color = textColor;

            yield return new WaitForSeconds(fadeSpeed);
        }
        yield return new WaitUntil(() => imageColor.a <= 0f);

        OpenClose(false);
        IsFade = false;
    }
}