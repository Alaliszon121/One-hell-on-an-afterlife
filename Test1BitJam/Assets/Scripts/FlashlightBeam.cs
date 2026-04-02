using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FlashlightBeam : MonoBehaviour
{
    [Header("Beam Settings")]
    public float maxDistance = 10f;
    [Range(10f, 90f)] public float beamAngle = 30f;
    public int resolution = 20;

    [Tooltip("CRITICAL: Ensure your Player layer is NOT selected here, or the beam will hit your own body!")]
    public LayerMask detectionLayers;
    public float smoothingSpeed = 15f;

    public Light attachedLight;

    [Header("Physics Stability")]
    [Tooltip("Gives the beam thickness so it doesn't clip through walls or slip through seams.")]
    public float beamThickness = 0.2f;

    private Mesh beamMesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float currentLength;
    private float targetLength;

    void Start()
    {
        beamMesh = new Mesh { name = "ProceduralFlashlightCone" };
        GetComponent<MeshFilter>().mesh = beamMesh;

        currentLength = maxDistance;

        GenerateMeshStructure();

        UpdateMeshVertices(currentLength);
    }

    void Update()
    {
        if (Physics.SphereCast(transform.position, beamThickness, transform.forward, out RaycastHit hit, maxDistance, detectionLayers, QueryTriggerInteraction.Ignore))
        {
            targetLength = Mathf.Max(0.1f, hit.distance);
        }
        else
        {
            targetLength = maxDistance;
        }

        currentLength = Mathf.Lerp(currentLength, targetLength, Time.deltaTime * smoothingSpeed);

        if (attachedLight != null)
        {
            attachedLight.range = currentLength + 2f;
        }

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
        beamMesh.RecalculateNormals();
    }
}