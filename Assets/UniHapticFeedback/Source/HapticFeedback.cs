using UnityEngine;
using System.Runtime.InteropServices;

public class HapticFeedback : MonoBehaviour
{
    public enum HapticForce { Light, Medium, Heavy }
    public enum NotificationType { Error, Success, Warning }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void doSelectionHaptic();

    [DllImport("__Internal")]
    private static extern void doImapctHaptic(HapticForce force);

    [DllImport("__Internal")]
    private static extern void doNotificationHaptic(NotificationType type);

    [DllImport("__Internal")]
    private static extern void fallbackHapticNope();
#endif

    public void DoNotificationHapticError()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(NotificationType.Error);
#else
        Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoNotificationHapticSuccess()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(NotificationType.Success);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoNotificationHapticWarning()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(NotificationType.Warning);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoSelectionHaptic()
    {
#if UNITY_IOS && !UNITY_EDITOR
        doSelectionHaptic();
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoLightImapactHaptic()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(HapticForce.Light);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoMediumImapactHaptic()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(HapticForce.Medium);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public void DoHeavyImapactHaptic()
    {
#if UNITY_IOS && !UNITY_EDITOR
        DoHaptic(HapticForce.Heavy);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    // Static Methods 
    // These for if you want to use the Haptics without instancing the class

    public static void DoHaptic()
    {
#if UNITY_IOS && !UNITY_EDITOR
        doSelectionHaptic();
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public static void DoHaptic(HapticForce type)
    {
#if UNITY_IOS && !UNITY_EDITOR
        doImapctHaptic(type);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public static void DoHaptic(NotificationType type)
    {
#if UNITY_IOS && !UNITY_EDITOR
        doNotificationHaptic(type);
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif
    }

    public static void DoFallbackHapticNope() {
#if UNITY_IOS && !UNITY_EDITOR
        fallbackHapticNope();
#else
		Debug.Log("HapticFeedback is not support on this platform");
#endif

    }
}