using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatueHoverLight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("1. Highlight")]
    [SerializeField] private Image highlightImage;
    [SerializeField][Range(0f, 1f)] private float maxLightAlpha = 0.85f;

    [Header("2. Panel")]
    [SerializeField] private RectTransform targetPanel;
    [SerializeField] private bool disablePanelRaycast = true;

    [Header("3. Time Setting")]
    [SerializeField] private float duration = 0.3f;

    private CanvasGroup panelCanvasGroup;
    private Vector3 originalPanelScale = Vector3.one;
    private Coroutine animCoroutine;

    private void Awake()
    {
        if (highlightImage != null)
        {
            highlightImage.raycastTarget = false;
            highlightImage.fillAmount = 0f;
            SetImageAlpha(highlightImage, 0f);
        }

        if (targetPanel != null)
        {
            originalPanelScale = targetPanel.localScale;
            if (originalPanelScale == Vector3.zero) originalPanelScale = Vector3.one;

            targetPanel.pivot = new Vector2(0.5f, 0.5f);

            panelCanvasGroup = targetPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = targetPanel.gameObject.AddComponent<CanvasGroup>();
            }

            if (disablePanelRaycast)
            {
                panelCanvasGroup.blocksRaycasts = false;
                panelCanvasGroup.interactable = false;
            }

            targetPanel.localScale = GetCollapsedScale();
            panelCanvasGroup.alpha = 0f;
            targetPanel.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHoverAnimation(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartHoverAnimation(false);
    }

    private void StartHoverAnimation(bool isHover)
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateHover(isHover));
    }

    private IEnumerator AnimateHover(bool isHover)
    {
        if (isHover && targetPanel != null && !targetPanel.gameObject.activeSelf)
        {
            targetPanel.gameObject.SetActive(true);
        }

        float startFill = highlightImage != null ? highlightImage.fillAmount : 0f;
        float targetFill = isHover ? 1f : 0f;

        float startLightAlpha = highlightImage != null ? highlightImage.color.a : 0f;
        float targetLightAlpha = isHover ? maxLightAlpha : 0f;

        Vector3 startScale = targetPanel != null ? targetPanel.localScale : Vector3.zero;
        Vector3 targetScale = isHover ? originalPanelScale : GetCollapsedScale();

        float startPanelAlpha = panelCanvasGroup != null ? panelCanvasGroup.alpha : 0f;
        float targetPanelAlpha = isHover ? 1f : 0f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(time / duration);

            float t = Mathf.SmoothStep(0f, 1f, progress);

            if (highlightImage != null)
            {
                highlightImage.fillAmount = Mathf.Lerp(startFill, targetFill, t);
                SetImageAlpha(highlightImage, Mathf.Lerp(startLightAlpha, targetLightAlpha, t));
            }

            if (targetPanel != null)
            {
                targetPanel.localScale = Vector3.Lerp(startScale, targetScale, t);
                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = Mathf.Lerp(startPanelAlpha, targetPanelAlpha, t);
                }
            }

            yield return null;
        }

        if (highlightImage != null)
        {
            highlightImage.fillAmount = targetFill;
            SetImageAlpha(highlightImage, targetLightAlpha);
        }

        if (targetPanel != null)
        {
            targetPanel.localScale = targetScale;
            if (panelCanvasGroup != null) panelCanvasGroup.alpha = targetPanelAlpha;

            if (!isHover)
            {
                targetPanel.gameObject.SetActive(false);
            }
        }
    }

    private Vector3 GetCollapsedScale()
    {
        return new Vector3(0f, originalPanelScale.y, originalPanelScale.z);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}