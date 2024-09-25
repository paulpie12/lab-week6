using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class Avoider : MonoBehaviour
{
    public GameObject Spotter;

    private Vector3 pos;
    private List<Vector2> samples = new List<Vector2>();
    private int test = 0;

    [SerializeField] private float Range = 20;
    [SerializeField] private bool Visuals = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //find point location
        Gizmos.DrawLine(pos,samples[test]);
    }


    void Start()
    {
        
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        pos = transform.position;


        // error logs and connection checks
        if (agent == null)
        {
            Debug.LogWarning("No Navemash Componant Found");
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("No NavMesh Surface Found");
        }

        if (Spotter == null)
        {
            Debug.LogError("No Spotter GameObject Found");
        }

    }


    void Update()
    {
        FindSpots();
        
    }

    void FindSpots()
    {
        PoissonDiscSampler sampler = new PoissonDiscSampler(10, 10, 0.3f);

       


        foreach( var point in sampler.Samples())
        {
            samples.Add(point);
            var ray = new Ray(this.transform.position, point);
            RaycastHit hit;

            test++;
        }

    }


}
