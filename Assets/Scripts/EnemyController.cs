using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour {
  public GameObject enemyPrefab;
  public GameObject deathPilePrefab;

  private int spawnCount = 2;

  void Start() {
    EnemyService.setDeathPilePrefab(this.deathPilePrefab);
    this.spawnEnemies(this.spawnCount);
  }

  void Update() {
    if (EnemyService.enemyList.Count <= 0) {
      this.spawnCount = System.Math.Min(this.spawnCount * 2, 128);
      this.spawnEnemies(this.spawnCount);
    }
  }

  public void spawnEnemies(int numEnemies) {
    List<GameObject> enemyList = new List<GameObject>();

    Tilemap tilemap = MapService.getTilemap();
    Dictionary<string, TileBase> tileDict = MapService.getTileDict();
    List<Vector3Int> tileDictKeys = MapService.getTileDictKeys();
    List<Vector3Int> spawnableCoordslist;

    spawnableCoordslist = tileDictKeys.FindAll(key => {
      TileBase tile = tileDict[MapService.vec3ToStringKey(key)];
      Vector3Int aboveCoord = key + new Vector3Int(0, 0, 1);
      string aboveKey = MapService.vec3ToStringKey(aboveCoord);
      TileBase aboveTile = tileDict.ContainsKey(aboveKey) ? tileDict[aboveKey] : null;
      // Tile at position is not null and tile above position is null and tile can be stood on
      return tile != null && aboveTile == null && tile.name != TileEnum.Bush.ToString();
    });

    for (int i = 0; i < numEnemies; i++) {
      int randSpawnPoint = Random.Range(0, spawnableCoordslist.Count);
      enemyList.Add(Instantiate(this.enemyPrefab, tilemap.CellToLocal(spawnableCoordslist[randSpawnPoint]) + new Vector3Int(0, 1, 1), new Quaternion()));
      spawnableCoordslist.RemoveAt(randSpawnPoint);
    }

    Debug.Log(enemyList.Count);
    enemyList.ForEach(enemy => {
      Debug.Log(enemy);
    });
    EnemyService.setEnemyList(enemyList);
  }
}
