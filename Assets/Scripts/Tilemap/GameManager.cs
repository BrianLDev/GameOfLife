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
        gridManager.Randomize();
    }

    public void StartSim() {
        Debug.Log("Starting sim...");
        // ToggleUI();
        gridManager.SaveGridState();   // save initial layout in case we need to reset back to it
        StartCoroutine(gridManager.Simulate(999));
    }

    public void StopSim() {
        Debug.Log("Stopping sim...");
        StopAllCoroutines();
    }

    public void ToggleUI() {
        UI.SetActive(!UI.activeInHierarchy);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
