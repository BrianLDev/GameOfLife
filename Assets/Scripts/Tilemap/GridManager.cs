using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public Camera mainCamera;
  public GridLayout grid;
  public Tilemap tilemap;
  public TmapTile smartTile;
  private int gridWidth, gridHeight;

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

  public void SetWidth(int width) {
    gridWidth = width+1;
  }

  public void SetHeight(int height) {
    gridHeight = height+1;
  }

  public void CreateGrid() {
    ResetGrid();
    Debug.Log("Creating a " + gridWidth + " x " + gridHeight + " grid.");
    tilemap.size = new Vector3Int(gridWidth, gridHeight, 1);
    tilemap.origin = new Vector3Int(-gridWidth/2, -gridHeight/2, 1);
    tilemap.FloodFill(tilemap.origin, smartTile);
  }

  public void ResetGrid() {
    Debug.Log("Resetting grid");
    tilemap.ClearAllTiles();
    tilemap.origin = Vector3Int.zero;
  }
}
