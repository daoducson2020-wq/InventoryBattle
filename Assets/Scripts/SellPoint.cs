using Unity.VisualScripting;
using UnityEngine;

public class SellPoint : MonoBehaviour
{
    [SerializeField] private RectTransform sellArea;
    [SerializeField] private Animator animator;

    public bool IsPointerOverSellPoint(Vector2 screenPosition)
    {
        RectTransform targetRect = sellArea != null ? sellArea : GetComponent<RectTransform>();
        if (targetRect == null)
        {
            return false;
        }

        Canvas canvas = targetRect.GetComponentInParent<Canvas>();
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(
            targetRect,
            screenPosition,
            eventCamera);
    }

    public void EndBurnAnimation()
    {
        if(animator != null)
        {
            animator.SetBool("isBurn", false);
        }
    }
}
