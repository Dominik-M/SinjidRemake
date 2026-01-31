
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndScreenController : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private CanvasGroup buttons;
    [SerializeField] private AnimatedImage outro;
    [SerializeField] private GameObject conclusionScreen;

    [Header("Parameters")]
    [SerializeField] private float outrolength = 5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool outroFinished = false;

    public override void OnEnable()
    {
        base.OnEnable();
        buttons.alpha = 0;
        conclusionScreen.SetActive(true);
    }

    public void OnProceed()
    {
        conclusionScreen.SetActive(false);
        StartCoroutine(PlayOutro());
    }

    private IEnumerator PlayOutro()
    {
        // play outro
        outroFinished = false;
        outro.Restart();
        GetComponent<AudioSource>().Play();
        // wait for outro
        yield return new WaitForSeconds(outrolength);
        // fade in buttons
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / fadeDuration;
            buttons.alpha = Mathf.Lerp(0, 1, normalized);
            yield return null;
        }
        outroFinished = true;
    }

    public void OnBack()
    {
        if (outroFinished)
        {
            GameController.CloseMenu();
            GameController.Instance.GetComponent<AudioSource>().Play();
        }
    }

    public void OnNGPlus()
    {
        if (outroFinished)
        {
            GameController.CloseMenu();
            GameController.Instance.GetComponent<AudioSource>().Play();
        }

    }

    public void OnMenu()
    {
        if (outroFinished)
        {
            GameController.LoadMainMenuScene();
        }
    }
}

