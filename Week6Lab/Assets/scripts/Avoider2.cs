using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Avoider2 : MonoBehaviour
{
    public GameObject Spotter;
    public bool Visuals = true;
    public int searchAreaSize = 10;
    public float searchCellSize = 0.3f;
    public float speed = 10f;

    float Range = 20f;
    float maxDistance = 5f;

    NavMeshAgent agent;

    Timer avoidLoopTimer = new Timer();
    float maxTimer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        NoErrors(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(NoErrors(0))
        {
            int timerStatus = avoidLoopTimer.LoopingTimer(maxTimer);

            if(timerStatus == 0)
            {
                FindHidingSpot();
            }
        }
    }

    void FindHidingSpot()
    {
        PoissonDiscSampler distribution = new PoissonDiscSampler(searchAreaSize, searchAreaSize, searchCellSize);
        List<Vector3> candidateHidingSpots = new List<Vector3>();

        foreach(Vector2 point in distribution.Samples())
        {
            Vector3 point3D = new Vector3(point.x, 0, point.y);
            if(Vector3.Distance(transform.position, point3D) < maxDistance)
            {
                if(CheckVisibility(Spotter.transform.position, point3D))
                {
                    candidateHidingSpots.Add(point3D);
                }
            }

            DrawRay(transform.position, point3D, Color.green, Range);
        }

        if(candidateHidingSpots.Count > 0)
        {
            Vector3 closestSpot = candidateHidingSpots[0];
            foreach(Vector3 point in candidateHidingSpots)
            {
                if(Vector3.Distance(transform.position, point) < Vector3.Distance(transform.position, closestSpot))
                {
                    closestSpot = point;
                }
            }

            if(agent.speed != speed)
            {
                agent.speed = speed;
            }
            agent.SetDestination(closestSpot);
        }
    }

    bool CheckVisibility(Vector3 startPos, Vector3 endPos)
    {
        bool canSee = false;
        Ray ray = new Ray(startPos, endPos);
        RaycastHit hit;

        // Draws a line in scene
        DrawRay(startPos, endPos, Color.green, Range);

        if(Physics.Raycast(ray, out hit, Range))
        {
            if(hit.collider.gameObject != Spotter)
            {
                canSee = true;
            }
        }

        return canSee;
    }

    #region Debug Methods
    void DrawRay(Vector3 startPos, Vector3 endPos, Color color, float range)
    {
        if(Visuals == true)
        {
            Debug.DrawRay(startPos, endPos, color, range);
        }
    }

    // Checks for possible errors, prints them if shouldPrint != 0
    bool NoErrors(int shouldPrint)
    {
        if (agent == null)
        {
            PrintError("No NavMeshAgent component found", shouldPrint);
            return false;
        }
        else if (!agent.isOnNavMesh)
        {
            PrintError("No NavMesh Surface found. Please bake a NavMesh", shouldPrint);
            return false;
        }

        if (Spotter == null)
        {
            PrintError("No Spotter GameObject found", shouldPrint);
            return false;
        }

        return true;
    }

    // Prints a warning message in console. Doesn't print if shouldPrint == 0
    void PrintError(string message, int shouldPrint)
    {
        if(shouldPrint != 0)
        {
            Debug.LogWarning(message);
        }
    }
    #endregion
}
