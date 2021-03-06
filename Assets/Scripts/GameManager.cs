﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        // If the game manager already exists don't create another one just return the existing instance
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }

        instance = this;

        // Attach to events when the scene is loaded
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // References to game objects
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;

    public List<int> weaponPrices;
    public List<int> xpTable;

    public Player player;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitPointBar;
    public Animator deathMenuAnimator;
    public GameObject hud;
    public GameObject menu;
    public Image ability;

    // Player State
    public int coins = 0;
    public int experience = 0;
    public int selectedWeapon = 0;

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    // Hitpoint Bar
    public void OnHitPointChange()
    {
        float ratio = (float)player.hitPoint / (float)player.maxHitpoint;
        hitPointBar.localScale = new Vector3(ratio, 1, 1);
    }

    // Experience Logic
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;
        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }

    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;
        }

        if (r == xpTable.Count)  // MAX LEVEL REACHED
            return r; 

        return r;

    }

    public void GrantXp(int xp)
    {
        int currentLevel = GetCurrentLevel();
        experience += xp;
        if (currentLevel < GetCurrentLevel())
        {
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {
        player.OnLevelUp();
        OnHitPointChange();
    }

    public void OnWeaponChange()
    {
        selectedWeapon++;
        if (selectedWeapon > weaponSprites.Count - 1)
        {
            selectedWeapon = 0;
        }
        ability.sprite = weaponSprites[selectedWeapon];
    }

    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    // Saves the state of the character between scenes
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += coins.ToString() + "|";
        s += experience.ToString() + "|";

        PlayerPrefs.SetString("SaveState", s);
    }

    // Clears the players state for a new game
    public void Respawn()
    {
        coins = 0;
        experience = 0;
        
        SaveState();
        deathMenuAnimator.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // Loads the state of the character between scenes
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        coins = int.Parse(data[1]);

        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());
       
    }
}
