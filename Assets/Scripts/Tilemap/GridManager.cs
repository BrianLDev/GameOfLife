using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public GridLayout grid;
  public Tilemap tilemap;
  private int gridWidth, gridHeight;

  private void Awake() {
    if(!grid) {
      grid = GameObject.FindObjectOfType<GridLayout>();
    }
    if(!tilemap) {
      tilemap = GameObject.FindObjectOfType<Tilemap>();
    }
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
    Debug.Log("Creating a " + gridWidth + " x " + gridHeight + " grid.");
    // TODO: CREATE GRID
  }
}
