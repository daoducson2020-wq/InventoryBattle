using UnityEngine;

public class SellPoint : MonoBehaviour
{
    [SerializeField] private RectTransform sellArea;
    [SerializeField] private Animator animator;

    // private void Awake()
    // {
    //     Time.timeScale = 1f;
    //     if (animator != null)
    //     {
    //         animator.Rebind();
    //         animator.Update(0f);
    //     }
    // }

    // public void Start()
    // {
    //     if (animator != null)
    //     {
    //         // Ví dụ nếu bạn dùng Bool để điều khiển animation bán hàng
    //         animator.SetBool("isBurn", false);

    //         // Hoặc nếu dùng Trigger, hãy chủ động reset nó
    //         animator.ResetTrigger("Sell");
    //     }
    // }

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
        if (animator != null)
        {
            animator.SetBool("isBurn", false);
        }
    }
}
