using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VGDC_RPG.Map
{
    public class PathFinder
    {
        public static List<Int2> FindPath(TileMapScript map, Int2 originTile, Int2 destinationTile)
        {
            //I have no clue what open and closed is supposed to mean
            //and it makes the rest of this code hard to decipher.
            List<Int2> closed = new List<Int2>();
            List<TilePath> open = new List<TilePath>();

            TilePath originPath = new TilePath(map);
            originPath.addTile(originTile);

            open.Add(originPath);

            while (open.Count > 0)
            {
                //open = open.OrderBy(x => x.costOfPath).ToList();
                TilePath current = open[0];
                open.Remove(open[0]);

                if (closed.Contains(current.lastTile))
                {
                    continue;
                }
                if (current.lastTile == destinationTile)
                {
                    current.listOfTiles.Remove(originTile);
                    return current.listOfTiles;
                }

                closed.Add(current.lastTile);

                var neighbors = map.GetNeighbors(current.lastTile);
                foreach (Int2 t in neighbors)
                {
                    if (!map[t].Walkable) continue;
                    TilePath newTilePath = new TilePath(map, current);
                    newTilePath.addTile(t);
                    open.Add(newTilePath);
                }
            }
            return null;
        }

        public static List<Int2> FindPathBeside(TileMapScript map, Int2 originTile, Int2 destinationTile)
        {
            //I have no clue what open and closed is supposed to mean
            //and it makes the rest of this code hard to decipher.
            List<Int2> closed = new List<Int2>();
            List<TilePath> open = new List<TilePath>();

            TilePath originPath = new TilePath(map);
            originPath.addTile(originTile);

            open.Add(originPath);

            while (open.Count > 0)
            {
                //open = open.OrderBy(x => x.costOfPath).ToList();
                TilePath current = open[0];
                open.Remove(open[0]);

                if (closed.Contains(current.lastTile))
                {
                    continue;
                }
                if (current.lastTile == new Int2(destinationTile.X - 1, destinationTile.Y) || current.lastTile == new Int2(destinationTile.X + 1, destinationTile.Y) || current.lastTile == new Int2(destinationTile.X, destinationTile.Y - 1) || current.lastTile == new Int2(destinationTile.X, destinationTile.Y + 1))
                {
                    current.listOfTiles.Remove(originTile);
                    return current.listOfTiles;
                }

                closed.Add(current.lastTile);

                var neighbors = map.GetNeighbors(current.lastTile);
                foreach (Int2 t in neighbors)
                {
                    if (!map[t].Walkable) continue;
                    TilePath newTilePath = new TilePath(map, current);
                    newTilePath.addTile(t);
                    open.Add(newTilePath);
                }
            }
            return null;
        }

        public static List<Int2> FindHighlight(TileMapScript map, Int2 originTile, int movementPoints)
        {
            List<Int2> closed = new List<Int2>();
            List<TilePath> open = new List<TilePath>();

            TilePath originPath = new TilePath(map);
            originPath.addTile(originTile);

            open.Add(originPath);

            while (open.Count > 0)
            {
                TilePath current = open[0];
                open.Remove(open[0]);

                if (closed.Contains(current.lastTile))
                {
                    continue;
                }
                if (current.costOfPath > movementPoints + 1)
                {
                    continue;
                }

                closed.Add(current.lastTile);

                var neighbors = map.GetNeighbors(current.lastTile);
                foreach (Int2 t in neighbors)
                {
                    if (!map[t].Walkable) continue;
                    TilePath newTilePath = new TilePath(map, current);
                    newTilePath.addTile(t);
                    open.Add(newTilePath);
                }
            }
            closed.Remove(originTile);
            return closed;
        }
    }
}