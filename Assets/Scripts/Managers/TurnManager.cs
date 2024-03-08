using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    private List<Character> turnOrder = new List<Character>();
    private int curTurnOrderIndex;
    private Character curTurnCharacter;

    [Header("Compoents")]
    public GameObject endTurnButton;
    
    //Singleton
    public static TurnManager instance;

    public event UnityAction onNewTurn;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Begin()
    {
        GenerateTurnOrder(Character.Team.Player);
        NewTurn(turnOrder[0]);
    }

    void GenerateTurnOrder(Character.Team startingTeam)
    {
        if (startingTeam == Character.Team.Player)
        {
            turnOrder.AddRange(GameManager.Instance.playerTeam);
            turnOrder.AddRange(GameManager.Instance.enemyTeam);
        }
        else if (startingTeam == Character.Team.Enemy)
        {
            turnOrder.AddRange(GameManager.Instance.enemyTeam);
            turnOrder.AddRange(GameManager.Instance.playerTeam);
        }
    }

    void NewTurn(Character character)
    {
        curTurnCharacter = character;
        onNewTurn?.Invoke();
        
        endTurnButton.SetActive(character.team == Character.Team.Player);
    }

    public void EndTurn()
    {
        curTurnOrderIndex++;

        if (curTurnOrderIndex == turnOrder.Count)
        {
            curTurnOrderIndex = 0;
        }

        while (turnOrder[curTurnOrderIndex] == null)
        {
            curTurnOrderIndex++;
            
            if (curTurnOrderIndex == turnOrder.Count)
            {
                curTurnOrderIndex = 0;
            }
        }
        NewTurn(turnOrder[curTurnOrderIndex]);
    }

    public Character GetCurrentTurnCharacter()
    {
        return curTurnCharacter;
    }
}
