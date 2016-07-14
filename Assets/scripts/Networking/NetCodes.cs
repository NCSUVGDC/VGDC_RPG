namespace VGDC_RPG.Networking
{
    //public enum NetCodes : byte
    //{
    //    ERROR = 0,
    //    ConnectInfo,
    //    Chat,
    //    DownloadTileMap,
    //    SetTeams,
    //    AddPlayer,
    //    StartMatch,

    //    NextTurn,
    //    DamagePlayer,
    //    MovePlayer,
    //    AddProjectile,
    //}

    public enum NetCodes : byte
    {
        ERROR = 0,
        ConnectInfo,
        Chat,
        DownloadTileMap,
        Clone,
        Event
    }
}
