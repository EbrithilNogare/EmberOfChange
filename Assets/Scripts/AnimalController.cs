using DG.Tweening;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public Sprite chillSprite;
    public Sprite panicSprite;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PanicMode()
    {
        if (panicSprite != null)
        {
            spriteRenderer.sprite = panicSprite;
        }
    }

    public void ChillMode()
    {
        if (chillSprite != null)
        {
            spriteRenderer.sprite = chillSprite;
        }
    }

    public void FlyTo(Vector3 destination, float duration)
    {
        Sequence flySequence = DOTween.Sequence();
        flySequence.Append(transform.DOMove(destination, duration).SetEase(Ease.OutBounce));
        flySequence.Join(transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360));
    }
}
