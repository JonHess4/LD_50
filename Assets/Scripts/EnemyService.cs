using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyService : MonoBehaviour {
  private static Dictionary<DirectionEnum, Vector3> dirVectDict = null;
  public static GameObject deathPilePrefab;
  public static List<GameObject> enemyList;
  private static HashSet<string> deathPileList;
  private static List<GameObject> deathPileObjs = null;

  private static Vector3Int tileCharDif = new Vector3Int(0, 1, 1);

  public static Dictionary<DirectionEnum, Vector3> getDirVectDict() {
    if (dirVectDict == null) {
      dirVectDict = new Dictionary<DirectionEnum, Vector3>();
      dirVectDict.Add(DirectionEnum.E, new Vector3(1f, 0f, 0f));
      dirVectDict.Add(DirectionEnum.NE, new Vector3(0.5f, 0.5f, 0f));
      dirVectDict.Add(DirectionEnum.SE, new Vector3(0.5f, -0.5f, 0f));
      dirVectDict.Add(DirectionEnum.N, new Vector3(0f, 1f, 0f));
      dirVectDict.Add(DirectionEnum.S, new Vector3(0f, -1f, 0f));
      dirVectDict.Add(DirectionEnum.NW, new Vector3(-0.5f, 0.5f, 0f));
      dirVectDict.Add(DirectionEnum.SW, new Vector3(-0.5f, -0.5f, 0f));
      dirVectDict.Add(DirectionEnum.W, new Vector3(-1f, 0f, 0f));
    }
    return dirVectDict;
  }

  public static void setDeathPilePrefab(GameObject newDeathPilePrefab) {
    deathPilePrefab = newDeathPilePrefab;
  }
  public List<GameObject> getEnemyList() {
    return enemyList;
  }
  public static void setEnemyList(List<GameObject> newEnemyList) {
    enemyList = newEnemyList;
    deathPileList = new HashSet<string>();

    if (deathPileObjs != null) {
      deathPileObjs.ForEach(obj => {
        Destroy(obj);
      });
    }

    deathPileObjs = new List<GameObject>();
  }

  // Use Old Pos for enemy movement and newPos for if player has lost
  public static bool moveEnemies(Vector3 oldPos, Vector3 newPos) {
    string stringPlayerPos = MapService.vec3ToStringKey(newPos);
    Tilemap tilemap = MapService.getTilemap();
    HashSet<GameObject> objsToDestroy = new HashSet<GameObject>();

    Dictionary<DirectionEnum, Vector3> localDirVectDict = EnemyService.getDirVectDict();

    List<string> movedEnemyPos = new List<string>();

    // move each enemy in the enemy list
    enemyList.ForEach(enemy => {

      Vector3 rawDir = (oldPos - enemy.transform.position).normalized;
      Vector3 dir = new Vector3();
      DirectionEnum dirEnum;

      if (rawDir.x > 0.85f) {
        dirEnum = DirectionEnum.E;
      } else if (rawDir.x > 0) {
        dirEnum = rawDir.y > 0 ? DirectionEnum.NE : DirectionEnum.SE;
      } else if (rawDir.x > -0.85f) {
        dirEnum = rawDir.y > 0 ? DirectionEnum.NW : DirectionEnum.SW;
      } else {
        dirEnum = DirectionEnum.W;
      }
      dir = localDirVectDict[dirEnum];

      // Cords of target tile
      Vector3Int cellCords = tilemap.LocalToCell(enemy.transform.position + dir - tileCharDif);

      if (tilemap.HasTile(cellCords) && tilemap.GetTile(cellCords).name != TileEnum.Bush.ToString() && !tilemap.HasTile(cellCords + new Vector3Int(0, 0, 1))) {

        Vector3 localCords = tilemap.CellToLocal(cellCords);
        enemy.transform.position = localCords + tileCharDif;
      } else if (tilemap.HasTile(cellCords + new Vector3Int(0, 0, 1))) {
        bool canMove = true;
        switch (dirEnum) {
          case DirectionEnum.E:
            dirEnum = rawDir.y > 0 ? DirectionEnum.NE : DirectionEnum.SE;
            break;
          case DirectionEnum.NE:
            dirEnum = rawDir.y > rawDir.x ? DirectionEnum.NW : DirectionEnum.E;
            break;
          case DirectionEnum.SE:
            dirEnum = rawDir.y > rawDir.x ? DirectionEnum.E : DirectionEnum.SW;
            break;
          case DirectionEnum.NW:
            dirEnum = rawDir.y > rawDir.x ? DirectionEnum.NE : DirectionEnum.W;
            break;
          case DirectionEnum.SW:
            dirEnum = rawDir.y > rawDir.x ? DirectionEnum.W : DirectionEnum.SE;
            break;
          case DirectionEnum.W:
            dirEnum = rawDir.y > 0 ? DirectionEnum.NW : DirectionEnum.SW;
            break;
          default:
            canMove = false;
            string enemyPosString = MapService.vec3ToStringKey(enemy.transform.position);
            Vector3 deathPosVec = MapService.stringKeyToVec3(enemyPosString);
            objsToDestroy.Add(enemy);
            deathPileObjs.Add(Instantiate(deathPilePrefab, deathPosVec, new Quaternion()));
            break;
        }

        if (canMove) {
          dir = localDirVectDict[dirEnum];
          cellCords = tilemap.LocalToCell(enemy.transform.position + dir - tileCharDif);
          if (tilemap.HasTile(cellCords) && tilemap.GetTile(cellCords).name != TileEnum.Bush.ToString() && !tilemap.HasTile(cellCords + new Vector3Int(0, 0, 1))) {
            Vector3 localCords = tilemap.CellToLocal(cellCords);
            enemy.transform.position = localCords + tileCharDif;
          }
        }
      }
    });

    // Resolve Enemy Collisions
    Dictionary<string, GameObject> posDict = new Dictionary<string, GameObject>();

    enemyList.ForEach(enemy => {
      // TODO: If in same position as another enemy, collapse enemy there
      string enemyPosString = MapService.vec3ToStringKey(enemy.transform.position);
      if (enemyPosString == stringPlayerPos) {
        // TODO: End the game
        Debug.Log("Game Over");
      } else if (deathPileList.Contains(enemyPosString)) {
        Debug.Log("Death Pile Found");
        objsToDestroy.Add(enemy);
      } else if (posDict.ContainsKey(enemyPosString)) {
        // New Deathpile
        Debug.Log("Collision Detected");
        deathPileList.Add(enemyPosString);
        Vector3 deathPosVec = MapService.stringKeyToVec3(enemyPosString);
        deathPileObjs.Add(Instantiate(deathPilePrefab, deathPosVec, new Quaternion()));
        objsToDestroy.Add(enemy);
        objsToDestroy.Add(posDict[enemyPosString]);
      } else {
        posDict.Add(enemyPosString, enemy);
      }
    });

    foreach (GameObject obj in objsToDestroy) {
      enemyList.Remove(obj);
      Destroy(obj);
    }

    return true;
  }
}
