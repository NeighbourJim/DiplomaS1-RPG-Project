﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateControl : MonoBehaviour {
    public TurnState currentState;
    public TurnState previousState;
    private MonBattleData firstToGo;
    private MonBattleData secondToGo;

    BattleControl battleControl;
    public GameObject battleUI;
    BattleUIControl uiButtonControl;
    BattleTextUIControl uiTextControl;
    BattleHPControl uiHPControl;

    public MoveData playerSelectedMove;
    public MoveData enemySelectedMove;

    // Use this for initialization
    void Start () {
        battleControl = GetComponent<BattleControl>();
        uiButtonControl = battleUI.GetComponent<BattleUIControl>();
        uiTextControl = battleUI.GetComponent<BattleTextUIControl>();
        uiHPControl = battleUI.GetComponent<BattleHPControl>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (currentState)
        {
            case (TurnState.Intro):
                uiTextControl.DisplayText(string.Format("A wild {0} appeared!", battleControl.enemyMon.monName));
                uiHPControl.SetMonsters(battleControl.playerMon, battleControl.enemyMon);
                //uiTextControl.DisplayText(string.Format("Go, {0}!", battleControl.playerMon.monName));
                AdvanceState(TurnState.SelectingAction);
                break;

            case (TurnState.SelectingAction):
                uiButtonControl.ShowActionSelect();
                break;

            case (TurnState.EnemySelectAction):
                battleControl.SelectEnemyMove();
                AdvanceState(TurnState.DetermineOrder);
                break;

            case (TurnState.DetermineOrder):
                DetermineTurnOrder();
                AdvanceState(TurnState.FirstAction);
                break;

            case (TurnState.FirstAction):
                uiTextControl.DisplayText(string.Format("{0} attacks with {1}!",firstToGo.monName, firstToGo.selectedMove.moveName));
                battleControl.ResolveAttack(firstToGo.selectedMove,firstToGo, secondToGo);
                AdvanceState(TurnState.FaintCheck);
                break;

            case (TurnState.SecondAction):
                //uiTextControl.DisplayText(string.Format("{0} attacks with {1}!", secondToGo.monName, secondToGo.selectedMove.moveName));
                battleControl.ResolveAttack(secondToGo.selectedMove, secondToGo, firstToGo);
                AdvanceState(TurnState.FaintCheck);
                break;

            case (TurnState.FaintCheck):
                if (CheckFainted(battleControl.playerMon))
                {
                    AdvanceState(TurnState.PlayerFaints);
                }
                else if (CheckFainted(battleControl.enemyMon))
                {
                    AdvanceState(TurnState.EnemyFaints);
                }
                else
                {
                    if(previousState == TurnState.FirstAction)
                    {
                        AdvanceState(TurnState.SecondAction);
                    }
                    if(previousState == TurnState.SecondAction)
                    {
                        AdvanceState(TurnState.SelectingAction);
                    }
                }
                break;

            case (TurnState.PlayerFaints):
                uiTextControl.DisplayText(battleControl.playerMon.monName + " fainted!");
                AdvanceState(TurnState.PlayerLose);
                break;
            case (TurnState.EnemyFaints):
                uiTextControl.DisplayText("The foe " + battleControl.enemyMon.monName + " fainted!");
                AdvanceState(TurnState.PlayerWin);
                break;
            case (TurnState.BothFaint):

                break;
            case (TurnState.PlayerLose):
                uiTextControl.DisplayText("You lose.");
                break;
            case (TurnState.PlayerWin):
                uiTextControl.DisplayText("You win!");
                break;
        }
	}

    public void AdvanceState(TurnState state)
    {
        previousState = currentState;
        currentState = state;
    }

    void DetermineTurnOrder()
    {
        if (battleControl.playerMon.curSpeed > battleControl.enemyMon.curSpeed)
        {
            firstToGo = battleControl.playerMon;
            secondToGo = battleControl.enemyMon;
        }
        else if (battleControl.playerMon.curSpeed < battleControl.enemyMon.curSpeed)
        {
            firstToGo = battleControl.enemyMon;
            secondToGo = battleControl.playerMon;
        }
        else
        {
            if (Random.value < 0.5f)
            {
                firstToGo = battleControl.playerMon;
                secondToGo = battleControl.enemyMon;
            }
            else
            {
                firstToGo = battleControl.enemyMon;
                secondToGo = battleControl.playerMon;
            }                
        }
    }

    bool CheckFainted(MonBattleData mon)
    {
        return (mon.curHP == 0);
    }
}
