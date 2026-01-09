using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupController : MonoBehaviour
{
    public float displayTime = 0.2f, fadeDuration = 0.3f;
    public bool animatePosition = true;

    private Text number;
    private CanvasGroup canvasGroup;
    private int value;
    public int Value
    {
        set
        {
            this.value = value;
            if (number)
            {
                if (value == 0)
                    number.text = "Miss";
                else
                    number.text = value.ToString();
            }
        }
    }

    private Color textcolor;
    public Color Textcolor
    {
        set
        {
            textcolor = value;
            if (number)
            {
                number.color = textcolor;
            }
        }
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        number = GetComponent<Text>();
        if (number)
        {
            number.color = textcolor;
            if (number)
            {
                if (value == 0)
                    number.text = "Miss";
                else
                    number.text = value.ToString();
            }
        }
        StartCoroutine(FadeInOut(fadeDuration));
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
            if (animatePosition)
                transform.localPosition = Vector3.Lerp(starPosition, middlePosition, normalized);
            yield return null;
        }

        // stay a bit
        yield return new WaitForSeconds(displayTime);

        // fade out
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, normalized);
            if (animatePosition)
                transform.localPosition = Vector3.Lerp(middlePosition, endPosition, normalized);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
