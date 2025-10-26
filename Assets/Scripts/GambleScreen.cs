using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GambleScreen : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private Text gold;
    [SerializeField] private RectTransform cup1, cup2, cup3;
    [SerializeField] private RectTransform cup1cup, cup2cup, cup3cup;
    [SerializeField] private float switchDuration=0.1f;
    [SerializeField] private int numSwitches = 10;

    private Vector3 positionUp = new Vector3(0, 20, 0), positionDown = new Vector3(0, 0, 0);
    private bool isMixing = false, gameStarted = false;

    void Start()
    {
        gold.text = "Gold: " + GameController.Gold;
        isMixing = false;
        gameStarted = false;
    }

    public void OnStartClicked()
    {
        if (!gameStarted)
        {
            AddGold(-20);
            Mix();
            isMixing = true;
            gameStarted = true;
        }
    }

    public void OnLooserCupClicked()
    {
        if (!isMixing && gameStarted)
        {
            ShowAllCups();
            gameStarted = false;
        }
    }
    public void OnWinnerCupClicked()
    {
        if (!isMixing && gameStarted)
        {
            ShowAllCups();
            AddGold(50);
            gameStarted = false;
        }
    }

    private void AddGold(int g)
    {
        StartCoroutine(AddGoldRoutine(g));
    }

    private void ShowAllCups()
    {
        StartCoroutine(ShowRoutine(0.8f));
    }

    private void Mix()
    {
        StartCoroutine(MixRoutine());
    }

    private IEnumerator MixRoutine()
    {
        // First close ups
        yield return HideRoutine(0.5f);
        // Now random switches
        for(int i=0; i<numSwitches; i++)
        {
            int n = Random.Range(0, 3);
            if(n==0)
                yield return SwitchCupsRoutine(cup1, cup2, switchDuration);
            if (n == 1)
                yield return SwitchCupsRoutine(cup2, cup3, switchDuration);
            if (n == 2)
                yield return SwitchCupsRoutine(cup1, cup3, switchDuration);
        }
        isMixing = false;
    }
    private IEnumerator SwitchCupsRoutine(RectTransform r1, RectTransform r2, float duration)
    {
        Vector3 pos1 = r1.localPosition;
        Vector3 pos2 = r2.localPosition;
        Vector3 posCenter1 = ((pos1+pos2)/2) + new Vector3(0,30,0);
        Vector3 posCenter2 = ((pos1 + pos2) / 2) + new Vector3(0, -30, 0);

        float t = 0f;
        // first move to center
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            r1.localPosition = Vector3.Lerp(pos1, posCenter1, normalized);
            r2.localPosition = Vector3.Lerp(pos2, posCenter2, normalized);
            yield return null;
        }
        // now move to final position
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            r1.localPosition = Vector3.Lerp(posCenter1, pos2, normalized);
            r2.localPosition = Vector3.Lerp(posCenter2, pos1, normalized);
            yield return null;
        }
    }

    private IEnumerator ShowRoutine(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            cup1cup.localPosition = Vector3.Lerp(positionDown, positionUp, normalized);
            cup2cup.localPosition = Vector3.Lerp(positionDown, positionUp, normalized);
            cup3cup.localPosition = Vector3.Lerp(positionDown, positionUp, normalized);
            yield return null;
        }
    }
    private IEnumerator HideRoutine(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            cup1cup.localPosition = Vector3.Lerp(positionUp, positionDown, normalized);
            cup2cup.localPosition = Vector3.Lerp(positionUp, positionDown, normalized);
            cup3cup.localPosition = Vector3.Lerp(positionUp, positionDown, normalized);
            yield return null;
        }
    }
    private IEnumerator AddGoldRoutine(int n)
    {
        for (int i = 0; i < Mathf.Abs(n); i++)
        {
            GameController.Gold += ((n > 0) ? 1 : -1);
            gold.text = "Gold: " + GameController.Gold;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
