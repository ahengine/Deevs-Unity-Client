using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

public static class CoroutineHelper 
{
    public delegate bool BoolFunc();

    public static IEnumerator CallActionWithDelay(Action action, float delay, bool isTimeScaled = true)
    {
        if (isTimeScaled)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForSecondsRealtime(delay);

        if (action != null)
            action.Invoke();
    }

    public static IEnumerator WaitToCallAction(Action action, BoolFunc waitingForThis)
    {
        while (!waitingForThis())
            yield return new WaitForEndOfFrame();

        if (action != null)
            action.Invoke();
    }
    public static IEnumerator WaitToCallAction(Action action, AsyncOperation waitingForThis)
    {
        yield return waitingForThis;

        if (action != null)
            action.Invoke();
    }
}
