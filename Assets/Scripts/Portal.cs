﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Collidable {

    public string[] sceneNames;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            // teleport the player to the new scene
            GameManager.instance.SaveState();
            string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
            if (sceneName == "Menu")
            {
                GameManager.instance.player.hitPoint = GameManager.instance.player.maxHitpoint;
                GameManager.instance.OnHitPointChange();

            }
            SceneManager.LoadScene(sceneName);
        }
    }
}
