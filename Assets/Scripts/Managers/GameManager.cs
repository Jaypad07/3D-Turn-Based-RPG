using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Character> playerTeam;
    public Character[] enemyTeam;

    private List<Character> allCharacters = new List<Character>();

    [Header("Components")]
    public Transform[] playerTeamSpawns;
    public Transform[] enemyTeamSpawns;

    [Header("Data")]
    public PlayerPersistentData playerPersistentData;
    public CharacterSet defaultEnemySet;

    public static GameManager Instance;
    public static CharacterSet curEnemySet;
    
    // For saving player progress.
    public MapData data;

    private void OnEnable()
    {
        Character.onCharacterDeath += OnCharacterKilled;
    }

    private void OnDisable()
    {
        Character.onCharacterDeath -= OnCharacterKilled;
    }

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

    private void Start()
    {
        if (curEnemySet == null)
        {
            CreateCharacters(playerPersistentData, defaultEnemySet);
        }
        else
        {
            CreateCharacters(playerPersistentData, curEnemySet);
        }
        
        TurnManager.instance.Begin();
    }

    void CreateCharacters(PlayerPersistentData playerData, CharacterSet enemyTeamSet)
    {
        playerTeam = new List<Character>();
        enemyTeam = new Character[enemyTeamSet.characters.Length];

        int playerSpawnIndex = 0;

        for (int i = 0; i < playerData.characters.Length; i++)
        {
            if (!playerData.characters[i].isDead)
            {
                Character character = CreateCharacter(playerData.characters[i].characterPrefab, playerTeamSpawns[playerSpawnIndex]);
                character.curHp = playerData.characters[i].health;
                playerTeam.Add(character);
                playerSpawnIndex++;
            }
        }

        for (int i = 0; i < enemyTeamSet.characters.Length; i++)
        {
            Character character = CreateCharacter(enemyTeamSet.characters[i], enemyTeamSpawns[i]);
            enemyTeam[i] = character;
        }

        allCharacters.AddRange(playerTeam);
        allCharacters.AddRange(enemyTeam);
    }

    Character CreateCharacter(GameObject characterPrefab, Transform spawnPos)
    {
        GameObject obj = Instantiate(characterPrefab, spawnPos.position, spawnPos.rotation);
        return obj.GetComponent<Character>();
    }

    void OnCharacterKilled(Character character)
    {
        allCharacters.Remove(character);

        int playersRemaining = 0;
        int enemiesRemaining = 0;

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacters[i].team == Character.Team.Player)
                playersRemaining++;
            else
                enemiesRemaining++;
        }

        if (enemiesRemaining == 0)
        {
            PlayerTeamWins();
        }
        else if (playersRemaining == 0)
        {
            EnemyTeamWins();
        }
    }

    void PlayerTeamWins()
    {
        UpdatePlayerPersistentData();
        Invoke(nameof(LoadMapScene), 0.5f);
    }

    void EnemyTeamWins()
    {
        playerPersistentData.ResetCharacters();
        Invoke(nameof(LoadMapScene), 0.5f);
    }

    void UpdatePlayerPersistentData()
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            if (playerTeam[i] != null)
            {
                playerPersistentData.characters[i].health = playerTeam[i].curHp;
            }
            else
            {
                playerPersistentData.characters[i].isDead = true;
            }
        }
    }

    void LoadMapScene()
    {
        MapManager.instance.data.IncrementEncounter();
        SceneManager.LoadScene("Map");
    }
}