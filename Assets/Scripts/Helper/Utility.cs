using System.Linq;
using UnityEngine;

public class Utility : MonoBehaviour
{
    // If needed, we could add overloads that take T[] instead of T
    public static void DisableAllOf<T>(T except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except).ToArray();

        foreach (var obj in objects)
            obj.enabled = false;
    }

    public static void EnableAllOf<T>(T except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except).ToArray();

        foreach (var obj in objects)
            obj.enabled = true;
    }
}

