using UnityEngine;

public class ScatterGeneratorV1 : MonoBehaviour
{
    public GameObject prefab;
    public int seed = 1;
    public Vector2 size = new Vector2(30f, 30f);
    [Min(0.5f)] public float cellSize = 3f;

    const string RootName = "Generated";

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();
        if (prefab == null) { Debug.LogWarning("Assignez un prefab.", this); return; }

        var pts = PointOps.Grid(size, cellSize, seed, Vector3.zero);

        var root = new GameObject(RootName).transform;
        root.SetParent(transform, false);
        foreach (var p in pts)
        {
            var go = Instantiate(prefab, root);
            go.transform.localPosition = p.position;
        }
        Debug.Log("[" + name + "] " + pts.Count + " objets générés.");
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        var t = transform.Find(RootName);
        if (t == null) return;
        if (Application.isPlaying) Destroy(t.gameObject);
        else DestroyImmediate(t.gameObject);
    }
}
