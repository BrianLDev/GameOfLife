using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public Camera mainCamera;
  public GridLayout grid;
  public Tilemap tilemap;
  public TmapTile smartTile;
  private int gridWidth = 2;
  private int gridHeight = 2;
  private float targetFOV = 65;

  private void Awake() {
    if(!mainCamera)
      mainCamera = FindObjectOfType<Camera>();
    if(!grid)
      grid = FindObjectOfType<GridLayout>();
    if(!tilemap)
      tilemap = FindObjectOfType<Tilemap>();
    if(!smartTile)
      smartTile = FindObjectOfType<TmapTile>();
  }

  private void Start() {
    Debug.Log("Size = " + tilemap.size);
  }

  private void Update() {
    if ((mainCamera.fieldOfView <= targetFOV*.95) || (mainCamera.fieldOfView >= targetFOV*1.05)) {
      mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime*2);
    }
  }

  public void SetWidth(int width) {
    gridWidth = width+2;
  }

  public void SetHeight(int height) {
    gridHeight = height+2;
  }

  public void CreateGrid() {
    ClearGrid();
    Debug.Log("Creating a " + gridWidth + " x " + gridHeight + " grid.");
    tilemap.size = new Vector3Int(gridWidth, gridHeight, 1);
    tilemap.origin = new Vector3Int(-gridWidth/2, -gridHeight/2, 1);
    tilemap.FloodFill(tilemap.origin, smartTile);
    SetCameraFOV();
  }

  public void ClearGrid() {
    Debug.Log("Clearing grid...");
    tilemap.ClearAllTiles();
    tilemap.origin = Vector3Int.zero;
  }

  public void ResetGrid() {
    Debug.Log("Resetting grid...");
    ClearGrid();
    gridWidth = 2;
    gridHeight = 2;
    SetCameraFOV();
  }

  private void SetCameraFOV() {
    if(gridWidth <= 15 && gridHeight <= 10)
      targetFOV = 65;
    else if(gridWidth <= 30 && gridHeight <= 20)
      targetFOV = 95;
    else if(gridWidth <= 60 && gridHeight <= 40)
      targetFOV = 125;
  }
}
