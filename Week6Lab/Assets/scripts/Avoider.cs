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

    private void OnDrawGizmos()
    {
        if (samples.Count > 0)
        {
            Gizmos.color = Color.red;

            // Draw Gizmos for each ray in the samples list
            foreach (var sample in samples)
            {
                Vector3 samplePoint3D = new Vector3(sample.x, 0, sample.y);
                Gizmos.DrawLine(pos, samplePoint3D); // Draw a line from the origin to the sample point
            }
        }
    }

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
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
            samples.Add(point); // Add to the samples list for Gizmo drawing
            Vector3 targetPoint = new Vector3(point.x, 0, point.y);

            // Create the ray from current position to the target point
            var ray = new Ray(this.transform.position, targetPoint - this.transform.position);
            RaycastHit hit;

            // Perform the raycast and log if anything is hit
            if (Physics.Raycast(ray, out hit, Range))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }

            // Visualize the ray in the Scene and Game view using Debug.DrawRay
            Debug.DrawRay(this.transform.position, (targetPoint - this.transform.position).normalized * Range, Color.green, 0.5f);
        }
    }
}