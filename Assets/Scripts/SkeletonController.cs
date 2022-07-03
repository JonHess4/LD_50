using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour {

  public float moveSpeed = 5f;
  public Transform movePoint;

  void Start() {
      this.movePoint.parent = null;
  }

  void Update() {

  }
}
