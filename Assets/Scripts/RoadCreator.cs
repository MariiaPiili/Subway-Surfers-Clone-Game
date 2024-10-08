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
