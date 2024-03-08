using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum  Team
    {
        Player,
        Enemy
    }

    [Header("Stats")]
    public Team team;
    public string displayName;
    public int curHp;
    public int maxHp;

    [Header("Combat Actions")]
    public CombatAction[] combatActions;

    [Header("Components")]
    public CharacterEffects characterEffects;
    public CharacterUI characterUI;
    public GameObject selectionVisual;
    public DamageFlash damageFlash;

    [Header("Prefabs")]
    public GameObject healParticlePrefab;

    private Vector3 _standingPosition;

    private void OnEnable()
    {
        TurnManager.instance.onNewTurn += OnNewTurn;
    }

    private void OnDisable()
    {
        TurnManager.instance.onNewTurn -= OnNewTurn;
    }

    void OnNewTurn()
    {
        characterUI.ToggleTurnVisual(TurnManager.instance.GetCurrentTurnCharacter() == this);
    }
}
