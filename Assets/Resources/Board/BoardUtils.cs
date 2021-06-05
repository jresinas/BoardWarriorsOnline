using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardUtils {
    public static int Distance(Vector2Int p1, Vector2Int p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    public static Vector2Int[] GetPath(Vector2Int origin, Vector2Int destiny) {
        Vector2Int[] path = new Vector2Int[Distance(origin, destiny)];
        Vector2Int position = origin;
        int i = 0;
        while (position != destiny) {
            if (position.y < destiny.y) position.y++;
            else if (position.y > destiny.y) position.y--;
            else if (position.x < destiny.x) position.x++;
            else if (position.x > destiny.x) position.x--;
            path[i] = position;
            i++;
        }

        return path;
    }
}
