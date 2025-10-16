using UnityEngine;

public class InteractionHighlightController : MonoBehaviour
{
    public InteractionID interaction = InteractionID.NONE;
    public float minAlpha = 0.2f, maxAlpha = 1.0f, increment = 2.0f;

    private SpriteRenderer sr;
    private bool active = false;
    private float currentAlpha;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        currentAlpha = minAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            currentAlpha += increment * Time.deltaTime;
            if ((increment > 0 && currentAlpha > maxAlpha)
                || (increment < 0 && currentAlpha < minAlpha))
            {
                increment = -1 * increment;
            }
            Color col = sr.color;
            col.a = currentAlpha;
            sr.color = col;
        }
        else
        {
            Color col = sr.color;
            col.a = 0;
            sr.color = col;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            active = true;
            GameController.CurrentInteraction = interaction;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            active = false;
            GameController.CurrentInteraction = InteractionID.NONE;
        }
    }
}
