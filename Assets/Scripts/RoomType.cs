using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
  public int type;
  private Collider2D col;

  public void Start() {
    col = GetComponent<Collider2D>();
  }

  public void RoomDestruction() {
    // remove RoomTemplate type
    if(gameObject.transform.parent && gameObject.transform.parent.gameObject) {
      Destroy(gameObject.transform.parent.gameObject);
    } else {
      Destroy(gameObject);
    }
  }

  public void DisableCollider() {
    Debug.Log("DisableCollider");
    col.enabled = false;
  }
}
