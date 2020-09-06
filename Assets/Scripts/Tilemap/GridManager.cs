using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public GridLayout grid;
  public Tilemap tilemap;

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
}
