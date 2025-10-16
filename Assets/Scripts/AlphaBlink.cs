using UnityEngine;
using UnityEngine.UI;

public class AlphaBlink : MonoBehaviour
{
    public float minAlpha = 0.2f, maxAlpha = 1.0f, increment = 2.0f;

    private SpriteRenderer sr;
    private Image img;
    private float currentAlpha;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
        currentAlpha = minAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        currentAlpha += increment * Time.deltaTime;
        if ((increment > 0 && currentAlpha > maxAlpha)
            || (increment < 0 && currentAlpha < minAlpha))
        {
            increment = -1 * increment;
        }
        if (img != null)
        {
            Color col = img.color;
            col.a = currentAlpha;
            img.color = col;
        }
        else if(sr != null)
        {
            Color col = sr.color;
            col.a = currentAlpha;
            sr.color = col;
        }
    }
}
