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
        
    }

    private void OnDisable()
    {
        
    }

    void OnNewTurn()
    {
        
    }

    void EnablePlayerCombat()
    {
        
    }

    void DisablePlayerCombat()
    {
        
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
    }

    void SelectionCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray,out hit, 999, selectionLayerMask))
        {
            Character character = hit.collider.GetComponent<Character>();

            if (curSelectedCharacter != null && curSelectedCharacter == character)
            {
                return;
            }

            if (canSelectSelf && character == TurnManager.instance.GetCurrentTurnCharacter())
            {
                SelectCharacter(character);
            }
            else if (canSelectTeam && character.team == Character.Team.Player)
            {
                SelectCharacter(character);
            }
            else if (canSelectEnemies && character.team == Character.Team.Enemy)
            {
                SelectCharacter(character);
            }
        }
        
        UnselectCharacter();
    }

    void CastCombatAction()
    {
        
    }

    void NextTurnDelay()
    {
        
    }

    void SelectCharacter(Character character)
    {
        UnselectCharacter();
        curSelectedCharacter = character;
    }

    void UnselectCharacter()
    {
        
    }

    public void SetCurrentCombatAction(CombatAction combatAction)
    {
        
    }
}
