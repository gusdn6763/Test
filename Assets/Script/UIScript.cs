using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIScript : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void OpenClose(bool on)
    {
        canvasGroup.alpha = on ? 1 : 0;
        canvasGroup.blocksRaycasts = on ? true : false;
    }
}