using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Map
{
    public class TilePath
    {
        public List<Tiles.Tile> listOfTiles = new List<Tiles.Tile>();

        public int costOfPath = 0;

        public Tiles.Tile lastTile;

        public TileMap Map;

        public TilePath(TileMap map)
        {
            Map = map;
        }

        public TilePath(TileMap map, TilePath tp) : this(map)
        {
            listOfTiles = tp.listOfTiles.ToList();
            costOfPath = tp.costOfPath;
            lastTile = tp.lastTile;
        }

        public void addTile(Tiles.Tile t)
        {
            costOfPath += t.MovementCost;
            listOfTiles.Add(t);
            lastTile = t;
        }
    }
}
