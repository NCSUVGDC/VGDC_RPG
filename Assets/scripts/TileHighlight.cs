/// © 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?


using System.Collections.Generic;

public class TileHighlight {
	
	public TileHighlight () {
		
	}
	
	public static List<Tile> FindHighlight(Tile originTile, int movementPoints) 
    {
		List<Tile> closed = new List<Tile>();
		List<TilePath> open = new List<TilePath>();
		
		TilePath originPath = new TilePath();
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
			
			foreach (Tile t in current.lastTile.neighbors) 
            {	
				if (t.impassible) continue;
				TilePath newTilePath = new TilePath(current);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
		}
		closed.Remove(originTile);
		return closed;
	}
}
