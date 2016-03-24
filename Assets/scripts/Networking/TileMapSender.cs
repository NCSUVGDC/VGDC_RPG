namespace VGDC_RPG.Networking
{
    public static class TileMapSender
    {
        public static void Send(ushort[][,] m, NetServer ns, NetConnection c)
        {
            byte[] buffer = new byte[1024];
            for (int layer = 0; layer < m.Length; layer++)
                for (int y = 0; y < m[0].GetLength(1); y++)
                {
                    var w = new DataWriter(buffer);
                    w.Write((byte)NetCodes.DownloadTileMap);
                    w.Write(y + layer * m[0].GetLength(1) + 1);
                    //Debug.Log("TSM: di: " + (y + layer * m[0].GetLength(1) + 1));
                    System.Threading.Thread.Sleep(5);   //TODO: Possible issue, some packets are lost is not given a little time between, regardless of QoS.  May cause issues later when packet delivery is esential.
                    for (int x = 0; x < m[0].GetLength(0); x++)
                        w.Write(m[layer][x, y]);
                    ns.SendReliable(w, c);
                }
        }
    }
}
