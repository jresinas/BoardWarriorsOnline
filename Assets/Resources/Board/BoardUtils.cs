using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardUtils {
    /// <summary>
    /// Returns true if an object can reach p2 from p1. Can move through allies but not enemies.
    /// </summary>
    /// <param name="range">Max number of steps availables to move</param>
    public static bool Reach(Vector2Int p1, Vector2Int p2, int range) {
        if (p1 == p2) return true;
        if (Vector2Int.Distance(p1, p2) > range) return false;
        else {
            CharacterController character = null;
            int characterId = CharacterManager.instance.GetId(p1);
            if (characterId >= 0) character = CharacterManager.instance.Get(characterId);
            if (character != null) return GetPath(p1, p2, character.GetPlayer()).Count > 0 && GetPath(p1, p2, character.GetPlayer()).Count <= range;
            else return false;
        }
    }

    /// <summary>
    /// Returns optimal list of steps to go from origin to destiny avoiding enemy positions
    /// </summary>
    /// <param name="player">Player id to identify enemies</param>
    /// <returns></returns>
    public static List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destiny, int player) {
        List<Vector2Int> path = AStar.GetPath(origin, destiny, player);
        if (path != null) {
            path.Remove(origin);
            return path;
        } else {
            return new List<Vector2Int>();
        }
    }

    public static Vector2Int GetShoveDestiny(Vector2Int shovedTile, Vector2Int shoverTile) {
        return shovedTile - (shoverTile - shovedTile);
    }
}

/// <summary>
/// Algorithm A*
/// </summary>
public static class AStar {
    class NodeData {
        // Distance to start node
        public int g;
        // Distance to end node (heuristic)
        public int h;
        // Node value (less better)
        public int f;
        // List of nodes from start to current node
        public List<Vector2Int> path;

        public NodeData(Vector2Int position, Vector2Int end, NodeData parentData = null) {
            g = (parentData != null) ? parentData.g + 1 : 0;
            h = Mathf.RoundToInt(Vector2Int.Distance(position, end));
            f = g + h;
            if (parentData != null) path = new List<Vector2Int>(parentData.path);
            else path = new List<Vector2Int>();
            path.Add(position);
        }
    }

    public static List<Vector2Int>? GetPath(Vector2Int start, Vector2Int end, int player) {
        Dictionary<Vector2Int, NodeData> open = new Dictionary<Vector2Int, NodeData>();
        open.Add(start, new NodeData(start, end));

        Dictionary<Vector2Int, NodeData> closed = new Dictionary<Vector2Int, NodeData>();
        Vector2Int current;
        NodeData currentData;

        while (open.Count > 0) {
            current = GetLowerCostNode(open) ?? default(Vector2Int);
            currentData = open[current];
            if (current == end) return currentData.path;
            open.Remove(current);
            closed.Add(current, currentData);
            Dictionary<Vector2Int, NodeData> successors = GetSuccessor(current, end, currentData, player);
            foreach (Vector2Int successor in successors.Keys) {
                NodeData successorData = successors[successor];
                if (closed.ContainsKey(successor)) continue;
                if (!open.ContainsKey(successor)) {
                    open.Add(successor, successorData);
                } else if (successorData.g < open[successor].g) {
                    open[successor] = successorData;
                }
            }
        }
        return null;
    }

    static Vector2Int? GetLowerCostNode(Dictionary<Vector2Int, NodeData> nodes) {
        Vector2Int? node = null;
        int minValue = int.MaxValue;
        if (nodes.Count > 0) {
            foreach (Vector2Int position in nodes.Keys) {
                if (nodes[position].f < minValue) {
                    node = position;
                    minValue = nodes[position].f;
                }
            }
        } else return null;
        return node;

        //open = open.OrderBy(x => x.Value.g).ToDictionary(x => x.Key, x => x.Value);
        //current = open.Keys.First();
    }

    static Dictionary<Vector2Int, NodeData> GetSuccessor(Vector2Int current, Vector2Int end, NodeData currentData, int player) {
        Dictionary<Vector2Int, NodeData> successors = new Dictionary<Vector2Int, NodeData>();
        List<Vector2Int> movements = new List<Vector2Int> { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        foreach (Vector2Int move in movements) {
            Vector2Int newPosition = current + move;
            if (newPosition.x >= 0 && newPosition.x < Const.BOARD_COLS && newPosition.y >= 0 && newPosition.y < Const.BOARD_ROWS) {
                CharacterController character = null;
                int characterId = CharacterManager.instance.GetId(newPosition);
                if (characterId >= 0) character = CharacterManager.instance.Get(characterId);
                if (character == null || character.GetPlayer() == player || newPosition == end) successors.Add(newPosition, new NodeData(newPosition, end, currentData));
            }
        }
        return successors;
    }
}