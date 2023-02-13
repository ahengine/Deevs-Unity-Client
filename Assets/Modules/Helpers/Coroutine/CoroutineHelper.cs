using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

public static class CoroutineHelper 
{
    public static IEnumerator CallAction(Action action, float delayCallAction, bool isTimeScaled = true)
    {
        if (isTimeScaled)
            yield return new WaitForSeconds(delayCallAction);
        else
            yield return new WaitForSecondsRealtime(delayCallAction);

        if (action == null)
            action.Invoke();
    }

    public delegate bool BoolFunc();

    public static IEnumerator WaitToCallAction(Action action, BoolFunc waitingForThis)
    {
        while(waitingForThis())
            yield return new WaitForEndOfFrame();

        if (action == null)
            action.Invoke();
    }
}
