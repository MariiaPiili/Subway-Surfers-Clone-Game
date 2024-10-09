# Subway Surfers Clone Game

This game is a clone of the popular endless runner Subway Surfers, where the player continuously moves forward on a road with three lanes. The objective is to dodge obstacles by switching lanes and jumping. 

## Player's movement сontrol

`Player.cs` handles the player’s movement, jumping, and collision detection with obstacles. 

The character automatically moves forward, simulating a continuous sprint. The player can switch lanes by pressing the A and D keys to move left and right respectively. Additionally the player can make the character jump by pressing the Space key to avoid obstacles that block their path.

- Move Left: A
- Move Rigth: D
- Space: Jump

`OnCollisionStay()` method checks if the player is touching the ground. When the player is in contact with a surface, it marks them as "grounded." This is crucial because the player can only jump when they’re on solid ground, preventing mid-air jumps and ensuring realistic movement.

If the player collides with an obstacle, `OnCollisionEnter()` is triggered. This method instantly restarts the game whenever the player makes contact with anything designated as an obstacle. It serves as a way to end the current run and reset the game if the player fails to avoid obstacles.

`OnCollisionExit()`  method detects when the player leaves the ground, whether they jump or fall. As soon as the player is no longer in contact with a surface, this method unmarks them as grounded. This prevents the player from jumping again until they land back on the ground, enforcing a realistic jumping mechanic.

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _changeRoadSpeed;

    private Rigidbody _rigidbody;
    private int _chosenRoad;
    private bool _jumpEnabled;
    private bool _isGrounded;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _chosenRoad = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _chosenRoad++;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _chosenRoad--;
        }

        MovementHorizontal();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpEnabled = true;
        }
    }

    private void FixedUpdate()
    {
        MovementVertical();
        Jump();
    }

    private void MovementVertical()
    {
        _rigidbody.velocity = new Vector3(_speed, _rigidbody.velocity.y, _rigidbody.velocity.z);
    }

    private void MovementHorizontal()
    {
        _chosenRoad = Mathf.Clamp(_chosenRoad, -1, 1);
        Vector3 position = transform.position;
        position.z = _chosenRoad;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _changeRoadSpeed);
    }

    private void Jump()
    {
        if (_jumpEnabled)
        {
            _jumpEnabled = false;

            if (_isGrounded)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        float dot = Vector3.Dot(normal, Vector3.up);
        if (dot > 0.5f)
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
```

`Obstacle.cs` designates objects as obstacles on the road. It does not contain any methods, but it allows the `Player.cs` to recognize objects as obstacles via GetComponent<Obstacle>().

```csharp
using UnityEngine;

public class Obstacle : MonoBehaviour
{
}
```

## Infinite Road

`RoadCreator.cs` is responsible for creating and managing the road segments that the player moves across. It keeps the road continuous and endless by reusing road segments as the player moves forward.

When the game starts, `Start()` method calls `RoadSpawn()`, which sets up the initial road segments. In this method, the game instantiates a specified number of road segments using prefabs and positions them sequentially. Each segment is spaced apart according to its size and then added to a queue called _createdRoads. This queue keeps track of the order of the road segments.

`Update()` method then continuously calls `RoadShuffling()`, which determines if any of the road segments need to be moved. This method is important for creating the illusion of an endless road. It checks if the first road segment in the queue has moved out of the player’s view, meaning it has gone behind them. When this happens, the segment is taken from the front of the queue, moved to the end of the road, and added back to the queue. This reuses the road segment by putting it in front of the player again, creating a smooth, endless path.

Through this recycling process, `RoadCreator.cs` maintains a continuous road without constantly generating new segments. 

```csharp
using System.Collections.Generic;
using UnityEngine;

public class RoadCreator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private List<GameObject> _roadPrefabList = new List<GameObject>();
    [SerializeField] private int _roadAmount;
    [SerializeField] private int _roadSize;

    private Queue<GameObject> _createdRoads = new Queue<GameObject>();
    private void Start()
    {
        RoadSpawn();
    }

    private void Update()
    {
        RoadShuffling();
    }

    private void RoadSpawn()
    {
        for (int i = 0; i < _roadAmount; i++)
        {
            GameObject road = Instantiate(_roadPrefabList[Random.Range(0, _roadPrefabList.Count)], new Vector3(i * _roadSize, 0f, 0f), Quaternion.identity);
            _createdRoads.Enqueue(road);
        }
    }

    private void RoadShuffling()
    {
        if (_createdRoads.Peek().transform.position.x + _roadSize/2 < _player.position.x)
        {
            GameObject road = _createdRoads.Dequeue();
            _createdRoads.Enqueue(road);
            Vector3 position = road.transform.position;
            position.x += _roadSize * _roadAmount;
            road.transform.position = position;
        }
    }
}
```

## Requirements

Unity Game Engine



 
