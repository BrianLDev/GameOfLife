using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject UI;

    private void Awake() {
        if (!UI)
            UI = GameObject.Find("UI");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleUI();
        }
    }

    public void Randomize() {
        Debug.Log("Randomizing grid layout");
        // TODO: RANDOMIZE GRID LAYOUT
    }

    public void StartSim() {
        Debug.Log("Starting sim...");
        ToggleUI();
        // TODO CREATE SIM LOGIC
    }

    public void ToggleUI() {
        UI.SetActive(!UI.activeInHierarchy);
    }
}
