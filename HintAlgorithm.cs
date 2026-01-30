using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HintAlgorithm
{

    public const int W = 10;
    public const int H = 10;

    // Known input:
    // 0 = Unknown, 1 = Miss, 2 = Ship(Hit)
    //
    // Hint output:
    // 0 = Unknown(light blue)
    // 1 = Ship(red)
    // 2 = Miss(blue)
    // 3 = No(white)
    // 4 = Maybe(green)
    public static int[,] BuildHintMap(int[,] known)
    {
        if (known == null) throw new ArgumentNullException(nameof(known));
        if (known.GetLength(0) != W || known.GetLength(1) != H)
            throw new ArgumentException("known must be 10x10");

        var map = new int[W, H];

        var hits = new List<Vector2Int>(32);
        var hitSet = new HashSet<Vector2Int>();

        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
            {
                int k = known[x, y];

                if (k == 1) map[x, y] = 2;      // Miss -> hint Miss
                else if (k == 2)
                {
                    map[x, y] = 1;              // Ship/Hit -> hint Ship
                    var p = new Vector2Int(x, y);
                    hits.Add(p);
                    hitSet.Add(p);
                }
                else map[x, y] = 0;             // Unknown
            }

        if (hits.Count == 0) return map;

        // 1. Diagonal sequence => No (white)
        foreach (var h in hits)
        {
            Mark(map, h.x - 1, h.y - 1, 3);
            Mark(map, h.x + 1, h.y - 1, 3);
            Mark(map, h.x - 1, h.y + 1, 3);
            Mark(map, h.x + 1, h.y + 1, 3);
        }

        // 2. Horizontal sequence
        for (int y = 0; y < H; y++)
        {
            int x = 0;
            while (x < W)
            {
                if (!hitSet.Contains(new Vector2Int(x, y))) { x++; continue; }

                int start = x;
                int end = x;
                while (end + 1 < W && hitSet.Contains(new Vector2Int(end + 1, y))) end++;

                if (end - start + 1 >= 2)
                {
                    
                    for (int xx = start; xx <= end; xx++)
                    {
                        Mark(map, xx, y - 1, 3);
                        Mark(map, xx, y + 1, 3);
                    }

                   
                    Mark(map, start - 1, y, 4);
                    Mark(map, end + 1, y, 4);
                }

                x = end + 1;
            }
        }

        // 3. vertical sequence
        for (int x = 0; x < W; x++)
        {
            int y = 0;
            while (y < H)
            {
                if (!hitSet.Contains(new Vector2Int(x, y))) { y++; continue; }

                int start = y;
                int end = y;
                while (end + 1 < H && hitSet.Contains(new Vector2Int(x, end + 1))) end++;

                if (end - start + 1 >= 2)
                {
                    
                    for (int yy = start; yy <= end; yy++)
                    {
                        Mark(map, x - 1, yy, 3);
                        Mark(map, x + 1, yy, 3);
                    }

                    
                    Mark(map, x, start - 1, 4);
                    Mark(map, x, end + 1, 4);
                }

                y = end + 1;
            }
        }

        // 4. one hit case
        foreach (var h in hits)
        {
            bool hasH = hitSet.Contains(new Vector2Int(h.x - 1, h.y)) || hitSet.Contains(new Vector2Int(h.x + 1, h.y));
            bool hasV = hitSet.Contains(new Vector2Int(h.x, h.y - 1)) || hitSet.Contains(new Vector2Int(h.x, h.y + 1));

            if (!hasH && !hasV)
            {
                Mark(map, h.x - 1, h.y, 4);
                Mark(map, h.x + 1, h.y, 4);
                Mark(map, h.x, h.y - 1, 4);
                Mark(map, h.x, h.y + 1, 4);
            }

            if (hasH && !hasV)
            {
                Mark(map, h.x, h.y - 1, 3);
                Mark(map, h.x, h.y + 1, 3);
            }

            if (hasV && !hasH)
            {
                Mark(map, h.x - 1, h.y, 3);
                Mark(map, h.x + 1, h.y, 3);
            }
        }

        return map;
    }

    // value: 3=No, 4=Maybe
    private static void Mark(int[,] map, int x, int y, int value)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return;

        int cur = map[x, y];

        // tiles that was already clicked on : Ship(1) / Miss(2)
        if (cur == 1 || cur == 2) return;

        // No stronger then maybe
        if (cur == 4 && value == 3)
        {
            map[x, y] = 3;
            return;
        }

        if (cur == 3) return;

        map[x, y] = value;

    }
}
