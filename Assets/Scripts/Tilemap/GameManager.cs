using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject UI;
    public GridManager gridManager;

    private void Awake() {
        if (!UI)
            UI = GameObject.Find("UI");
        if (!gridManager)
            gridManager = GameObject.FindObjectOfType<GridManager>();
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
        gridManager.SaveGridState();   // save initial layout in case we need to reset back to it
        // TODO CREATE SIM LOGIC
    }

    public void ToggleUI() {
        UI.SetActive(!UI.activeInHierarchy);
    }
}
