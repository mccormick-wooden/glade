using System;

public static class Extensions
{
    public static T Next<T>(this T src) where T : Enum
    {
        T[] enumArray = (T[])Enum.GetValues(src.GetType());
        return enumArray[Array.IndexOf(enumArray, src) + 1 % enumArray.Length];
    }
}
