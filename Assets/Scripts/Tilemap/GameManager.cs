using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;    // singleton
    [SerializeField] GameObject menuPanel, menuToggler;
    [SerializeField] Text textUIGen;
    public enum GameState {Simulating, Paused}
    public GameState gameState { get; private set; }
    public GridManager gridManager { get; private set; }
    public int generation { get; private set; }


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
        if (textUIGen)
            textUIGen = Text.FindObjectOfType<Text>();
    }

    private void Start() {
        menuPanel.SetActive(true);
        menuToggler.SetActive(false);
        gameState = GameState.Paused;
        generation = 0;
    }

    private void Update() {
        // INPUT
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleUI();
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (gameState == GameState.Simulating)
                SimPause();
            else {
                SimStart();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SimStep();

        // UPDATE UI
        textUIGen.text = "Gen: " + generation;
    }


    public void SimStart() {
        Debug.Log("Starting sim...");
        HideUI();
        if(generation == 0) {
            gridManager.SaveLayout();   // save initial layout in case we need to reset back to it
        }
        gameState = GameState.Simulating;
        StartCoroutine(gridManager.Simulate(9999));
    }

    public void SimPause() {
        Debug.Log("Pausing Simulation...");
        StopAllCoroutines();
        gameState = GameState.Paused;
    }

    public void SimStep() {
        Debug.Log("Simulating 1 generation...");
        gameState = GameState.Simulating;
        HideUI();
        StartCoroutine(gridManager.Simulate(1));
        gameState = GameState.Paused;
    }

    public void SimReset() {
        Debug.Log("Stopping sim and resetting to gen 0...");
        StopAllCoroutines();
        gameState = GameState.Paused;
        generation = 0;
    }

    public void GenerationIncrement() {
        generation++;
    }

    public void ToggleUI() {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
        menuToggler.SetActive(!menuToggler.activeInHierarchy);
    }

    public void HideUI() {
        menuPanel.SetActive(false);
        menuToggler.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(menuToggler);
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
