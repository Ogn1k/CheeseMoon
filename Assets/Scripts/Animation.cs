using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private bool animationRunning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !animationRunning)
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
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    IEnumerator PerformAnimation()
    {
        while (animationRunning)
        {
            Debug.Log("Performing dance moves..."); // Заглушка для анимации

            yield return new WaitForSeconds(1.0f); // Wait for 1 second
        }

        Debug.Log("Dance animation finished!");
    }
}