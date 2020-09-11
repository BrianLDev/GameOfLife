using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public Camera mainCamera;
  public GridLayout grid;
  public Tilemap tilemap;
  public Tile tileEmpty, tileSelected, tileAlive;
  // public TmapTile smartTile;   // not used in this version.  Keep for possible future use
  private Tilemap initialTilemap;
  private int gridWidth = 5;
  private int gridHeight = 5;
  private float targetFOV = 65;
  private Vector3 mousePosition;

  private void Awake() {
    if(!mainCamera)
      mainCamera = FindObjectOfType<Camera>();
    if(!grid)
      grid = FindObjectOfType<GridLayout>();
    if(!tilemap)
      tilemap = FindObjectOfType<Tilemap>();
  }

  private void Start() {
    SaveGridLayout();
    SetCameraFOV();
  }

  private void Update() {

    mousePosition = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(10);
    if (Input.GetMouseButtonDown(0)) {
      Debug.Log("Clicked at " + mousePosition);
      tilemap.SetTile(tilemap.WorldToCell(mousePosition), tileAlive);
    }

    // Smooth transition camera to target zoom
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
    StopAllCoroutines();
    ClearGrid();
    Debug.Log("Creating a " + gridWidth + " x " + gridHeight + " grid.");
    tilemap.size = new Vector3Int(gridWidth, gridHeight, 1);
    tilemap.origin = new Vector3Int(-gridWidth/2, -gridHeight/2, 0);
    tilemap.FloodFill(tilemap.origin, tileEmpty);
    SetCameraFOV();
  }

  public void ClearGrid() {
    Debug.Log("Clearing grid...");
    StopAllCoroutines();
    tilemap.ClearAllTiles();
  }

  public void RecalculateGridBounds() {
    tilemap.CompressBounds(); 
    tilemap.ResizeBounds();
    gridWidth = tilemap.size[0];
    gridHeight = tilemap.size[1];
  }

  public void Randomize() {
    StopAllCoroutines();
    // Tile newTile = new Tile();
    Vector3Int pos = new Vector3Int();
    float randomFloat;

    for (int i=0; i<tilemap.size.x; i++) {
      for (int j=0; j<tilemap.size.y; j++) {
        pos.x = tilemap.origin.x + i;
        pos.y = tilemap.origin.y + j;
        randomFloat = Random.Range(0f,1f);
        if (randomFloat <= 0.9) {
          // tileArray[i,j] = Instantiate(tileEmpty); // instantiate causes major slowdowns.  Just use copies of a prefab
          // newTile = Instantiate(tileEmpty); // instantiate causes major slowdowns.  Just use copies of a prefab
          tilemap.SetTile(pos, tileEmpty);
        }
        else {
          // tileArray[i,j] = Instantiate(tileAlive); // instantiate causes major slowdowns.  Just use copies of a prefab
          // newTile = Instantiate(tileAlive); // instantiate causes major slowdowns.  Just use copies of a prefab
          tilemap.SetTile(pos, tileAlive);
        }
        // tilemap.SetTile(pos, tileArray[i,j]);
      }
    }
  }

  public void SaveGridLayout() {
    // saves duplicate copy of initial tilemap to restore to later
    if(initialTilemap != null)
      Destroy(initialTilemap.gameObject);
    
    initialTilemap = Instantiate(tilemap, tilemap.transform.position, tilemap.transform.rotation, tilemap.transform.parent);
    initialTilemap.name = "Tilemap Initial State";
    initialTilemap.gameObject.SetActive(false); // make sure it's not visible
  }

  public void RestoreGridLayout() {
    Debug.Log("Restoring grid to previous state...");
    StopAllCoroutines();
    RestoreTilemap();
    SetCameraFOV();
  }

  private void RestoreTilemap() {
    // restores to copy of initial tilemap and makes a new copy
    if (tilemap != null)
      Destroy(tilemap.gameObject);

    tilemap = Instantiate(initialTilemap, initialTilemap.transform.position, initialTilemap.transform.rotation, initialTilemap.transform.parent);
    tilemap.name = "Tilemap";
    tilemap.gameObject.SetActive(true); // make sure it's visible
    RecalculateGridBounds();
  }

  public IEnumerator Simulate(int generations=1) {
    for (int g=1; g<=generations; g++) {
      Debug.Log("***** SIMULATING GENERATION " + g);
      Tile getTile = ScriptableObject.CreateInstance<Tile>();
      // Tile setTile = new Tile();
      Vector3Int pos = new Vector3Int();
      int aliveNeighbors = 0;

      // Debug.Log("Grid Size = " + tilemap.size);
      // Debug.Log("Grid Bounds Min = " + tilemap.cellBounds.xMin + "," + tilemap.cellBounds.yMin);
      // Debug.Log("Grid Bounds Max = " + tilemap.cellBounds.xMax + "," + tilemap.cellBounds.yMax);

      // Create a temp Tilemap to hold original state so incremental changes don't affect each other
      Tilemap tempTilemap = Instantiate(tilemap, tilemap.transform.position, tilemap.transform.rotation, tilemap.transform.parent);

      for (int i=0; i<tilemap.size.x; i++) {
        for (int j=0; j<tilemap.size.y; j++) {
          pos.x = tilemap.origin.x + i;
          pos.y = tilemap.origin.y + j;
          // tilemap.SetTileFlags(pos, TileFlags.None);  // remove tileflags so we can change color if needed. (note this changes the prefab)
          getTile = tempTilemap.GetTile<Tile>(pos);  // get copy of current tile to check alive/dead, color, etc
          aliveNeighbors = CountAliveNeighbors(getTile, pos);
          // Debug.Log("At position (" + pos.x + "," + pos.y + ") found " + aliveNeighbors + " alive neighbors.");

          // CHECK SURVIVAL OR BIRTH BASED ON GAME OF LIFE RULES
          // Rules are here https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
          bool isAlive = getTile.name == tileAlive.name;
          if (isAlive) { // Alive tiles. Use separate if statements for empty/alive to minimize the number of "if" checks.
            if (aliveNeighbors < 2 || aliveNeighbors > 3) {
              // setTile = Instantiate(tileEmpty);
              // setTile.color = Color.red;
              tilemap.SetTile(pos, tileEmpty);
            }
          }
          else {  // Empty tiles. Use separate if statements for empty/alive to minimize the number of "if" checks.
            if (aliveNeighbors == 3) {
              // setTile = Instantiate(tileAlive);
              // setTile.color = Color.green;
              tilemap.SetTile(pos, tileAlive);
            }
          }
          
        }
      }
      // Now that all cells have been updated, delete the copy of the original Tilemap
      Destroy(tempTilemap.gameObject);
      yield return null;
    }
    yield return null;
  }  

  private int CountAliveNeighbors(Tile tile, Vector3Int pos) {
    // Debug.Log("Counting neighbors for tile at: " + pos);
    int aliveCount = 0;

    Vector3Int[] neighborPositions = GetNeighborPositions(pos, tilemap);
    // Tile lookupTile = new Tile();

    foreach(Vector3Int neighborPos in neighborPositions) {
      // Debug.Log("Looking up tile at: " + neighborPos);
      // lookupTile = tilemap.GetTile<Tile>(neighborPos);
      // Debug.Log("Tile value: " + lookupTile);
      if(tilemap.GetTile<Tile>(neighborPos).name == tileAlive.name) {
        // Debug.Log("Found a live one...");
        aliveCount++;
      }
    }
    return aliveCount;
  }

  public static Vector3Int[] GetNeighborPositions(Vector3Int tilePos, Tilemap tmap) {
    Vector3Int[] neighbors = new Vector3Int[8];

    int[] xAxis = new int[3] {  // from left to right.  Handles wraparound
      tilePos.x-1 < tmap.cellBounds.xMin ? tmap.cellBounds.xMax-1 : tilePos.x-1,
      tilePos.x,
      tilePos.x+1 > tmap.cellBounds.xMax-1 ? tmap.cellBounds.xMin : tilePos.x+1
    };

    int[] yAxis = new int[3] {  // from bottom to top.  Handles wraparound
      tilePos.y-1 < tmap.cellBounds.yMin ? tmap.cellBounds.yMax-1 : tilePos.y-1,
      tilePos.y,
      tilePos.y+1 > tmap.cellBounds.yMax-1 ? tmap.cellBounds.yMin : tilePos.y+1
    };

    int neighborIndex = 0;
    foreach (int xVal in xAxis) {
      foreach (int yVal in yAxis) {
        if(xVal == tilePos.x && yVal == tilePos.y) {
          // Debug.Log("Skipping self");
          continue; // skip self
        }
        else {
          neighbors[neighborIndex].x = xVal;
          neighbors[neighborIndex].y = yVal;
          // Debug.Log("Neighbor " + neighborIndex + " = (" + xVal + "," + yVal + ")");
          neighborIndex++;
        }
      }
    }
    return neighbors;
  }


  private static Vector3Int PosNorm(Vector3Int tilePos, Tilemap tmap) {  // normalized tile position adjusting for origin
    Vector3Int normalizedPos = new Vector3Int();
    normalizedPos.x = tilePos.x - tmap.origin.x;
    normalizedPos.y = tilePos.y - tmap.origin.y;
    return normalizedPos;
  }

  private static Vector3Int PosOrig(Vector3Int tilePos, Tilemap tmap) {  // adjusts position based on tilemap origin
    Vector3Int adjustedPos = new Vector3Int();
    adjustedPos.x = tilePos.x + tmap.origin.x;
    adjustedPos.y = tilePos.y + tmap.origin.y;
    return adjustedPos;
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

  // NO LONGER USING THIS SCRIPT.  SAVE IN CASE I NEED TO REFERENCE IT
  // public Tile[,] PopulateTileArray(Tilemap tmap) {
  //   Tile[,] arrayOfTiles = new Tile[tmap.size.x, tmap.size.y];

  //   Tile tile = new Tile();
  //   Tile newTile = new Tile();
  //   Vector3Int pos = new Vector3Int();

  //   for (int i=0; i<gridWidth; i++) {
  //     for (int j=0; j<gridHeight; j++) {
  //       pos.x = tilemap.origin.x + i;
  //       pos.y = tilemap.origin.y + j;
  //       tile = tilemap.GetTile<Tile>(pos);  // get current tile to check alive/dead, color, etc
  //       arrayOfTiles[i,j] = tile; // assigns a copy of the tile to a spot in the 2d array
  //       // Debug.Log("Tile " + i + "," + j + " = " + tile.name);
  //       // Debug.Log("Array of tiles " + i + "," + j + " = " + arrayOfTiles[i,j].name);
  //     }
  //   }
  //   return arrayOfTiles;
  // }