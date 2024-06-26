using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Encounter : MonoBehaviour, IPointerDownHandler
{
    public enum State
    {
        Locked,
        CanVisit,
        Visited
    }

    public CharacterSet enemySet;
    private State state;

    public Color lockedColor;
    public Color canVisitColor;
    public Color visitedColor;

    public Renderer render;

    public void SetState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.Locked:
            {
                render.material.color = lockedColor;
                break;
            }
            case State.CanVisit:
            {
                render.material.color = canVisitColor;
                break;
            }
            case State.Visited:
            {
                render.material.color = visitedColor;
                break;
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == State.CanVisit)
        {
            MapManager.instance.MoveParty(this);
        }
    }
}
