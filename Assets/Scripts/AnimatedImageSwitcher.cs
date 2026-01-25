using UnityEngine;
using System.Collections.Generic;

public class AnimatedImageSwitcher : MonoBehaviour
{

    [Header("Idle State Animation")]
    [SerializeField] private AnimatedImage idleAnimation;
    [Header("State Names")]
    [Tooltip("Must be the exact names from the animation controller")]
    [SerializeField] private string[] stateNames;
    [Header("State Animations")]
    [Tooltip("Animations mapped to the states. Must have the same length as State Names")]
    [SerializeField] private AnimatedImage[] stateAnimations;

    private Dictionary<string, AnimatedImage> stateToImageMapping;
    private string currentState, nextState;
    private readonly string IDLE_STATE = "Idle", FINAL_STATE = "Die";

    void Start()
    {
        currentState = IDLE_STATE;
        nextState = IDLE_STATE;
        stateToImageMapping = new Dictionary<string, AnimatedImage>();
        if(stateNames.Length != stateAnimations.Length)
        {
            Debug.LogError("State names list length does not match Animation list length!");
            return;
        }
        for(int i=0; i<stateNames.Length; i++)
        {
            stateToImageMapping.Add(stateNames[i], stateAnimations[i]);
        }
    }

    void Update()
    {
        if (currentState.Equals(IDLE_STATE))
        {
            // we are idling, check if another animation should be started
            if (!nextState.Equals(IDLE_STATE))
            {
                currentState = nextState;
                SwitchAnimations(idleAnimation, stateToImageMapping[nextState]);
            }
        }
        else if(!currentState.Equals(FINAL_STATE)) // Do nothing in final state
        {
            // currently in a state animation
            // check if finished and go to idle
            AnimatedImage currentAnim = stateToImageMapping[currentState];
            if (currentAnim.isFinished())
            {
                currentState = IDLE_STATE;
                nextState = IDLE_STATE;
                SwitchAnimations(currentAnim, idleAnimation);
            }
        }
    }

    private void SwitchAnimations(AnimatedImage current, AnimatedImage next)
    {
        current.Stopp();
        current.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        next.Restart();
    }

    public void EnterState(AnimatorStateInfo stateInfo)
    {
        foreach(string key in stateToImageMapping.Keys)
        {
            if (stateInfo.IsName(key))
            {
                nextState = key;
                return;
            }
        }
    }
}
