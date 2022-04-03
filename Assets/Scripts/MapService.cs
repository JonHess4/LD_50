using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapService {
  private static Tilemap tilemap;
  private static Dictionary<string, TileBase> tileDict = null;
  private static List<Vector3Int> tileDictKeys = null;

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

    int index = 0;
    for (int i = 0; i < bounds.size.y; i++) {
      for (int j = 0; j < bounds.size.x; j++) {
        for (int k = 0; k < bounds.size.z; k++) {
          Vector3Int cellCoords = new Vector3Int(bounds.x + j, bounds.y + i, bounds.z + k);

          tileDict.Add(MapService.vec3ToStringKey(cellCoords), tileList[index]);
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
