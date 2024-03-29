﻿using System;
using System.Collections.Generic;

namespace VGDC_RPG.Map.Pathfinding
{
    public static class AStarSearch
    {

        /// <summary>
        /// Manhatten heuristic (dx + dy).
        /// </summary>
        /// <param name="a">First location.</param>
        /// <param name="b">Second location.</param>
        /// <returns></returns>
        public static int Heuristic(Int2 a, Int2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Int2> DoSearch(TileMap map, Int2 start, Int2 goal, bool b)
        {
            Dictionary<Int2, Int2> origin = new Dictionary<Int2, Int2>();
            Dictionary<Int2, int> costs = new Dictionary<Int2, int>();

            var frontier = new PriorityQueue<Int2Float>();
            frontier.Enqueue(new Int2Float(start, 0));

            origin[start] = start;
            costs[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (!b && current.Value == goal)
                    break;
                else if (b && (current.Value == new Int2(goal.X - 1, goal.Y)
                    || current.Value == new Int2(goal.X + 1, goal.Y)
                    || current.Value == new Int2(goal.X, goal.Y - 1)
                    || current.Value == new Int2(goal.X, goal.Y + 1)))
                {
                    goal = current.Value;
                    break;
                }

                foreach (var next in map.GetNeighbors(current.Value))
                {
                    if (!map.IsWalkable(next.X, next.Y))
                        continue;
                    int newCost = costs[current.Value] + map.GetMovementCost(current.Value.X, current.Value.Y);
                    if (!costs.ContainsKey(next) || newCost < costs[next])
                    {
                        costs[next] = newCost;
                        int priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(new Int2Float(next, priority));
                        origin[next] = current.Value;
                    }
                }
            }

            if (!origin.ContainsKey(goal))
                return null;

            List<Int2> r = new List<Int2>();
            Int2 lv = goal;
            r.Add(lv);
            while (lv != start)
            {
                lv = origin[lv];
                r.Insert(0, lv);
            }
            return r;
        }
        
        /// <summary>
        /// Finds a path to a location.
        /// </summary>
        /// <param name="map">The TileMap to use.</param>
        /// <param name="start">The start location.</param>
        /// <param name="goal">The goal location.</param>
        /// <returns></returns>
        public static List<Int2> FindPath(TileMap map, Int2 start, Int2 goal)
        {
            return DoSearch(map, start, goal, false);
        }

        /// <summary>
        /// Finds a path beside a location.
        /// </summary>
        /// <param name="map">The TileMap to use.</param>
        /// <param name="start">The start location.</param>
        /// <param name="goal">The goal location.</param>
        /// <returns></returns>
        public static List<Int2> FindPathBeside(TileMap map, Int2 start, Int2 goal)
        {
            return DoSearch(map, start, goal, true);
        }

        /// <summary>
        /// Not any fast than the original yet, can't really benifit from A*.
        /// </summary>
        /// <param name="map">The TileMap to use.</param>
        /// <param name="start">The start location.</param>
        /// <param name="maxCost">The max movement cost that can be reached.</param>
        /// <returns></returns>
        public static List<Int2> FindHighlight(TileMap map, Int2 start, int maxCost)
        {
            List<Int2> r = new List<Int2>();
            var frontier = new PriorityQueue<Int2Float>();
            frontier.Enqueue(new Int2Float(start, 0));
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Distance <= maxCost)
                {
                    if (current.Value != start)
                        r.Add(current.Value);
                }
                else
                    continue;

                foreach (var next in map.GetNeighbors(current.Value))
                {
                    if (!map.IsWalkable(next.X, next.Y))
                        continue;
                    frontier.Enqueue(new Int2Float(next, current.Distance + map.GetMovementCost(current.Value.X, current.Value.Y)));
                }
            }

            return r;
        }
    }
}
