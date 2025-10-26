using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumberDisplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text number;

    public void Show(int value, Color textcolor)
    {
        if (value == 0)
            number.text = "Miss";
        else
            number.text = value.ToString();
        number.color = textcolor;
        StartCoroutine(FadeInOut(0.3f));
    }

    private IEnumerator FadeInOut(float duration)
    {
        Vector3 starPosition = transform.position + new Vector3(0, 0, 0);
        Vector3 middlePosition = transform.position + new Vector3(0, 1f, 0);
        Vector3 endPosition = transform.position + new Vector3(0, 1.5f, 0);
        // fade in
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            canvasGroup.alpha = Mathf.Lerp(0, 1, normalized);
            transform.localPosition = Vector3.Lerp(starPosition, middlePosition, normalized);
            yield return null;
        }

        // stay a bit
        yield return new WaitForSeconds(0.2f);

        // fade out
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, normalized);
            transform.localPosition = Vector3.Lerp(middlePosition, endPosition, normalized);
            yield return null;
        }
    }
}
