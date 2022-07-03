 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTheCube : MonoBehaviour
{

    [HideInInspector]
    public int numberOfDestroy;
    [HideInInspector]
    public bool enemy;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("GetCube"))
        {
            numberOfDestroy+=1;
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("EnemyCube"))
        {
            Destroy(other.gameObject);
            enemy=true;
        }
        
    }
}
