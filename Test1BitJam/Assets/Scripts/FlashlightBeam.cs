using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FlashlightBeam : MonoBehaviour
{
    [Header("Beam Settings")]
    public float maxDistance = 10f;
    [Range(10f, 90f)] public float beamAngle = 30f;
    public int resolution = 20;
    public LayerMask detectionLayers;
    public float smoothingSpeed = 15f;
    public Light light;
    private Mesh beamMesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float currentLength;
    private float targetLength;

    void Start()
    {
        beamMesh = new Mesh {
            name = "ProceduralFlashlightCone" };
        GetComponent<MeshFilter>().mesh = beamMesh;

        currentLength = maxDistance;

        GenerateMeshStructure();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, detectionLayers))
        {
            targetLength = hit.distance;
        }
        else
        {
            targetLength = maxDistance;
        }

        currentLength = Mathf.Lerp(currentLength, targetLength, Time.deltaTime * smoothingSpeed);
        light.range = targetLength + 2f;

        UpdateMeshVertices(currentLength);
    }

    private void GenerateMeshStructure()
    {
        vertices = new Vector3[resolution + 1];
        triangles = new int[resolution * 3];

        vertices[0] = Vector3.zero;

        beamMesh.vertices = vertices;

        for (int i = 0; i < resolution; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % resolution + 1;
        }

        beamMesh.triangles = triangles;
    }

    private void UpdateMeshVertices(float length)
    {
        float radius = length * Mathf.Tan((beamAngle / 2f) * Mathf.Deg2Rad);
        float angleStep = 360f / resolution;

        vertices[0] = Vector3.zero;

        for (int i = 0; i < resolution; i++)
        {
            float currentAngle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(currentAngle) * radius;
            float y = Mathf.Sin(currentAngle) * radius;

            vertices[i + 1] = new Vector3(x, y, length);
        }

        beamMesh.vertices = vertices;

        beamMesh.RecalculateBounds();
    }
}