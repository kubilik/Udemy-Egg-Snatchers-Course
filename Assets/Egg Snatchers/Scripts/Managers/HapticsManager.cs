using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static void Light() => HapticFeedback.LightFeedback();
    public static void Medium() => HapticFeedback.MediumFeedback();
    public static void Heavy() => HapticFeedback.HeavyFeedback();
}
