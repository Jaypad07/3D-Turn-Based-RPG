using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatManager : MonoBehaviour
{
    [Header("AI")]
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.0f;

    [Header("Attacking")]
    public float attackWeakestChance = 0.7f;

    [Header("Chance Curves")]
    public AnimationCurve healChanceCurve;
    
}
