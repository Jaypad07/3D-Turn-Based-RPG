using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : MonoBehaviour
{
    public float selectionCheckRate = 0.02f;
    private float lastSelectionCheckTime;
    public LayerMask selectionLayerMask;

    private bool isActive;

    private CombatAction curSelectionCombatAction;
    private Character curSelectedCharacter;
    
    // Selection Flags
    private bool canSelectSelf;
    private bool canSelectTeam;
    private bool canSelectEnemies;
    
    // Singleton
    public static PlayerCombatManager Instance;

    [Header("Components")]
    public CombatActionsUI combatActionsUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

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
        if (TurnManager.instance.GetCurrentTurnCharacter().team == Character.Team.Player)
        {
            EnablePlayerCombat();
        }
        else
        {
            DisablePlayerCombat();
        }
    }

    void EnablePlayerCombat()
    {
        curSelectedCharacter = null;
        curSelectionCombatAction = null;
        isActive = true;
    }

    void DisablePlayerCombat()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive || curSelectionCombatAction == null)
        {
            return;
        }

        if (Time.time - lastSelectionCheckTime > selectionCheckRate)
        {
            lastSelectionCheckTime = Time.time;
            SelectionCheck();
        }

        if (Mouse.current.leftButton.isPressed && curSelectedCharacter != null)
        {
            CastCombatAction();
        }
    }

    void SelectionCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 999, selectionLayerMask))
        {
            Character character = hit.collider.GetComponent<Character>();

            if (curSelectedCharacter != null && curSelectedCharacter == character)
            {
                return;
            }

            if (canSelectSelf && character == TurnManager.instance.GetCurrentTurnCharacter())
            {
                SelectCharacter(character);
                return;
            }
            else if (canSelectTeam && character.team == Character.Team.Player)
            {
                SelectCharacter(character);
                return;
            }
            else if (canSelectEnemies && character.team == Character.Team.Enemy)
            {
                SelectCharacter(character);
                return;
            }
        }
        
        UnselectCharacter();
    }

    void CastCombatAction()
    {
        TurnManager.instance.GetCurrentTurnCharacter().CastCombatAction(curSelectionCombatAction, curSelectedCharacter);
        curSelectionCombatAction = null;
        
        UnselectCharacter();
        DisablePlayerCombat();
        combatActionsUI.DisableCombatActions();
        TurnManager.instance.endTurnButton.SetActive(false);
        
        Invoke(nameof(NextTurnDelay), 1.0f);
    }

    void NextTurnDelay()
    {
        TurnManager.instance.EndTurn();
    }

    void SelectCharacter(Character character)
    {
        UnselectCharacter();
        curSelectedCharacter = character;
        character.ToggleSelectionVisual(true);
    }

    void UnselectCharacter()
    {
        if (curSelectedCharacter == null)
        {
            return;
        }
        curSelectedCharacter.ToggleSelectionVisual(false);
        curSelectedCharacter = null;
    }

    public void SetCurrentCombatAction(CombatAction combatAction)
    {
        curSelectionCombatAction = combatAction;

        canSelectSelf = false;
        canSelectTeam = false;
        canSelectEnemies = false;

        if (combatAction as MeleeCombatAction || combatAction as RangedCombatAction) // This checks if its type is of MeleeCombatAction, which is a sub class of CombatAction.
        {
            canSelectEnemies = true;
        }
        else if (combatAction as HealCombatAction)
        {
            canSelectSelf = true;
            canSelectTeam = true;
        }
        else if (combatAction as EffectCombatAction)
        {
            canSelectSelf = (combatAction as EffectCombatAction).canEffectSelf;
            canSelectTeam = (combatAction as EffectCombatAction).canEffectTeam;
            canSelectEnemies = (combatAction as EffectCombatAction).canEffectEnemy;

        }
    }
}
