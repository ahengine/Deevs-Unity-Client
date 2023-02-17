using UnityEngine;

public static class FloatHelper 
{
    public static float Distance(float a, float b) =>
        Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));

    public static bool InBetween(float value,Vector2 range) => value > range.x && value < range.y;
    public static bool InBetween(float value, float a, float b)
    {
        float min = a < b ? a : b;
        float max = min == b ? a : b;

        return InBetween(value, new Vector2(min, max));
    }

    public static bool TargetIsFrontOfSelf(int sign, float self,float target) =>
        (sign > 0 && self < target) ||
        (sign < 0 && self > target);
}
