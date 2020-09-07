﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public Camera mainCamera;
  public GridLayout grid;
  public Tilemap tilemap;
  public Tile tileEmpty, tileSelected, tileAlive;
  // public TmapTile smartTile;   // not used in this version.  Keep for possible future use
  private Tile[,] tileArray;
  private Tilemap initialTilemap;
  private int gridWidth = 5;
  private int gridHeight = 5;
  private float targetFOV = 65;

  private void Awake() {
    if(!mainCamera)
      mainCamera = FindObjectOfType<Camera>();
    if(!grid)
      grid = FindObjectOfType<GridLayout>();
    if(!tilemap)
      tilemap = FindObjectOfType<Tilemap>();
  }

  private void Start() {
    SaveGridState();
    SetCameraFOV();
  }

  private void Update() {
    if ((mainCamera.fieldOfView <= targetFOV*.995) || (mainCamera.fieldOfView >= targetFOV*1.005)) {
      mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime*3.5f);
    }
  }

  public void SetWidth(int widthIndex) {
    if (widthIndex==0) { gridWidth = 5; }
    else if (widthIndex <=19)
      gridWidth = (widthIndex+1)*5;   // increments of 5 from 5 to 100 (index 0 to 19)
    else
      gridWidth = (widthIndex-17)*50; // increments of 50 from 150 to 500 (index 20 to 27)
  }

  public void SetHeight(int heightIndex) {
    if (heightIndex==0) { gridHeight = 5; }
    else if (heightIndex <= 19)
      gridHeight = (heightIndex+1)*5;   // increments of 5 from 5 to 100 (index 0 to 19)
    else if (heightIndex >= 20)
      gridHeight = (heightIndex-17)*50; // increments of 50 from 150 to 500 (index 20 to 27)
  }

  public void CreateGridLayout() {
    ClearGrid();
    Debug.Log("Creating a " + gridWidth + " x " + gridHeight + " grid.");
    tilemap.size = new Vector3Int(gridWidth, gridHeight, 1);
    tilemap.origin = new Vector3Int(-gridWidth/2, -gridHeight/2, 0);
    tilemap.FloodFill(tilemap.origin, tileEmpty);
    SetCameraFOV();
  }

  public void ClearGrid() {
    Debug.Log("Clearing grid...");
    tilemap.ClearAllTiles();
  }

  public void ResetGrid() {
    Debug.Log("Resetting grid to previous state...");
    RestoreGridState();
    SetCameraFOV();
  }

  public void RecalculateGridBounds() {
    tilemap.CompressBounds(); 
    tilemap.ResizeBounds();
    gridWidth = tilemap.size[0];
    gridHeight = tilemap.size[1];
  }

  public void SaveGridState() {
    // saves duplicate copy of initial tilemap to restore to later
    if(initialTilemap != null)
      Destroy(initialTilemap.gameObject);
    
    initialTilemap = Instantiate(tilemap, tilemap.transform.position, tilemap.transform.rotation, tilemap.transform.parent);
    initialTilemap.name = "Tilemap Initial State";
    initialTilemap.gameObject.SetActive(false); // make sure it's not visible
  }

  public void RestoreGridState() {
    // restores to copy of initial tilemap and makes a new copy
    if (tilemap != null)
      Destroy(tilemap.gameObject);

    tilemap = Instantiate(initialTilemap, initialTilemap.transform.position, initialTilemap.transform.rotation, initialTilemap.transform.parent);
    tilemap.name = "Tilemap";
    tilemap.gameObject.SetActive(true); // make sure it's visible
    RecalculateGridBounds();
  }

  public Tile[,] PopulateTileArray(Tilemap tmap) {
    Tile[,] arrayOfTiles = new Tile[tmap.size.x, tmap.size.y];

    Tile tile = new Tile();
    Tile newTile = new Tile();
    Vector3Int pos = new Vector3Int();

    for (int i=0; i<gridWidth; i++) {
      for (int j=0; j<gridHeight; j++) {
        pos.x = tilemap.origin.x + i;
        pos.y = tilemap.origin.y + j;
        tile = tilemap.GetTile<Tile>(pos);  // get current tile to check alive/dead, color, etc
        arrayOfTiles[i,j] = tile; // assigns a copy of the tile to a spot in the 2d array
        // Debug.Log("Tile " + i + "," + j + " = " + tile.name);
        // Debug.Log("Array of tiles " + i + "," + j + " = " + arrayOfTiles[i,j].name);
      }
    }
    return arrayOfTiles;
  }

  public void Randomize() {
    tileArray = PopulateTileArray(tilemap);
    Tile newTile = new Tile();
    Vector3Int pos = new Vector3Int();

    for (int i=0; i<tilemap.size.x; i++) {
      for (int j=0; j<tilemap.size.y; j++) {
        pos.x = tilemap.origin.x + i;
        pos.y = tilemap.origin.y + j;
        if (Random.Range(1,100) <= 93) {
          tileArray[i,j] = Instantiate(tileEmpty);
        }
        else {
          tileArray[i,j] = Instantiate(tileAlive);
        }
        tilemap.SetTile(pos, tileArray[i,j]);
      }
    }
  }

  private int CountAliveNeighbors(Tile tile, Vector3Int pos) {

    // Debug.Log("Counting neighbors for tile at: " + pos);
    Vector3Int normalizedPos = new Vector3Int();
    normalizedPos.x = pos.x - tilemap.origin.x;
    normalizedPos.y = pos.y - tilemap.origin.y;

    int aliveCount = 0;
    Tile lookupTile = new Tile();
    Vector3Int lookupPos = new Vector3Int();
    Vector3Int lookupPosOrigin = new Vector3Int();

    for (int i=-1; i<2; i++) {
      lookupPos.x = normalizedPos.x + i;

      if(lookupPos.x < 0) { 
        lookupPos.x = tilemap.size.x-1; 
      }
      else if (lookupPos.x > tilemap.size.x-1) { 
        lookupPos.x = 0; 
      }
      for (int j=-1; j<2; j++) {
        lookupPos.y = normalizedPos.y + j;

        if(lookupPos.y < 0) { 
          lookupPos.y = tilemap.size.y-1; 
        }
        else if (lookupPos.y > tilemap.size.y-1) { 
          lookupPos.y = 0; 
        }

        // Debug.Log("looking up tile at: " + lookupPos);
        lookupPosOrigin.x = lookupPos.x + tilemap.origin.x;
        lookupPosOrigin.y = lookupPos.y + tilemap.origin.y;
        // Debug.Log("adjusted by origin to: " + lookupPosOrigin);
        lookupTile = tilemap.GetTile<Tile>(lookupPosOrigin);
        // Debug.Log(lookupTile);
        if (lookupTile.sprite == tileAlive.sprite) {
          aliveCount++;
          // Debug.Log("Found a live one...");
        }
      }
    }
    return aliveCount;
  }

  public void Simulate(int generations=1) {
    tileArray = PopulateTileArray(tilemap);

    Tile tile = new Tile();
    Tile newTile = new Tile();
    Vector3Int pos = new Vector3Int();
    int aliveNeighbors = 0;

    for (int i=0; i<tilemap.size.x; i++) {
      for (int j=0; j<tilemap.size.y; j++) {
        pos.x = tilemap.origin.x + i;
        pos.y = tilemap.origin.y + j;
        tile = tilemap.GetTile<Tile>(pos);  // get current tile to check alive/dead, color, etc
        
        tilemap.SetTileFlags(pos, TileFlags.None);  // remove tileflags so we can change color

        // if (tile.sprite == tileEmpty.sprite) {
        //   newTile = Instantiate(tileAlive);
        //   newTile.color = Color.green;
        //   tilemap.SetTile(pos, newTile);
        // }
        // else if (tile.sprite == tileAlive.sprite) {
        //   newTile = Instantiate(tileSelected);
        //   newTile.color = Color.red;
        //   tilemap.SetTile(pos, newTile);
        // }

        aliveNeighbors = CountAliveNeighbors(tile, pos);
        // Debug.Log("At position (" + pos.x + "," + pos.y + ") found " + aliveNeighbors + " alive neighbors.");
        bool alive = tile.sprite == tileAlive.sprite;
        if (alive && aliveNeighbors < 2) {
          newTile = Instantiate(tileEmpty);
          newTile.color = Color.red;
          tilemap.SetTile(pos, newTile);
        }
        else if (alive && aliveNeighbors > 3) {
          newTile = Instantiate(tileEmpty);
          newTile.color = Color.red;
          tilemap.SetTile(pos, newTile);
        }
        else if (!alive && aliveNeighbors == 3) {
          newTile = Instantiate(tileAlive);
          newTile.color = Color.green;
          tilemap.SetTile(pos, newTile);
        }
      }
    }
  }  

  private void SetCameraFOV() {
    // TODO: Automate this if possible and if there's time.  For now, it works. 
    RecalculateGridBounds();

    if(gridWidth <= 20 && gridHeight <= 10)
      targetFOV = 65;
    else if(gridWidth <= 30 && gridHeight <= 20)
      targetFOV = 100;
    else if(gridWidth <= 45 && gridHeight <= 30)
      targetFOV = 120;
    else if(gridWidth <= 60 && gridHeight <= 40)
      targetFOV = 135;
    else if(gridWidth <= 75 && gridHeight <= 50)
      targetFOV = 142;
    else if(gridWidth <= 100 && gridHeight <= 75)
      targetFOV = 153;      
    else if(gridWidth <= 150 && gridHeight <= 100)
      targetFOV = 160;  
    else if(gridWidth <= 300 && gridHeight <= 200)
      targetFOV = 170;  
    else if(gridWidth <= 450 && gridHeight <= 300)
      targetFOV = 172.5f;  
    else if(gridWidth <= 600 && gridHeight <= 400)
      targetFOV = 175.5f;  
    else if(gridWidth <= 750 && gridHeight <= 500)
      targetFOV = 176.5f;  
  }
}
