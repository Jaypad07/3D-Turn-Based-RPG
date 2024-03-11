using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class EnemyCombatManager : MonoBehaviour
{
    [Header("AI")]
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.0f;

    [Header("Attacking")]
    public float attackWeakestChance = 0.7f;

    [Header("Chance Curves")]
    public AnimationCurve healChanceCurve;

    private Character curEnemy;

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
        if (TurnManager.instance.GetCurrentTurnCharacter().team == Character.Team.Enemy)
        {
            curEnemy = TurnManager.instance.GetCurrentTurnCharacter();
            Invoke(nameof(DecideCombatAction), Random.Range(minWaitTime, maxWaitTime));
        }
    }

    void DecideCombatAction()
    {
        // Do we need to heal ourself or teammate
        if (HasCombatActionOfType(typeof(HealCombatAction)))
        {
            Character weakestEnemy = GetWeakestCharacter(Character.Team.Enemy);

            if (Random.value < healChanceCurve.Evaluate(GetHealthPercentage(weakestEnemy)))
            {
                CastCombatAction(GetHealCombatAction(), weakestEnemy);
                return;
            }
        }

        // Deal damage to a player character.
        Character playerToDamage;

        if (Random.value < attackWeakestChance)
            playerToDamage = GetWeakestCharacter(Character.Team.Player);
        else
            playerToDamage = GetRandomCharacter(Character.Team.Player);

        if (playerToDamage != null)
        {
            if (HasCombatActionOfType(typeof(MeleeCombatAction)) || HasCombatActionOfType(typeof(RangedCombatAction)))
            {
                CastCombatAction(GetDamageCombatAction(), playerToDamage);
                return;
            }
        }

        Invoke(nameof(EndTurn), Random.Range(minWaitTime, maxWaitTime));
    }

    void CastCombatAction(CombatAction combatAction, Character target)
    {
        if (curEnemy == null)
        {
            EndTurn();
            return;
        }
        
        curEnemy.CastCombatAction(combatAction, target);
        Invoke(nameof(EndTurn), Random.Range(minWaitTime, maxWaitTime));
    }

    void EndTurn()
    {
        TurnManager.instance.EndTurn();
    }

    float GetHealthPercentage(Character character)
    {
        return (float)character.curHp / character.maxHp;
    }

    bool HasCombatActionOfType(System.Type type)
    {
        foreach (CombatAction ca in curEnemy.combatActions)
        {
            if (ca.GetType() == type)
            {
                return true;
            }
        }

        return false;
    }

    CombatAction GetDamageCombatAction()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(MeleeCombatAction) || x.GetType() == typeof(RangedCombatAction)).ToArray();

        if (ca == null || ca.Length == 0)
        {
            return null;
        }

        return ca[Random.Range(0, ca.Length)];
    }

    CombatAction GetHealCombatAction()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(HealCombatAction)).ToArray();

        if (ca == null || ca.Length == 0)
        {
            return null;
        }

        return ca[Random.Range(0, ca.Length)];
    }

    CombatAction GetEffectCombatAction()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(EffectCombatAction)).ToArray();

        if (ca == null || ca.Length == 0)
        {
            return null;
        }

        return ca[Random.Range(0, ca.Length)];
    }

    Character GetWeakestCharacter(Character.Team team)
    {
        int weakestHp = 999;
        int weakestIndex = 0;

        Character[] characters = team == Character.Team.Player ? GameManager.Instance.playerTeam : GameManager.Instance.enemyTeam;

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null)
            {
                continue;
            }

            if (characters[i].curHp < weakestHp)
            {
                weakestHp = characters[i].curHp;
                weakestIndex = i;
            }
        }

        return characters[weakestIndex];
    }

    Character GetRandomCharacter(Character.Team team)
    {
        Character[] characters = null;

        if (team == Character.Team.Player)
        {
            characters = GameManager.Instance.playerTeam.Where(x => x != null).ToArray();
        }
        else if (team == Character.Team.Enemy)
        {
            characters = GameManager.Instance.enemyTeam.Where(x => x != null).ToArray();
        }

        return characters[Random.Range(0, characters.Length)];
    }
}