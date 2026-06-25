using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    Vector3 target = new Vector3(0, 0, -45.0f);
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,target,0.006f);
    }
}
