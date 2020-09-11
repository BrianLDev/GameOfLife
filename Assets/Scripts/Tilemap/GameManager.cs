﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;    // singleton
    public GameObject menuPanel, menuToggler;
    public GridManager gridManager;
    public bool simulationRunning = false;
    public bool simulationPaused = false;
    public int generation = 0;

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }
        if (!menuPanel)
            menuPanel = GameObject.Find("Menu Panel");
        if (!menuToggler)
            menuToggler = GameObject.Find("Menu Toggler");
        if (!gridManager)
            gridManager = GameObject.FindObjectOfType<GridManager>();
    }

    private void Start() {
        menuPanel.SetActive(true);
        menuToggler.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleUI();

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (simulationRunning)
                SimPause();
            else {
                SimStart();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SimStep();
    }

    public void SimStart() {
        Debug.Log("Starting sim...");
        HideUI();
        if(!simulationPaused) {
            gridManager.SaveLayout();   // save initial layout in case we need to reset back to it
        }
        simulationRunning = true;
        simulationPaused = false;
        StartCoroutine(gridManager.Simulate(9999));
    }

    public void SimPause() {
        Debug.Log("Pausing Simulation...");
        simulationRunning = false;
        simulationPaused = true;
        StopAllCoroutines();
    }

    public void SimStep() {
        Debug.Log("Simulating 1 generation...");
        simulationRunning = true;
        HideUI();
        StartCoroutine(gridManager.Simulate(1));
        simulationRunning = false;
        simulationPaused = true;
    }

    public void SimStop() {
        Debug.Log("Stopping sim and restarting to gen 0...");
        simulationRunning = false;
        simulationPaused = false;
        StopAllCoroutines();
        generation = 0;
    }

    public void ToggleUI() {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
        menuToggler.SetActive(!menuToggler.activeInHierarchy);
    }

    public void HideUI() {
        menuPanel.SetActive(false);
        menuToggler.SetActive(true);
    }

    public void ShowUI() {
        menuPanel.SetActive(true);
        menuToggler.SetActive(false);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
