using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target; //플레이어
    [SerializeField]
    private GameObject enemySpawnPointPrefab; //등장위치
    [SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField]
    private float enemySpawnTime = 1;  //생성주기
    [SerializeField]
    private float enemySpawnLatency = 1; //대기시간

    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;

    private int numberOfEnemiesSpawnedAtOnce = 1; //적 숫자
    private Vector2Int mapSize = new Vector2Int(100, 100);

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }
    private void Update()
    {
        
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            for(int i= 0;i<numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();
                //item.transform.position = new Vector3(target.position.x, 0, 45); //스폰위치 일렬
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.24f, mapSize.x * 0.24f), 0, Random.Range(-mapSize.y * 0.24f, mapSize.y * 0.24f));

                StartCoroutine("SpawnEnemy", item);
            }
            currentNumber++;
            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
    }
    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position; // 적위치를 point로 설정

        item.GetComponent<EnemyFSM>().Setup(target, this);

        spawnPointMemoryPool.DeactivatePoolItem(point); 
    }
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }

    

}
