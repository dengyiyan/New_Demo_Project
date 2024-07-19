using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;



[RequireComponent (typeof(Tilemap))]
public class ItemFader : MonoBehaviour
{
    private int activeTriggers = 0;
    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void FadeIn()
    {
        if (activeTriggers <= 0) // Only fade in if no active triggers
        {
            Color currentColor = tilemap.color;
            float currentAlpha = currentColor.a;

            DOTween.To(() => currentAlpha, x =>
            {
                currentColor.a = x;
                tilemap.color = currentColor;
            }, 1f, Settings.itemFadeDuration).SetEase(Ease.InOutQuad);
        }
    }

    public void FadeOut()
    {
        Color currentColor = tilemap.color;
        float currentAlpha = currentColor.a;

        DOTween.To(() => currentAlpha, x =>
        {
            currentColor.a = x;
            tilemap.color = currentColor;
        }, Settings.targetAlpha, Settings.itemFadeDuration).SetEase(Ease.InOutQuad);
    }

    public void IncrementTriggerCount()
    {
        activeTriggers++;
        FadeOut();
    }

    public void DecrementTriggerCount()
    {
        activeTriggers--;
        if (activeTriggers <= 0)
        {
            FadeIn();
        }
    }
}
