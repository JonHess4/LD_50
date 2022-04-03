using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour {
  public GameObject enemyPrefab;
  public GameObject deathPilePrefab;

  private PlayerController playerController;

private int oldSpawnCount = 2;
  private int spawnCount = 3;

  void Start() {
    this.playerController = Object.FindObjectOfType<PlayerController>();
    EnemyService.setDeathPilePrefab(this.deathPilePrefab);
    this.spawnEnemies(this.spawnCount, this.playerController.body.transform.position);

  }

  void Update() {
    if (EnemyService.enemyList.Count <= 0) {
      int newSpawnCount = System.Math.Min(this.spawnCount + this.oldSpawnCount, 144);
      this.oldSpawnCount = this.spawnCount;
      this.spawnCount = newSpawnCount;
      this.spawnEnemies(this.spawnCount, this.playerController.body.transform.position);
    }
  }

  public void spawnEnemies(int numEnemies, Vector3 playerPos) {
    List<GameObject> enemyList = new List<GameObject>();

    Tilemap tilemap = MapService.getTilemap();
    Dictionary<string, TileBase> tileDict = MapService.getTileDict();
    List<Vector3Int> tileDictKeys = MapService.getTileDictKeys();
    List<Vector3Int> spawnableCoordslist;

    string stringPlayerPos = MapService.vec3ToStringKey(playerPos);

    spawnableCoordslist = tileDictKeys.FindAll(key => {
      string stringKey = MapService.vec3ToStringKey(key);
      TileBase tile = tileDict[stringKey];
      Vector3Int aboveCoord = key + new Vector3Int(0, 0, 1);
      string aboveKey = MapService.vec3ToStringKey(aboveCoord);
      TileBase aboveTile = tileDict.ContainsKey(aboveKey) ? tileDict[aboveKey] : null;
      // Tile at position is not null and tile above position is null and tile can be stood on
      return tile != null && aboveTile == null && tile.name != TileEnum.Bush.ToString() && stringKey != stringPlayerPos;
    });

    for (int i = 0; i < numEnemies; i++) {
      int randSpawnPoint = Random.Range(0, spawnableCoordslist.Count);
      enemyList.Add(Instantiate(this.enemyPrefab, tilemap.CellToLocal(spawnableCoordslist[randSpawnPoint]) + new Vector3Int(0, 1, 1), new Quaternion()));
      spawnableCoordslist.RemoveAt(randSpawnPoint);
    }

    EnemyService.setEnemyList(enemyList);
  }
}
