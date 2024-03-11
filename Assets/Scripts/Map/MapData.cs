using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Data", menuName = "New Map Data")]
public class MapData : ScriptableObject
{
    public int curEncounter;
    private const string SAVE_KEY = "CurrentEncounter";

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            curEncounter = PlayerPrefs.GetInt(SAVE_KEY);
            return;
        }

        curEncounter = 0;
    }

    public void IncrementEncounter()
    {
        curEncounter++;
        PlayerPrefs.SetInt(SAVE_KEY, curEncounter);
        PlayerPrefs.Save();
    }
}
