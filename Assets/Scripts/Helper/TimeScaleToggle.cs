using UnityEngine;

public class TimeScaleToggle : ScriptableObject
{
    private static float nextScale = 0;

    private static float prevScale;

    public static bool IsTimePaused => Time.timeScale == 0;

    public static void Toggle()
    {
        prevScale = Time.timeScale;
        Time.timeScale = nextScale;
        nextScale = prevScale;
    }
}
