using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Utility : MonoBehaviour
{
    /// <summary>
    /// Disables all objects of T type. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except">Optionally, caller may pass a T that they wish to be excluded from Disable.</param>
    public static void DisableAllOf<T>(T except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except).ToArray();

        foreach (var obj in objects)
            obj.enabled = false;
    }

    /// <summary>
    /// Disables all objects of T type. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except">Optionally, caller may pass a T[] that they wish to be excluded from Disable.</param>
    public static void DisableAllOf<T>(T[] except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except.Any(e => e == o)).ToArray();

        foreach (var obj in objects)
            obj.enabled = false;
    }

    /// <summary>
    /// Enables all objects of T type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except">Optionally, caller may pass a reference that they wish to be excluded from Enable.</param>
    public static void EnableAllOf<T>(T except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except).ToArray();

        foreach (var obj in objects)
            obj.enabled = true;
    }

    /// <summary>
    /// Enables all objects of T type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except">Optionally, caller may pass a reference that they wish to be excluded from Enable.</param>
    public static void EnableAllOf<T>(T[] except = default) where T : Behaviour
    {
        var objects = FindObjectsOfType<T>();

        if (except != default)
            objects = objects.Where(o => o != except.Any(e => e == o)).ToArray();

        foreach (var obj in objects)
            obj.enabled = true;
    }

    /// <summary>
    /// Logs an error if the reference is null (more helpful than NullReferenceExceptions).
    /// </summary>
    /// <param name="checkReference">
    /// The reference to check.
    /// </param>
    /// <param name="checkReferenceName">
    /// A helpful name to log if reference is null. Usually this can just be passed as nameof(checkReference) from calling context,
    /// but occasionally it may be helpful to pass a different / more descriptive name if the calling context name is not specific enough.
    /// </param>
    /// <param name="optionalInfo">Optional context information to include in the log.</param>
    public static void LogErrorIfNull<T>(T checkReference, string checkReferenceName, string optionalInfo = "")
    {
        if (checkReference == null)
        {
            var callStack = new StackFrame(skipFrames: 1, fNeedFileInfo: true);
            Debug.LogError($"{Path.GetFileName(callStack.GetFileName())}:{callStack.GetFileLineNumber()} - '{checkReferenceName}' is null. {optionalInfo}");
        }
    }

    /// <summary>
    /// Helper to add onClick callback to buttons.
    /// </summary>
    /// <param name="buttonRootName">The parent GameObject name for the button. If this isn't unique in scene u gon be mad.</param>
    /// <param name="lambda">The callback, an arrow function works.</param>
    public static void AddButtonCallback(string buttonRootName, UnityAction lambda)
    {
        var buttonRoot = GameObject.Find(buttonRootName);
        LogErrorIfNull(buttonRoot, nameof(buttonRootName));

        AddButtonCallback(buttonRoot.GetComponentInChildren<Button>(), lambda);
    }

    /// <summary>
    /// Helper to add onClick callback to buttons.
    /// </summary>
    /// <param name="button">button.</param>
    /// <param name="lambda">The callback, an arrow function works.</param>
    public static void AddButtonCallback(Button button, UnityAction lambda)
    {
        LogErrorIfNull(button, nameof(button));
        button.onClick.AddListener(lambda);
    }

    /// <summary>
    /// Helper to mimic in the runtime what clicking "Reset" on a transform in the editor while respecting the parent's transform
    /// </summary>
    /// <param name="transform">The transform to reset to local positions.</param>
    public static void ResetLocalTransform(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ClearButtonAllCallbacks(string buttonRootName)
    {
        ClearButtonAllCallbacks(GameObject.Find(buttonRootName)?.GetComponentInChildren<Button>());
    }

    public static void ClearButtonAllCallbacks(Button button)
    {
        LogErrorIfNull(button, nameof(button));
        button.onClick.RemoveAllListeners();
    }

    public static void ShowCursor()
    {
        if (Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public static void HideCursor()
    {
        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
