using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    private LevelGeneration levelGen;

    void Start() {
      levelGen = GameObject.FindWithTag("LevelGenerator").GetComponent<LevelGeneration>();
    }

    // Update is called once per frame
    void Update()
    {
      if(levelGen.stopGeneration == true) {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
        if(roomDetection == null) {
          int rand = Random.Range(0, levelGen.rooms.Length);
          Instantiate(levelGen.rooms[rand], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
      }
    }
}
