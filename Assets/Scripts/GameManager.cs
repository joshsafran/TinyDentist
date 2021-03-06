﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState{
	Intro,Game,Win,Lose
}


public class GameManager : MonoBehaviour {
	public static int maxLooseTeeth = 7;
	public static int looseTeeth = 0;
	public GameState gameState = GameState.Intro;

	public GameObject intro;
	public GameObject game;
	public GameObject win;
	public GameObject lose;

	public GameObject spawnArmPrefab;

	public int bonusScore = 0;
	public float timeRemaining = 100;

	public Text looseLabel;
	public Text bonusPtsLabel;
	public Text timeLabel;

	public List<ToothBehaviour> teethList = new List<ToothBehaviour>();
	PlayerController playerController;
	float startDelay = 0f;
	public float startDelayBuffer = 1f;
	public Transform playerIntroLockPt;

	public float inputDelayTimeTotal = 4f;
	float inputDelay = 0f;

	GiantHeadBehaviour head;
	// Use this for initialization
	void Start () {
		playerController = GameObject.FindObjectOfType<PlayerController>();
		teethList = new List<ToothBehaviour>(GameObject.FindObjectsOfType<ToothBehaviour>());
		var arm = spawnArmPrefab.GetComponent<ArmBehaviour>();
		startDelay = arm.lerpSpeed + startDelayBuffer;
		inputDelay = inputDelayTimeTotal;

		head = GameObject.FindObjectOfType<GiantHeadBehaviour>();
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSceneState();
		UpdateForState();
	}

	void UpdateForState(){
		inputDelay -= Time.deltaTime;
		switch(gameState){
			case GameState.Intro:
				var rigidBody = playerController.GetComponent<Rigidbody2D>();
				rigidBody.simulated = false;
				playerController.transform.position = playerIntroLockPt.position;
				if(inputDelay <= 0f && Input.anyKeyDown){
					StartGame();
				}
				break;
			case GameState.Game:
				if(startDelay>0){
					startDelay -= Time.deltaTime;
					looseLabel.text = "";
					bonusPtsLabel.text = "";
					timeLabel.text = "";
				}else{
					UpdateScoreLabels();
					timeRemaining -= Time.deltaTime;
					CheckForGameEnd();
				}
				

				// if(Input.GetKeyDown(KeyCode.O)){
				// 	DidLose();
				// }if(Input.GetKeyDown(KeyCode.P)){
				// 	DidWin();
				// }
				break;
			case GameState.Win:
				if(inputDelay <= 0f && Input.anyKeyDown){
					ReloadScene();
				}
				break;
			case GameState.Lose:
				if(inputDelay <= 0f && Input.anyKeyDown){
					ReloadScene();
				}
				break;
		}
	}

	void StartGame(){
		var rigidBody = playerController.GetComponent<Rigidbody2D>();
		rigidBody.simulated = true;

		gameState = GameState.Game;
		// GameObject.Instantiate(spawnArmPrefab);
	}

	void CheckForGameEnd(){
		var looseCount = LooseTeethRemaining();
		if(looseCount <= 0){
			DidWin();
		}

		if(timeRemaining < 0f){
			DidLose();
		}
	}

	void DidLose(){
		head.SwitchToTitleAnim();
		gameState = GameState.Lose;
		inputDelay = inputDelayTimeTotal;
	}

	void DidWin(){
		head.SwitchToTitleAnim();
		gameState = GameState.Win;
		inputDelay = inputDelayTimeTotal;
	}

	void UpdateScoreLabels(){
		if(bonusScore == 0){
			bonusPtsLabel.text = "";
		}else{
			bonusPtsLabel.text = "" +bonusScore	+" BONUS PT" + ((bonusScore > 1) ? "S!":"!");
		}
		
		var looseCount = LooseTeethRemaining();
		looseLabel.text = "" + looseCount + " LOOSE " + ((looseCount == 1) ? "TOOTH" : "TEETH");

		var timeDisplayNum = (int)timeRemaining;
		timeDisplayNum = Mathf.Max(0,timeDisplayNum);

		timeLabel.text = timeDisplayNum + " SECOND" + ((timeDisplayNum == 1) ? "":"S");

	}

	int LooseTeethRemaining(){
		int total = 0;
		foreach(ToothBehaviour tooth in teethList){
			if(tooth.toothState == ToothState.Anchored && tooth.isLooseTooth){
				total += 1;
			}
		}
		return total;
	}

	void ReloadScene(){
		looseTeeth = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void UpdateSceneState(){
		intro.SetActive(gameState == GameState.Intro);
		game.SetActive(gameState == GameState.Game || 
						gameState == GameState.Lose || 
						gameState == GameState.Win);
		win.SetActive(gameState == GameState.Win);
		lose.SetActive(gameState == GameState.Lose);

	}
}
