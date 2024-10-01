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

    [SerializeField] private float Range = 20f;
    [SerializeField] private bool Visuals = true;
    [SerializeField] private float sampleRadius = 0.3f;

    NavMeshAgent agent;

    Timer avoidLoopTimer = new Timer();
    float maxTimer = 0.5f;

    public delegate void VisualsDelegate(Vector3 startPos, Vector3 endPos, Color color, float range);

    private void OnDrawGizmos()
    {
        if (samples.Count > 0)
        {
            Gizmos.color = Color.red;

            // Draw Gizmos for each ray in the samples list
            foreach (var sample in samples)
            {
                Vector3 samplePoint3D = new Vector3(sample.x, 0, sample.y);
                //Gizmos.DrawLine(pos, samplePoint3D); // Draw a line from the origin to the sample point
                DrawRay(pos, samplePoint3D, Color.red, samplePoint3D.magnitude); // not a gizmo, but can be changed to one within method
            }
        }
    }

    void Start()
    {
        // assigns DrawRay to VisualsDelegate
        VisualsDelegate VDhandler = DrawRay;

        agent = GetComponent<NavMeshAgent>();
        pos = transform.position;

        // Error handling
        if (agent == null)
        {
            Debug.LogWarning("No NavMeshAgent component found");
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("No NavMesh Surface found");
        }

        if (Spotter == null)
        {
            Debug.LogError("No Spotter GameObject found");
        }

        // Initialize raycasting points
        FindSpots();
    }

    void Update()
    {
        // Optional: Call FindSpots() in Update if you want to update rays every frame
        // FindSpots();

        // Calls AvoidLoop() on a timer
        int timerStatus = avoidLoopTimer.LoopingTimer(maxTimer);

        if(timerStatus == 0)
        {
            AvoidLoop();
        }
    }

    void FindSpots()
    {
        // Clear the previous samples to avoid accumulating new points on each frame
        samples.Clear();

        // Use Poisson Disc sampling to generate random points
        PoissonDiscSampler sampler = new PoissonDiscSampler(10, 10, sampleRadius);

        // Loop through the generated sample points
        foreach (var point in sampler.Samples())
        {
            //samples.Add(point); // Add to the samples list for Gizmo drawing
            Vector3 targetPoint = new Vector3(point.x, 0, point.y);

            // Create the ray from current position to the target point
            var ray = new Ray(this.transform.position, targetPoint - this.transform.position);
            RaycastHit hit;

            // Perform the raycast and log if anything is hit
            if (Physics.Raycast(ray, out hit, Range))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                if(!CheckVisibility(Spotter.transform.position, targetPoint))
                {
                    samples.Add(point); // Only adds samples not visible to the Spotter
                }
            }

            // Visualize the ray in the Scene and Game view using Debug.DrawRay
            DrawRay(this.transform.position, (targetPoint - this.transform.position).normalized * Range, Color.green, 0.5f);
        }
    }

    #region Avoid Logic
    // Checks if startPos can see whatever's at endPos.
    // startPos is the Spotter/Player.
    bool CheckVisibility(Vector3 startPos, Vector3 endPos)
    {
        bool canSee = false;
        Ray ray = new Ray(startPos, endPos);
        RaycastHit hit;

        // Draws a line in scene
        DrawRay(startPos, endPos.normalized, Color.green, Range);

        if(Physics.Raycast(ray, out hit, Range))
        {
            if(hit.collider.gameObject != Spotter)
            {
                canSee = true;
            }
        }

        return canSee;
    }

    // Logic for detecting avoidee
    // Casts a ray in a certain direction and checks if it's the Spotter
    void Avoid(Vector3 rayDirection)
    {
        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit hit;

        // Draws a line in scene
        DrawRay(transform.position, rayDirection, Color.green, Range);

        if(Physics.Raycast(ray, out hit, Range))
        {
            // Checks for Spotter, searches for and moves to closest point if true
            if(hit.collider.gameObject == Spotter)
            {
                FindClosestSpot();
            }
        }
    }

    // Calls Avoid() four times: once for each cardinal direction.
    // Currently exists for testing purposes
    void AvoidLoop()
    {
        Avoid(transform.forward);
        Avoid(-transform.forward);
        Avoid(transform.right);
        Avoid(-transform.right);
    }

    // Searches through samples and finds spot closest to Avoider
    // Moves to that spot
    void FindClosestSpot()
    {
        Vector3 closestSpot = (Vector3)samples[0];
        //Debug.Log("current position: " + transform.position);

        // Searches for the closest sample in samples
        foreach(Vector2 sample in samples)
        {
            Vector3 sample3D = (Vector3)sample;
            if(Vector3.Distance(transform.position, sample3D) < Vector3.Distance(transform.position, closestSpot))
            {
                // Checks if Spotter can see the sample, uses that spot if not
                if(!CheckVisibility(Spotter.transform.position, sample3D))
                {
                    closestSpot = sample3D;
                }
            }
        }

        // Moves this object to closestSpot
        agent.SetDestination(closestSpot);
        
        //Debug.Log("closest spot found: " + closestSpot);
    }
    #endregion

    #region Delegate Methods
    void DrawRay(Vector3 startPos, Vector3 endPos, Color color, float range)
    {
        if(Visuals == true)
        {
            Debug.DrawRay(startPos, endPos, color, range);
        }
    }
    #endregion
}