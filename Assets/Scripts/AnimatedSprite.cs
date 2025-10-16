using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public float frameTime = 0.1f;
    public SpriteRenderer sr;
    public Sprite[] sprites;

    private float t;
    private int idx;


    void Start()
    {
        t = 0;
        idx = 0;
    }


    void Update()
    {
        t += Time.deltaTime;
        if (t > frameTime)
        {
            t = 0;
            NextFrame();
        }
    }

    void NextFrame()
    {
        idx++;
        if (idx >= sprites.Length)
            idx = 0;
        sr.sprite = sprites[idx];
    }
}
