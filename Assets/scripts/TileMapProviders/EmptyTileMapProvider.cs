namespace VGDC_RPG.TileMapProviders
{
    public class EmptyTileMapProvider : TileMapProvider
    {
        int w, h;
        ushort tid;

        public EmptyTileMapProvider(int width, int height, ushort tileID)
        {
            w = width;
            h = height;
            tid = tileID;
        }

        public ushort[,] GetTileMap()
        {
            ushort[,] m = new ushort[w, h];
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    m[i, j] = tid;
            return m;
        }
    }
}
