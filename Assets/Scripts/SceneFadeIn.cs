using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;
    public Color fadeColor = Color.black;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create image as child of canvas
        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(canvasObj.transform, false);

        Image fadeImage = imgObj.AddComponent<Image>();
        fadeImage.color = fadeColor;
        fadeImage.raycastTarget = false;

        RectTransform rect = imgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        // Fade from black to clear
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        Destroy(canvasObj);
        Destroy(gameObject);
    }
}