using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Avoider : MonoBehaviour
{
    public GameObject spotter;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogWarning("No Navemash Componant Found");
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("No NavMesh Surface Found");
        }

        if (spotter == null)
        {
            Debug.LogError("No Spotter GameObject Found");
        }
       
  


    }


    void Update()
    {
        
    }
}
