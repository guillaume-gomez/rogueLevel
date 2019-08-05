using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    public List<Transform> startingPositions;
    public GameObject posePrefab;
    public GameObject[] rooms;
    // index 0 => LR
    // index 1 => LRB
    // index 2 => LRT
    // index 3 => LRBT

    private int direction;
    public float moveAmount;
    private int downCounter;

    private float timeBtwRoom;
    public float startTimeBtwRoom = 0.25f;

    public float minX = -5;
    public float maxX = 25;
    public float minY = -25;
    public float maxY = 25;
    private float step = 10;
    public bool stopGeneration = false;
    public GameObject player;

    public LayerMask room;

    void Start()
    {
      //SetUpTopPoses();
      FillLayout();
      StartGeneration();
    }

    void SetUpTopPoses() {
      for(int i = 0; i < 4; i++) {
        Vector2 pos = new Vector2(minX + i * step, 5);
        GameObject instance = Instantiate(posePrefab, pos, Quaternion.identity);
        startingPositions.Add(instance.transform);
      }
    }


    void FillLayout()
    {
      // the difference to fit in the borders
      float offsetY = 20;
      for(float x = minX; x <= maxX; x += step) {
        for(float y = minY + offsetY; y <= maxY; y += step) {
          Instantiate(posePrefab, new Vector2(x, -y), Quaternion.identity);
        }
      }
    }

    void StartGeneration() {
      int randStartingPosition = Random.Range(0, startingPositions.Count);
      transform.position = startingPositions[randStartingPosition].position;

      CreateRoom(0, rooms.Length);
      direction = Random.Range(1, 6);
    }

    public void CreateRoom(int min, int max, Vector3 pos) {
      int randomRoomIndex = Random.Range(min, max);
      GameObject instanciateRoom = Instantiate(rooms[randomRoomIndex], pos, Quaternion.identity);
    }

    void CreateRoom(int min, int max) {
      CreateRoom(min, max, transform.position);
    }

    void CreateRoom(int randomRoomIndex)
    {
      GameObject instanciateRoom = Instantiate(rooms[randomRoomIndex], transform.position, Quaternion.identity);
    }

    void DisableColliderInRoom() {
      GameObject[] generatedRooms = GameObject.FindGameObjectsWithTag("RoomTemplates");
      foreach (GameObject currentRoom in generatedRooms) {
        // we assume that the first child is the room
        GameObject roomInRoomTemplate = currentRoom.transform.GetChild(0).gameObject;
        if(roomInRoomTemplate.GetComponent<RoomType>()) {
          roomInRoomTemplate.GetComponent<RoomType>().DisableCollider();
        }
      }
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
          downCounter = 0;
          Vector2 newPos = new Vector2(transform.position.x + moveAmount, transform.position.y);
          transform.position = newPos;

          CreateRoom(0, rooms.Length);

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
          downCounter = 0;
          Vector2 newPos = new Vector2(transform.position.x - moveAmount, transform.position.y);
          transform.position = newPos;

          CreateRoom(0, rooms.Length);

          direction = Random.Range(3, 6);
        } else {
          direction = 5;
        }
      } else if (direction == 5) { // Move Down
        downCounter++;
        if (transform.position.y > minY) {
          Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
          if(roomDetection.GetComponent<RoomType>().type != 1 && roomDetection.GetComponent<RoomType>().type != 3) {
            if(downCounter >= 2) {
              roomDetection.GetComponent<RoomType>().RoomDestruction();
              CreateRoom(3);
            } else {
              roomDetection.GetComponent<RoomType>().RoomDestruction();
              int randomBottomRoom = Random.Range(1, rooms.Length);
              if(randomBottomRoom == 2) {
                randomBottomRoom = 1;
              }
              CreateRoom(randomBottomRoom);
            }
          }

          Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmount);
          transform.position = newPos;

          CreateRoom(2, rooms.Length);
          direction = Random.Range(1, 6);
        } else {
          // stop generation
          stopGeneration = true;
          player.SetActive(true);
          Invoke("DisableColliderInRoom", 5);
          //player.transform.position = new Vector2(-0f, 0.0f);
          return;
        }
      }
    }
}
