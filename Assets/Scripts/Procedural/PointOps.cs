using System.Collections.Generic;
using UnityEngine;

public static class PointOps
{
    // Mélange la seed globale et deux entiers (ici, les indices de cellule)
    public static int CombineSeed(int seed, int a, int b)
    {
        unchecked { return (seed * 73856093) ^ (a * 19349663) ^ (b * 83492791); }
    }

    // 1. GÉNÉRER : grille régulière centrée sur 'center'
    public static List<SpawnPoint> Grid(Vector2 size, float cellSize, int seed, Vector3 center)
    {
        var pts = new List<SpawnPoint>();
        int nx = Mathf.FloorToInt(size.x / cellSize);
        int nz = Mathf.FloorToInt(size.y / cellSize);
        var origin = center + new Vector3(-size.x * 0.5f, 0f, -size.y * 0.5f);
        for (int x = 0; x < nx; x++)
            for (int z = 0; z < nz; z++)
            {
                var pos = origin + new Vector3((x + 0.5f) * cellSize, 0f, (z + 0.5f) * cellSize);
                pts.Add(new SpawnPoint(pos, CombineSeed(seed, x, z)));
            }
        return pts;
    }
}