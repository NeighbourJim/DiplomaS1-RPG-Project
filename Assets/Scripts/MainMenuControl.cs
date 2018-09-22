﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour {

    public GameObject mainPanel;
    public GameObject monPanel;
    public GameObject settingsPanel;

    private void Awake()
    {
        ShowMain();
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        monPanel.SetActive(false);
    }

    public void ShowMonsterSelect()
    {
        mainPanel.SetActive(false);
        monPanel.SetActive(true);
    }

    public void ShowSettings()
    {

    }

	public void LoadOverworld()
    {
        SceneManager.LoadScene("Overworld_Route1");
    }

    public void SelectMonster(int monID)
    {
        PlayerDataHolder playerData = GameObject.Find("GameDataController").GetComponent<PlayerDataHolder>();

        playerData.SetData(monID, 20);
        LoadOverworld();
    }
}