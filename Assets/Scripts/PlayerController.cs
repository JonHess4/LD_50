using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {
  public Rigidbody2D body;
  private bool canMove = true;
  private float elapsedTime;
  private float diagonalDistanceMod = 0.5f;
  private float zIndexGroundDiff = 1f;
  private float yIndexGroundDiff = 1f;
  private float walkAnimationTime = .5f;
  private bool hasTriedMove = false;
  // move delay to allow for consistent diagonal movement and changing face without moving
  private float moveDelay = 0.05f;

  // Update is called once per frame
  void Update() {
    // base enemy pathing on old position
    Vector3 oldPos = new Vector3(this.body.transform.position.x, this.body.transform.position.y, this.body.transform.position.z);

    // if can move
    if (this.canMove && !EnemyService.isGameOver) {
      // get movement input
      Vector2Int movementInput = this.getMovementInput();
      this.hasTriedMove = this.resolveMovementInput(movementInput);
    }

    if (this.hasTriedMove) {
      // halt user inputs while resolving NPC movement
      this.canMove = false;
      this.canMove = EnemyService.moveEnemies(oldPos, this.body.transform.position);
    }
    this.hasTriedMove = false;
  }

  private Vector2Int getMovementInput() {
    Vector2Int direction = new Vector2Int();

    float xAxis = Input.GetAxis(AxisEnum.Horizontal.ToString());
    float yAxis = Input.GetAxis(AxisEnum.Vertical.ToString());

    bool isDiagonalOnly = Input.GetButton("Fire2");

    if ((isDiagonalOnly && xAxis != 0 && yAxis != 0) || (!isDiagonalOnly && (xAxis != 0 || yAxis != 0))) {

      // restrain each axis value to -1, 0, or 1 for maths
      int xInt = xAxis > 0 ? 1 : (xAxis < 0 ? -1 : 0);
      int yInt = yAxis > 0 ? 1 : (yAxis < 0 ? -1 : 0);

      direction.x = xInt;
      direction.y = yInt;
    }

    return direction;
  }

  private bool resolveMovementInput(Vector2Int movementInput) {
    bool hasTriedMove = false;

    if (movementInput.x == 0 && movementInput.y == 0) {
      // no direction inputs detected, clear out elapsed time
      // set to a negative to act as a buffer for future inputs
      this.elapsedTime = -1 * this.moveDelay;

    } else if (this.elapsedTime <= 0) {
      // user just started moving, call tryMove and start tracking elapsed time
      this.elapsedTime += Time.deltaTime;
      if (this.elapsedTime > 0) {
        this.tryMove(this.body.transform.position, movementInput);
        hasTriedMove = true;
      }
    } else {
      // user is continuing to move, wait until move animation is finished to update their position again
      this.elapsedTime += Time.deltaTime;
      if (this.elapsedTime >= this.walkAnimationTime) {
        this.elapsedTime = this.elapsedTime % this.walkAnimationTime;
        this.tryMove(this.body.transform.position, movementInput);
        hasTriedMove = true;
      }
    }

    return hasTriedMove;
  }

  private void tryMove(Vector3 startingPos, Vector2Int movementInput) {
    Vector2 processedMovementInput = this.processMovementInput(movementInput);
    //  get target coords
    Vector3 targetTileLocalPos = this.getTargetTilePos(this.body.transform.position, processedMovementInput);

    Tilemap tileMap = MapService.getTilemap();

    Vector3Int targetTileCellPos = tileMap.LocalToCell(targetTileLocalPos);

    if (tileMap.HasTile(targetTileCellPos) && !tileMap.HasTile(targetTileCellPos + new Vector3Int(0, 0, 1)) && tileMap.GetTile(targetTileCellPos).name != TileEnum.Water.ToString()) {
      targetTileLocalPos.z += 1;
      targetTileLocalPos.y += this.yIndexGroundDiff;
      this.body.transform.position = targetTileLocalPos;
    }
  }

  private Vector2 processMovementInput(Vector2Int direction) {
    Vector2 processedDirection = new Vector2();

    if (direction.x != 0 && direction.y != 0) {
      processedDirection.x = this.diagonalDistanceMod * direction.x;
      processedDirection.y = this.diagonalDistanceMod * direction.y;
    } else {
      processedDirection.x = (float)direction.x;
      processedDirection.y = (float)direction.y;
    }

    return processedDirection;
  }

  private Vector3 getTargetTilePos(Vector3 startingPos, Vector2 movementInput) {
    Vector3 targetTilePos = new Vector3();

    targetTilePos.x = startingPos.x + movementInput.x;
    targetTilePos.y = startingPos.y + movementInput.y - this.yIndexGroundDiff;
    targetTilePos.z = startingPos.z - this.zIndexGroundDiff;

    return targetTilePos;
  }
}
