using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardUtils {
    public static int Distance(Vector2Int p1, Vector2Int p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    public static bool Reach(Vector2Int p1, Vector2Int p2, int range) {
        return Distance(p1, p2) <= range;
        /*
        if (Distance(p1, p2) > range) return false;
        else {
            CharacterController character = null;
            int characterId = CharacterManager.instance.GetId(p1);
            if (characterId >= 0) character = CharacterManager.instance.Get(characterId);
            if (character != null) return GetPath(p1, p2, character.GetPlayer()).Count > 0 && GetPath(p1, p2, character.GetPlayer()).Count <= range + 1;
            else return false;
        }
        */
    }

    public static List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destiny, int player) {
        return new List<Vector2Int>(new Vector2Int[] { origin, destiny});
        //return AStar.GetPath(origin, destiny, player) ?? new List<Vector2Int>();
    }
}

public static class AStar {
    class NodeData {
        public int g;
        public int h;
        public int f;
        public List<Vector2Int> path;

        public NodeData(Vector2Int position, Vector2Int end, NodeData parentData = null) {
            g = (parentData != null) ? parentData.g + 1 : 0;
            h = BoardUtils.Distance(position, end);
            f = g + h;
            if (parentData != null) path = new List<Vector2Int>(parentData.path);
            else path = new List<Vector2Int>();
            path.Add(position);
        }
    }

    public static List<Vector2Int>? GetPath(Vector2Int start, Vector2Int end, int player) {
        Debug.Log("Start Pathfind: " + Time.time);
        Dictionary<Vector2Int, NodeData> open = new Dictionary<Vector2Int, NodeData>();
        open.Add(start, new NodeData(start, end));

        Dictionary<Vector2Int, NodeData> closed = new Dictionary<Vector2Int, NodeData>();
        Vector2Int current;
        NodeData currentData;

        while (open.Count > 0) {
            current = GetLowerCostNode(open) ?? default(Vector2Int);
            currentData = open[current];
            if (current == end) {
                Debug.Log("End Pathfind: "+Time.time);
                return currentData.path;
            }
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
        Debug.Log("End Pathfind: " + Time.time);
        return null;
    }

    static Vector2Int? GetLowerCostNode(Dictionary<Vector2Int, NodeData> nodes) {
        Vector2Int? node = null;
        int minValue = 99999;
        if (nodes.Count > 0) {
            foreach (Vector2Int position in nodes.Keys) {
                if (nodes[position].f < minValue) node = position;
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
            if (newPosition.x >= 0 && newPosition.x < 5 && newPosition.y >= 0 && newPosition.y < 5) {
                CharacterController character = null;
                int characterId = CharacterManager.instance.GetId(newPosition);
                if (characterId >= 0) character = CharacterManager.instance.Get(characterId);
                if (character == null || character.GetPlayer() == player || newPosition == end) successors.Add(newPosition, new NodeData(newPosition, end, currentData));
            }
        }
        return successors;
    }
}