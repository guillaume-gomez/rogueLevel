using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    public Transform[] startingPositions;
    public GameObject[] rooms;
    // index 0 => LR
    // index 1 => LRB
    // index 2 => LRT
    // index 3 => LRBT

    private int direction;
    public float moveAmount;

    private float timeBtwRoom;
    public float startTimeBtwRoom = 0.25f;

    public float minX = -5;
    public float maxX = 25;
    public float minY = -25;
    private bool stopGeneration = false;

    public LayerMask room;
    private int downCounter;

    void Start()
    {
        int randStartingPosition = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPosition].position;

        int randomRoomIndex = Random.Range(0, rooms.Length);
        Instantiate(rooms[randomRoomIndex], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6);
    }

    void Update() {
      if(timeBtwRoom <= 0 && !stopGeneration) {
        Move();
        timeBtwRoom = startTimeBtwRoom;
      } else {
        timeBtwRoom -= Time.deltaTime;
      }
    }

    private void Move() {
      if(direction == 1 || direction == 2) { // Move Right
        if (transform.position.x < maxX) {
          Vector2 newPos = new Vector2(transform.position.x + moveAmount, transform.position.y);
          transform.position = newPos;

          int rand = Random.Range(0, rooms.Length);
          Instantiate(rooms[rand], transform.position, Quaternion.identity);

          direction = Random.Range(1, 6);
          if(direction == 3) {
            direction = 2;
          } else if(direction == 4) {
            direction = 5;
          }
        } else {
          direction = 5;
        }
      } else if (direction == 3 || direction == 4) { // Move Left
        if (transform.position.x > minX) {
          Vector2 newPos = new Vector2(transform.position.x - moveAmount, transform.position.y);
          transform.position = newPos;

          int rand = Random.Range(0, rooms.Length);
          Instantiate(rooms[rand], transform.position, Quaternion.identity);

          direction = Random.Range(3, 6);
        } else {
          direction = 5;
        }
      } else if (direction == 5) { // Move Down
        downCounter++;
        if (transform.position.y > minY) {

          Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
          if(roomDetection.GetComponent<RoomType>().type != 1 && roomDetection.GetComponent<RoomType>().type != 3) {
            roomDetection.GetComponent<RoomType>().RoomDestruction();

            int randomBottomRoom = Random.Range(1, rooms.Length);
            if(randomBottomRoom == 2) {
              randomBottomRoom = 1;
            }
            Instantiate(rooms[randomBottomRoom], transform.position, Quaternion.identity);
          }

          Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmount);
          transform.position = newPos;

          int rand = Random.Range(2, rooms.Length);
          Instantiate(rooms[rand], transform.position, Quaternion.identity);

          direction = Random.Range(1, 6);
        } else {
          // stop generation
          stopGeneration = true;
          return;
        }
      }
      //Instantiate(rooms[0], transform.position, Quaternion.identity);
    }
}
