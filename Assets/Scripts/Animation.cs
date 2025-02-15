using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private bool animationRunning = false;
    [SerializeField] private KeyCode startAnimationKey = KeyCode.G;
    [SerializeField] private KeyCode[] movementKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    [SerializeField] private float animationDelay = 1.0f;

    void Update()
    {
        if (Input.GetKeyDown(startAnimationKey) && !animationRunning)
        {
            StartAnimation();
        }

        if (animationRunning && IsPlayerMoving())
        {
            StopAnimation();
        }
    }

    void StartAnimation()
    {
        animationRunning = true;
        Debug.Log("Dance animation started!");
        StartCoroutine(PerformAnimation());
    }

    void StopAnimation()
    {
        animationRunning = false;
        Debug.Log("Dance animation stopped as player started moving.");
    }

    bool IsPlayerMoving()
    {
        foreach (KeyCode key in movementKeys)
        {
            if (Input.GetKey(key))
                return true;
        }
        return false;
    }

    IEnumerator PerformAnimation()
    {
        while (animationRunning)
        {
            Debug.Log("Performing dance moves...");
            yield return new WaitForSeconds(animationDelay);
        }

        Debug.Log("Dance animation finished!");
    }
}