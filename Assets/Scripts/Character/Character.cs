using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public enum Team
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

    private void Start()
    {
        _standingPosition = transform.position;
        characterUI.SetCharacterNameText(displayName);
        characterUI.UpdateHealthBar(curHp, maxHp);
    }

    void OnNewTurn()
    {
        characterUI.ToggleTurnVisual(TurnManager.instance.GetCurrentTurnCharacter() == this);
        characterEffects.ApplyCurrentEffects();
    }

    public void CastCombatAction(CombatAction combatAction, Character target = null)
    {
        if (target == null)
            target = this;
        
        combatAction.Cast(this, target);
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;
        
        characterUI.UpdateHealthBar(curHp, maxHp);

        if (curHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        curHp += amount;

        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        
        characterUI.UpdateHealthBar(curHp, maxHp);
        Instantiate(healParticlePrefab, transform);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void MovetoTarget(Character target, UnityAction<Character> arriveCallback)
    {
        StartCoroutine(MeleeAttackAnimation());

        IEnumerator MeleeAttackAnimation()
        {
            while (transform.position != target.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
                yield return null;
            }

            arriveCallback?.Invoke(target);

            while (transform.position != _standingPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _standingPosition, 10 * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void ToggleSelectionVisual(bool toggle)
    {
        selectionVisual.SetActive(toggle);
    }
}