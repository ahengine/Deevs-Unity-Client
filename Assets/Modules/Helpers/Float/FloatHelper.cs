using UnityEngine;

public static class FloatHelper 
{
    public static float Distance(float a, float b) =>
        Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));

    public static bool TargetIsFrontOfSelf(int selfDirection, float self,float target) =>
        (selfDirection > 0 && self < target) ||
        (selfDirection < 0 && self > target);
}
