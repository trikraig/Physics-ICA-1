using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
   public Transform spawnPoint;
   public GameObject prefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("FIRE CANNON");
            Instantiate(prefab, spawnPoint);
        }
    }
}
