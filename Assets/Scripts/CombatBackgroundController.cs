using UnityEngine;
using UnityEngine.UI;

public class CombatBackgroundController : MonoBehaviour
{
    public Sprite[] backgrounds;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetBackground(int idx)
    {
        if (idx >= 0 && idx < backgrounds.Length)
            sr.sprite = backgrounds[idx];
    }
    public void SetBackground(Stage stage)
    {
        int idx;
        if (stage.name.Equals("Training"))
            idx = 0;
        else
            idx = Random.Range(1, backgrounds.Length);
        SetBackground(idx);
    }
}
