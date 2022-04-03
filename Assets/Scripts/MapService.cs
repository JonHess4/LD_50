using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapService {
  private static Tilemap tilemap;
  private static Dictionary<string, TileBase> tileDict = null;
  private static List<Vector3Int> tileDictKeys = null;

  // private static Tile waterTile;
  // private static Tile grassTile;
  // private static Tile bushTile;

  public static Tilemap getTilemap() {
    if (!tilemap) {
      tilemap = Object.FindObjectOfType<Tilemap>();
      tilemap.CompressBounds();
    }
    return tilemap;
  }

  public static Dictionary<string, TileBase> getTileDict() {
    if (tileDict == null || tileDict.Count <= 0) {
      MapService.processTilemap(MapService.getTilemap());
    }
    return tileDict;
  }

  public static List<Vector3Int> getTileDictKeys() {
    if (tileDictKeys == null || tileDictKeys.Count <= 0) {
      MapService.processTilemap(MapService.getTilemap());
    }
    return tileDictKeys;
  }

  private static void processTilemap(Tilemap tilemap) {
    tileDict = new Dictionary<string, TileBase>();
    tileDictKeys = new List<Vector3Int>();

    BoundsInt bounds = tilemap.cellBounds;
    UnityEngine.Tilemaps.TileBase[] tileList = tilemap.GetTilesBlock(bounds);

    // int xHalf = (bounds.size.x / 2) + bounds.x;
    // int yHalf = (bounds.size.y / 2) + bounds.y;
    // int xMax = bounds.size.x + bounds.x;
    // int yMax = bounds.size.y + bounds.y;

    int index = 0;
    for (int i = 0; i < bounds.size.y; i++) {
      for (int j = 0; j < bounds.size.x; j++) {
        for (int k = 0; k < bounds.size.z; k++) {
          Vector3Int cellCoords = new Vector3Int(bounds.x + j, bounds.y + i, bounds.z + k);

          // if (
          //   (cellCoords.y < -1 * cellCoords.x + (yHalf + 1 * bounds.x)) ||
          //   (cellCoords.y > -1 * cellCoords.x + (yHalf + 1 * xMax)) ||
          //   (cellCoords.y < 1 * cellCoords.x + (yHalf - 1 * xMax)) ||
          //   (cellCoords.y > 1 * cellCoords.x + (yMax - 1 * xHalf))
          // ) {
          //   continue;
          // }

          // int randNum = Random.Range(0, 4);
          // if (k == 0) {
          //   if (randNum <= 2) {
          //     tilemap.SetTile(cellCoords, getTile(TileEnum.Grass));
          //   } else {
          //     tilemap.SetTile(cellCoords, getTile(TileEnum.Water));
          //   }
          // } else if (k == 1 && randNum > 2 && tilemap.GetTile(cellCoords - new Vector3Int(0, 0, 1)).name == TileEnum.Grass.ToString()) {
          //   tilemap.SetTile(cellCoords, getTile(TileEnum.Bush));
          // }

          tileDict.Add(MapService.vec3ToStringKey(cellCoords), tilemap.GetTile(cellCoords));
          tileDictKeys.Add(cellCoords);
          index++;
        }
      }
    }
  }


  public static string vec3ToStringKey(Vector3 coords) {
    return coords.x + "," + coords.y + "," + coords.z;
  }

  public static Vector3 stringKeyToVec3(string key) {
    string[] nums = key.Split(',');
    return new Vector3(float.Parse(nums[0]), float.Parse(nums[1]), float.Parse(nums[2]));
  }

  // private static Tile getTile(TileEnum tileEnum) {
  //   Tile tile = null;

  //   if (tileEnum == TileEnum.Water) {
  //     if (!waterTile) {
  //       // waterTile = ScriptableObject.CreateInstance<Tile>();
  //       waterTile = Resources.Load<Tile>(tileEnum.ToString());
  //     }
  //     tile = waterTile;
  //   } else if (tileEnum == TileEnum.Grass) {
  //     if (!grassTile) {
  //       // grassTile = ScriptableObject.CreateInstance<Tile>();
  //       grassTile = Resources.Load<Tile>(tileEnum.ToString());
  //     }
  //     tile = grassTile;
  //   } else if (tileEnum == TileEnum.Bush) {
  //     if (!bushTile) {
  //       // bushTile = ScriptableObject.CreateInstance<Tile>();
  //       bushTile = Resources.Load<Tile>(tileEnum.ToString());
  //     }
  //     tile = bushTile;
  //   }

  //   return tile;
  // }
}


// if(tileList[index] != null) {

//   Debug.Log("*_*_*_*_*_*_*_*_*_*_*_*_*_*_*");
//   string name = tileList[index] != null ? tileList[index].name : "null";
// index: <tilename> (<tile xyz coords)
//   Debug.Log(index + ": " + name + " (" + (bounds.x + j) + ", " + (bounds.y + i) + ", " + (bounds.z + k) + ")");

//   Vector3 worldCoords = tilemap.CellToWorld(new Vector3Int(bounds.x + j, bounds.y + i, bounds.z + k));
//   Debug.Log("World Coords: " + worldCoords + " (Cell: " + tilemap.WorldToCell(worldCoords) + ")");

//   Vector3 localCoords = tilemap.CellToLocal(new Vector3Int(bounds.x + j, bounds.y + i, bounds.z + k));
//   Debug.Log("Local Coords: " + localCoords + " (Cell: " + tilemap.LocalToCell(localCoords) + ")");
//   Debug.Log("**********************************************************************");
// }
