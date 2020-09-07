using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject menuPanel, menuToggler;
    public GridManager gridManager;

    private void Awake() {
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            ToggleUI();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(gridManager.Simulate(1));
    }

    public void StartSim() {
        Debug.Log("Starting sim...");
        // ToggleUI();
        gridManager.SaveGridState();   // save initial layout in case we need to reset back to it
        StartCoroutine(gridManager.Simulate(9999));
    }

    public void StopSim() {
        Debug.Log("Stopping sim...");
        StopAllCoroutines();
    }

    public void ToggleUI() {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
        menuToggler.SetActive(!menuToggler.activeInHierarchy);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
