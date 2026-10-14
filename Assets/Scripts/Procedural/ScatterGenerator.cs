using System.Collections.Generic;
using UnityEngine;

public class ScatterGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public GameObject prefab;
        [Min(0f)] public float weight = 1f;
    }

    [Header("Aléa")]
    public int seed = 1;

    [Header("Zone et grille")]
    public Vector2 size = new Vector2(30f, 30f);
    [Min(0.5f)] public float cellSize = 3f;

    [Header("Règles")]
    [Range(0f, 1f)] public float minDensity = 0.45f;
    public float noiseFrequency = 0.08f;
    public float jitter = 1f;
    public Vector2 scaleRange = new Vector2(0.5f, 1.2f);
    public float pruneRadius = 0.6f;

    [Header("Contenu")]
    public List<Entry> entries = new List<Entry>();

    [Header("Debug")]
    public bool drawGizmos = true;

    const string RootName = "Generated";
    List<SpawnPoint> lastPoints = new List<SpawnPoint>();

    // GÉNÉRER -> FILTRER -> MODIFIER
    public List<SpawnPoint> BuildPoints()
    {
        // décalage du bruit : dérivé de la seed, donc reproductible
        var offset = new Vector2(PointOps.Range(seed, 100, 0f, 500f), PointOps.Range(seed, 101, 0f, 500f));

        var pts = PointOps.Grid(size, cellSize, seed, Vector3.zero);
        Debug.Log("Grille : " + pts.Count);
        PointOps.NoiseDensity(pts, noiseFrequency, offset);
        pts = PointOps.FilterDensity(pts, minDensity);
        Debug.Log("Après filtre de densité : " + pts.Count);
        PointOps.Randomize(pts, jitter, scaleRange.x, scaleRange.y);
        pts = PointOps.SelfPrune(pts, pruneRadius);
        Debug.Log("Après anti-chevauchement : " + pts.Count);
        return pts;
    }

    // INSTANCIER
    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();
        if (entries.Count == 0 || entries[0].prefab == null)
        {
            Debug.LogWarning("Ajoutez au moins une entrée avec un prefab.", this);
            return;
        }

        lastPoints = BuildPoints();

        var root = new GameObject(RootName).transform;
        root.SetParent(transform, false);

        var weights = new List<float>();
        foreach (var e in entries) weights.Add(e.weight);

        foreach (var p in lastPoints)
        {
            int index = PointOps.PickWeighted(weights, PointOps.Hash01(p.seed, 4));
            var prefab = entries[index].prefab;
            if (prefab == null) continue;

            var go = Instantiate(prefab, root);
            go.transform.localPosition = p.position;
            go.transform.localRotation = p.rotation;
            go.transform.localScale = p.scale;
        }
        Debug.Log($"[{name}] seed {seed} : {lastPoints.Count} objets générés.");
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        var t = transform.Find(RootName);
        if (t == null) return;
        if (Application.isPlaying) Destroy(t.gameObject);
        else DestroyImmediate(t.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        foreach (var p in BuildPoints())
        {
            Gizmos.color = Color.Lerp(Color.red, Color.green, p.density);
            Gizmos.DrawWireSphere(transform.TransformPoint(p.position), 0.25f * p.scale.x);
        }
    }
}