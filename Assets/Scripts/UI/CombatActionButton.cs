using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    private CombatAction _combatAction;
    private CombatActionsUI ui;

    private void Awake()
    {
        ui = FindObjectOfType<CombatActionsUI>();
    }

    public void SetCombatAction(CombatAction ca)
    {
        _combatAction = ca;
        nameText.text = ca.displayName;
    }

    public void OnClick()
    {
        PlayerCombatManager.Instance.SetCurrentCombatAction(_combatAction);
    }

    public void OnHoverEnter()
    {
        ui.SetCombatActionDescription(_combatAction);
    }

    public void OnHoverExit()
    {
        ui.DisableCombatActionDescription();
    }
}
