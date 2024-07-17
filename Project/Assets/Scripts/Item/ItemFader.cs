using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;



[RequireComponent (typeof(Tilemap))]
public class ItemFader : MonoBehaviour
{

    private Tilemap tilemap;

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    public void FadeIn()
    {
        Color currentColor = tilemap.color;
        float currentAlpha = currentColor.a;

        DOTween.To(() => currentAlpha, x =>
        {
            currentColor.a = x;
            tilemap.color = currentColor;
        }, 1f, Settings.itemFadeDuration);
    }

    public void FadeOut()
    {
        Color currentColor = tilemap.color;
        float currentAlpha = currentColor.a;

        DOTween.To(() => currentAlpha, x =>
        {
            currentColor.a = x;
            tilemap.color = currentColor;
        }, Settings.targetAlpha, Settings.itemFadeDuration);
    }
}
