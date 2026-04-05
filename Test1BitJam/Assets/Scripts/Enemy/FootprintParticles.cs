using UnityEngine;

public class FootprintSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject whiteFootprintPrefabA;
    public Vector3 offsetA;
    public GameObject whiteFootprintPrefabB;
    public Vector3 offsetB;
    public Transform spawnParent;
    public GameObject blueFootprintPrefabA;
    public GameObject blueFootprintPrefabB;

    [Header("Spawn Timing")]
    public float spawnInterval = 0.5f;
    public float firstSpawnDelay = 0f;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    private float timer = 0f;
    private bool firstSpawned = false;
    private bool spawnAWhite = true;
    private bool spawnABlue = true;

    void Update()
    {
        if (!whiteFootprintPrefabA || !whiteFootprintPrefabB) return;

        timer += Time.deltaTime;

        if (!firstSpawned)
        {
            if (timer >= firstSpawnDelay)
            {
                SpawnWhiteFootprint();
                SpawnBlueFootprint();
                timer = 0f;
                firstSpawned = true;
            }
        }
        else
        {
            if (timer >= spawnInterval)
            {
                SpawnWhiteFootprint();
                SpawnBlueFootprint();
                timer = 0f;
            }
        }
    }

    void SpawnWhiteFootprint()
    {
        GameObject prefabToSpawn;
        Vector3 spawnOffset;

        if (spawnAWhite)
        {
            prefabToSpawn = whiteFootprintPrefabA;
            spawnOffset = offsetA;
        }
        else
        {
            prefabToSpawn = whiteFootprintPrefabB;
            spawnOffset = offsetB;
        }

        spawnAWhite = !spawnAWhite; 

        
        Vector3 spawnPos = transform.TransformPoint(spawnOffset);
        
        GameObject footprint = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        footprint.transform.rotation = Quaternion.Euler(90f, spawnParent.eulerAngles.y, 0f);

        var fade = footprint.AddComponent<FadeOverTime>();
        fade.fadeDuration = fadeDuration;

    }
    
    void SpawnBlueFootprint()
    {
        GameObject prefabToSpawn;
        Vector3 spawnOffset;

        if (spawnABlue)
        {
            prefabToSpawn = blueFootprintPrefabA;
            spawnOffset = offsetA;
        }
        else
        {
            prefabToSpawn = blueFootprintPrefabB;
            spawnOffset = offsetB;
        }

        spawnABlue = !spawnABlue; 

        
        Vector3 spawnPos = transform.TransformPoint(spawnOffset);
        GameObject footprint = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        footprint.transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);

        var fade = footprint.AddComponent<FadeOverTime>();
        fade.fadeDuration = fadeDuration;
    }

    private class FadeOverTime : MonoBehaviour
    {
        public float fadeDuration = 2f;
        private Material mat;
        private Color initialColor;
        private float timer = 0f;

        void Awake()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (!renderer)
            {
                Debug.LogError("FadeOverTime requires a Renderer component.");
                enabled = false;
                return;
            }

            mat = renderer.material;
            initialColor = mat.color;
        }

        void Update()
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, timer / fadeDuration);

            Color c = initialColor;
            c.a = alpha;
            mat.color = c;

            if (alpha <= 0f)
                Destroy(gameObject);
        }
    }
}
