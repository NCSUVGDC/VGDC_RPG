using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VGDC_RPG.Map
{
    public class PathFinder
    {
        public static List<Tiles.Tile> FindPath(TileMap map, Tiles.Tile originTile, Tiles.Tile destinationTile)
        {
            //I have no clue what open and closed is supposed to mean
            //and it makes the rest of this code hard to decipher.
            List<Tiles.Tile> closed = new List<Tiles.Tile>();
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

                var neighbors = map.GetNeighbors(current.lastTile.X, current.lastTile.Y);
                foreach (Tiles.Tile t in neighbors)
                {
                    if (!t.Walkable) continue;
                    TilePath newTilePath = new TilePath(map, current);
                    newTilePath.addTile(t);
                    open.Add(newTilePath);
                }
            }
            return null;
        }

        public static List<Tiles.Tile> FindHighlight(TileMap map, Tiles.Tile originTile, int movementPoints)
        {
            List<Tiles.Tile> closed = new List<Tiles.Tile>();
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

                var neighbors = map.GetNeighbors(current.lastTile.X, current.lastTile.Y);
                foreach (Tiles.Tile t in neighbors)
                {
                    if (!t.Walkable) continue;
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