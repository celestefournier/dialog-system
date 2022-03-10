using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ArrowVfx : MonoBehaviour
{
    Image image;
    RectTransform rectTransform;
    float duration = 0.2f;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        rectTransform
            .DOAnchorPosY(rectTransform.anchoredPosition.y + 15, duration)
            .SetEase(Ease.OutSine)
            .From();
        image.DOFade(0, duration).SetEase(Ease.Linear).From();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
