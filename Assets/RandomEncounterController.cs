﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomEncounterController : MonoBehaviour {

    GameObject player;
    PlayerDataHolder playerData;
    EnemyDataHolder enemyData;
    RegionDataHolder regionData;

    public int noRecentBattleMinSecs;
    public int noRecentBattleMaxSecs;
    public int recentBattleMinSecs;
    public int recentBattleMaxSecs;

    public float recentBattleTime = 30f;

    Image screenRect;

    [SerializeField] float timeUntilEncounter;
    [SerializeField] float recentBattleCounter;

    public static bool battledRecently = false;

    public AudioClip encounterStart;

    Monpedia mp;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerData = GetComponent<PlayerDataHolder>();
        enemyData = GetComponent<EnemyDataHolder>();
        mp = GetComponent<Monpedia>();
        regionData = GameObject.Find("RegionData").GetComponent<RegionDataHolder>();
        screenRect = GameObject.Find("BattleTransitionRect").GetComponent<Image>();
        ResetBattleTimer();
    }

    // Update is called once per frame
    void Update ()
    {
        if (player != null)
        {
            if (player.GetComponent<SimpleCharacterControl>().moving && player.GetComponent<SimpleCharacterControl>().inEncounterZone)
            {
                timeUntilEncounter -= Time.deltaTime;
                if (timeUntilEncounter < 0.2f)
                {
                    StartWildBattle();
                }
            }
            if (battledRecently)
            {
                recentBattleCounter -= Time.deltaTime;
                if (recentBattleCounter < 0.2f)
                {
                    battledRecently = false;
                    int r = Random.Range(noRecentBattleMinSecs, noRecentBattleMaxSecs);
                    if (r < timeUntilEncounter)
                    {
                        timeUntilEncounter = r;
                    }
                }
            }
        }
	}

    void StartWildBattle()
    {
        battledRecently = true;
        enemyData.SetData(regionData.GetEncounterMon(), regionData.GetEncounterLevel());
        ResetBattleTimer();
        ResetRecentBattleTimer();
        StartCoroutine(BattleTransition());
    }

    void ResetBattleTimer()
    {
        if (battledRecently)
        {
            timeUntilEncounter = (float)Random.Range(recentBattleMinSecs, recentBattleMaxSecs);
        }
        else
        {
            timeUntilEncounter = (float)Random.Range(noRecentBattleMinSecs, noRecentBattleMaxSecs);
        }
    }

    void ResetRecentBattleTimer()
    {
        recentBattleCounter = recentBattleTime;
    }

    IEnumerator BattleTransition()
    {
        GameObject.Find("MusicController").GetComponent<AudioSource>().clip = encounterStart;
        GameObject.Find("MusicController").GetComponent<AudioSource>().loop = false;
        GameObject.Find("MusicController").GetComponent<AudioSource>().Play();
        for (float a = 0f; a <= 1f; a += Time.deltaTime * 8)
        {
            screenRect.color = new Color(screenRect.color.r, screenRect.color.g, screenRect.color.b, a);
            if(a > 0.91f)
            {
                screenRect.color = new Color(screenRect.color.r, screenRect.color.g, screenRect.color.b, 1);
            }
            yield return null;
        }
        while (GameObject.Find("MusicController").GetComponent<AudioSource>().isPlaying)
        {
            yield return null;
        }
        SceneManager.LoadScene("BaseBattle");
    }
}
