using System;
using System.Collections.Generic;
using UnityEngine;
using VGDC_RPG.Map;
using VGDC_RPG.Networking;
using VGDC_RPG.TileMapProviders;
using VGDC_RPG.Units;

namespace VGDC_RPG
{
    public static class GameLogic
    {
        public static class MatchInfo
        {
            public enum PlayerType : byte
            {
                None = 0,
                Local,
                Remote,
                AI
            }

            public struct PlayerInfo
            {
                public string PlayerName;
                public PlayerType PlayerType;
                public byte Team;
                public AIController AIController;

                public PlayerInfo(string name, PlayerType type, byte team, byte playerID)
                {
                    PlayerName = name;
                    PlayerType = type;
                    Team = team;
                    if (type == PlayerType.AI && IsHost)
                        AIController = new AIController(playerID);
                    else
                        AIController = null;
                }
            }

            public static PlayerInfo[] PlayerInfos = new PlayerInfo[8];
        }

        private class EventHandler : INetEventHandler
        {
            public int HandlerID { get { return -2; } }

            public EventHandler()
            {
                NetEvents.RegisterHandler(this);
            }

            public void HandleEvent(int cid, DataReader r)
            {
                if (!IsHost)
                {
                    var et = (EventType)r.ReadByte();

                    switch (et)
                    {
                        case EventType.StartMatch:
                            MyPlayerID = r.ReadByte();
                            StartMatch();
                            break;
                        case EventType.SetPlayer:
                            SetPlayer(r.ReadByte());
                            break;
                        case EventType.SetActionState:
                            SetState((ActionState)r.ReadByte());
                            break;
                        case EventType.SetUnit:
                            SetUnit(r.ReadByte());
                            break;
                        case EventType.SetSunColor:
                            SetSunColor(new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));
                            break;
                        case EventType.SetAmbientColor:
                            SetAmbientColor(new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));
                            break;
                        case EventType.SetBrightness:
                            SetBrightness(r.ReadSingle());
                            break;
                        case EventType.InitUI:
                            InitUI();
                            break;
                        default:
                            throw new Exception("Invalid event type: " + et.ToString());
                    }
                }
                else
                {
                    var et = (EventType)r.ReadByte();

                    switch (et)
                    {
                        case EventType.ClickTile:
                            ClickTile(CIDPlayers[cid], new Int2(r.ReadInt32(), r.ReadInt32()));
                            break;
                        case EventType.EndTurn:
                            //Debug.Log("Host Received End Turn");
                            //Debug.Log(CIDPlayers[cid] + "/" + CurrentPlayer);
                            //if (CIDPlayers[cid] == CurrentPlayer)
                            //    NextPlayer();
                            EndTurn();
                            break;
                        case EventType.ReqSetActionState:
                            if (CIDPlayers[cid] == CurrentPlayer)
                                SetState((ActionState)r.ReadByte());
                            break;
                        case EventType.ReqSetUnit:
                            if (CIDPlayers[cid] == CurrentPlayer)
                                SetUnit(r.ReadByte());
                            break;
                        default:
                            throw new Exception("Invalid event type: " + et.ToString());
                    }
                }
            }
        }

        private enum EventType : byte
        {
            ERROR = 0,
            StartMatch,
            ClickTile,
            SetPlayer,
            EndTurn,
            ReqSetActionState,
            SetActionState,
            ReqSetUnit,
            SetUnit,
            SetSunColor,
            SetAmbientColor,
            SetBrightness,
            InitUI
        }

        public enum ActionState : byte
        {
            ERROR = 0,
            None,
            Move,
            Attack,
            Inventory,
            Potion
        }

        public static TileMap Map;
        public static GameObject Camera;
        private static CameraController camScript;
        public static CameraController CameraScript
        {
            get
            {
                if (camScript == null)
                    camScript = Camera.GetComponent<CameraController>();
                return camScript;
            }
        }

        public static ushort[][,] mapConstructionData;

        public static bool IsHost;

        public static List<Unit>[] Units;

        private static byte[] netBuffer = new byte[512];

        public static int TeamCount;
        public static byte CurrentPlayer;
        public static byte CurrentUnitID;

        private static EventHandler eh;

        public static ActionState State = ActionState.None;

        public static Dictionary<int, byte> CIDPlayers;
        public static int[] PlayersCID;

        public static byte MyPlayerID;

        public static bool IsMyTurn
        {
            get
            {
                return CurrentPlayer == MyPlayerID;
            }
        }

        public static TileMapProvider TMP;

        public static Queue<byte> UnitQueue;
        public static Queue<byte> PlayerQueue;

        public static void Init()
        {
            Units = new List<Unit>[MatchInfo.PlayerInfos.Length];
            CIDPlayers = new Dictionary<int, byte>();
            PlayersCID = new int[MatchInfo.PlayerInfos.Length];
            UnitQueue = new Queue<byte>();
            PlayerQueue = new Queue<byte>();

            for (byte i = 0; i < MatchInfo.PlayerInfos.Length; i++)
            {
                MatchInfo.PlayerInfos[i] = new MatchInfo.PlayerInfo("Empty", MatchInfo.PlayerType.None, 0, i);
                Units[i] = new List<Unit>();
                PlayersCID[i] = -3;
            }

            eh = new EventHandler();
        }

        public static Int2 GetScreenTile(float x, float y)
        {
            if (Camera == null)
                Camera = GameObject.Find("CameraObject/Main Camera");

            x -= Camera.GetComponent<Camera>().pixelWidth / 2.0f;
            y -= Camera.GetComponent<Camera>().pixelHeight / 2.0f;
            x /= 64.0f * CameraController.Zoom;
            y /= 64.0f * CameraController.Zoom;
            x += Camera.transform.position.x;
            y += Camera.transform.position.z;

            return new Int2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public static void GenerateTestMap(int width, int height)
        {
            for (int i = 0; i < 20; i++)
            {
                var tc = Environment.TickCount;
                mapConstructionData = new TestTileMapProvider(width, height).GetTileMap();
                var map = TileMap.Construct(mapConstructionData);
                Debug.Log("TMCT: " + (Environment.TickCount - tc));
                if (map.LargestIsland * 4 >= (width * height))
                {
                    Map = map;
                    break;
                }
                else
                {
                    map.Destroy();
                }
            }
            if (Map == null)
                Debug.LogError("Failed to generate suitable map after 20 attempts.");
        }

        internal static void InitUI()
        {
            UpdateUnitUI();
            for (int i = 0; i < Units.Length; i++)
                foreach (var u in Units[i])
                    u.Sprite.SetHealth(u.Stats.HitPoints, u.Stats.MaxHitPoints);
            if (IsHost)
            {
                var w = new DataWriter(6);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.InitUI);
                MatchServer.Send(w);
            }
        }

        public static void SetMapProvider(TileMapProvider tmp)
        {
            SetMap(tmp.GetTileMap());
            TMP = tmp;
        }

        public static void SetMap(ushort[][,] data)
        {
            mapConstructionData = data;
            if (IsHost)
                TileMapSender.SendToAll(data);
        }

        public static void BuildMap()
        {
            Map = TileMap.Construct(mapConstructionData);
            SetSunColor(TMP.GetInitialSunColor());
            SetAmbientColor(TMP.GetInitialAmbientColor());
            SetBrightness(TMP.GetInitialBrightness());
        }

        public static void SetSunColor(Color color)
        {
            MergingScript.LAI.mat.SetColor("_SunColor", color);
            if (IsHost)
            {
                var w = new DataWriter(22);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetSunColor);
                w.Write(color.r);
                w.Write(color.g);
                w.Write(color.b);
                w.Write(color.a);
                MatchServer.Send(w);
            }
        }

        public static void SetAmbientColor(Color color)
        {
            MergingScript.LAI.mat.SetColor("_AmbientColor", color);
            if (IsHost)
            {
                var w = new DataWriter(22);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetAmbientColor);
                w.Write(color.r);
                w.Write(color.g);
                w.Write(color.b);
                w.Write(color.a);
                MatchServer.Send(w);
            }
        }

        public static void SetBrightness(float v)
        {
            MergingScript.LAI.mat.SetFloat("_Brightness", v);
            if (IsHost)
            {
                var w = new DataWriter(10);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetBrightness);
                w.Write(v);
                MatchServer.Send(w);
            }
        }

        public static Color GetSunColor()
        {
            return MergingScript.LAI.mat.GetColor("_SunColor");
        }

        public static Color GetAmbientColor()
        {
            return MergingScript.LAI.mat.GetColor("_AmbientColor");
        }

        public static float GetBrightness()
        {
            return MergingScript.LAI.mat.GetFloat("_Brightness");
        }

        public static void NextTeam()
        {
            do
            {
                CurrentPlayer++;
                if (CurrentPlayer > MatchInfo.PlayerInfos.Length)
                    CurrentPlayer = 0;
            }
            while (PlayerHasAliveUnits(CurrentPlayer));
        }

        public static void AddUnit(byte player, Unit unit)
        {
            Units[player].Add(unit);
            unit.PlayerID = player;
            unit.Sprite.PlayerID = unit.PlayerID;
            unit.Sprite.UnitID = (byte)(Units[player].Count - 1);
            if (IsHost)
            {
                //var w = new DataWriter(netBuffer);
                //unit.Inventory.Clone(w);
                //MatchServer.Send(w);
                var w = new DataWriter(netBuffer);
                unit.Clone(w);
                MatchServer.Send(w);
            }
        }

        public static bool PlayerHasAliveUnits(int player)
        {
            foreach (var u in Units[player])
                if (u.Stats.Alive)
                    return true;
            return false;
        }

        public static void StartMatch()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("scenes/matchScene");

            if (IsHost)
            {
                for (byte i = 0; i < PlayersCID.Length; i++)
                {
                    if (PlayersCID[i] >= 0)
                    {
                        var w = new DataWriter(7);
                        w.Write((byte)NetCodes.Event);
                        w.Write(eh.HandlerID);
                        w.Write((byte)EventType.StartMatch);
                        w.Write(i);
                        MatchServer.SendTo(w, MatchServer.GetConnection(PlayersCID[i]));
                    }
                }
            }
        }

        public static Unit SpawnUnit(string resName, int x, int y)
        {
            var s = Resources.Load<TextAsset>("units/" + resName).text.Split('\n');
            var u = new Unit();
            u.SetPosition(x, y);
            u.Stats.Alive = true;

            for (int i = 0; i < s.Length; i++)
            {
                var split = s[i].IndexOf(':');
                if (split == -1)
                    continue;
                var prop = s[i].Substring(0, split).Trim();
                var val = s[i].Substring(split + 1, s[i].Length - split - 1).Trim();
                switch (prop)
                {
                    case "Name":
                        u.Name = val;
                        break;
                    case "Sprite":
                        u.Sprite.SetSpriteSet(val);
                        break;
                    case "MaxHP":
                        u.Stats.MaxHitPoints = int.Parse(val);
                        break;
                    case "HP":
                        u.Stats.HitPoints = int.Parse(val);
                        break;
                    case "MvmtRng":
                        u.Stats.MovementRange = int.Parse(val);
                        break;
                    case "Initiative":
                        u.Stats.Initiative = int.Parse(val);
                        break;
                    case "Range":
                        u.Stats.Range = int.Parse(val);
                        break;
                    case "Weapon":
                        switch (val)
                        {
                            case "Bow":
                                {
                                    var w = new DataWriter(512);
                                    var bow = new Units.Items.BowWeapon();
                                    //bow.Clone(w);  TODO
                                    //MatchServer.Send(w);

                                    u.Inventory.AddItem(bow.HandlerID, false);
                                    u.Inventory.SelectWeapon(bow.HandlerID, false);
                                }
                                break;
                            case "Grenade":
                                {
                                    var w = new DataWriter(512);
                                    var bow = new Units.Items.GrenadeWeapon();
                                    //bow.Clone(w);  TODO
                                    //MatchServer.Send(w);

                                    u.Inventory.AddItem(bow.HandlerID, false);
                                    u.Inventory.SelectWeapon(bow.HandlerID, false);
                                }
                                break;
                            case "Healing_Staff":
                                {
                                    var w = new DataWriter(512);
                                    var bow = new Units.Items.HealdingStaff();

                                    u.Inventory.AddItem(bow.HandlerID, false);
                                    u.Inventory.SelectWeapon(bow.HandlerID, false);
                                }
                                break;
                        }
                        break;
                    default:
                        Debug.LogWarning("Invalid property while loading unit: " + resName + ":" + prop + " with value: " + val);
                        break;
                }
            }

            return u;
        }

        public static void SpawnUnits()
        {
            if (IsHost)
            {
                for (byte i = 0; i < PlayersCID.Length; i++)
                {
                    if (PlayersCID[i] == -2)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            //var u = new Unit();
                            //u.SetPosition(i * 2, j + 3);
                            //u.Name = "Host Unit";
                            //u.Sprite.SetSpriteSet("Grenadier");

                            //u.Stats.Alive = true;
                            //u.Stats.MaxHitPoints = 20;
                            //u.Stats.HitPoints = u.Stats.MaxHitPoints;
                            //u.Stats.MovementRange = 4;

                            //AddUnit(i, u);
                            int x;
                            int y;
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Grenadier", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Ranger", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Cleric", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Warrior", x, y));
                        }
                    }
                    else if (PlayersCID[i] >= 0 || PlayersCID[i] == -1)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            //var u = new Unit();
                            //u.SetPosition(i * 2, j + 3);
                            //u.Name = "Player " + i + " Unit";
                            //u.Sprite.SetSpriteSet("Ranger");

                            //u.Stats.Alive = true;
                            //u.Stats.MaxHitPoints = 20;
                            //u.Stats.HitPoints = u.Stats.MaxHitPoints;
                            //u.Stats.MovementRange = 4;

                            //AddUnit(i, u);
                            int x;
                            int y;
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Grenadier", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Ranger", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Cleric", x, y));
                            FindSpawn(out x, out y);
                            AddUnit(i, SpawnUnit("Warrior", x, y));
                        }
                    }
                    else
                        Debug.Log("PID: " + PlayersCID[i]);
                }
            }

            //CreateUnitQueue();
            EndTurn();
            //UpdateUnitUI();
        }

        private static void FindSpawn(out int x, out int y)
        {
            int attempts = 0;
            while (attempts++ < 1000)
            {
                x = UnityEngine.Random.Range(0, Map.Width);
                y = UnityEngine.Random.Range(0, Map.Height);

                if (Map.InSpawn(x, y))
                    return;
            }
            x = -1;
            y = -1;
        }

        private static void CreateUnitQueue()
        {
            Debug.Log("Creating Unit Queue");
            UnitQueue.Clear();
            PlayerQueue.Clear();
            for (int i = 0; i < 5; i++)
                for (byte p = 0; p < Units.Length; p++)
                    for (byte u = 0; u < Units[p].Count; u++)
                        if (Units[p][u].Stats.Initiative == i)
                        {
                            UnitQueue.Enqueue(u);
                            PlayerQueue.Enqueue(p);
                            Debug.Log("Player/Unit: " + p + "/" + u);
                        }

            for (int id = 0; id < Units.Length; id++)
                foreach (var u in Units[id])
                    u.TurnReset();
        }

        public static void ClickTile(Int2 t)
        {
            if (IsHost)
            {
                if (t.X >= 0 && t.Y >= 0 && t.X < Map.Width && t.Y < Map.Height)
                    ClickTile(0, t);
            }
            else
            {
                var w = new DataWriter(14);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.ClickTile);
                w.Write(t.X);
                w.Write(t.Y);
                MatchClient.Send(w);
            }
        }

        public static void ClickTile(byte player, Int2 tile)
        {
            Debug.Log("Player " + player + " clicked tile @: " + tile);
            if (tile.X >= 0 && tile.Y >= 0 && tile.X < Map.Width && tile.Y < Map.Height)
                if (CurrentPlayer == player)
                    if (State == ActionState.Move)
                    {
                        var u = Units[player][CurrentUnitID];
                        if (!u.HasMoved && u.PossibleMovementTiles.Contains(tile))
                            u.GoTo(tile.X, tile.Y);
                    }
                    else if (State == ActionState.Attack)
                    {
                        var u = Units[player][CurrentUnitID];
                        if (!u.HasAttacked && u.Inventory.SelectedWeapon.GetAttackTiles(u).Contains(tile))
                        {
                            if (u.Inventory.SelectedWeapon.Attack(u, tile))
                            {
                                u.HasAttacked = true;
                                u.HasMoved = true;
                            }
                        }
                    }
        }

        public static void SetPlayer(byte id)
        {
            CurrentPlayer = id;
            //foreach (var u in Units[id])
            //    u.TurnReset();
            //State = ActionState.None;
            //CurrentUnitID = 0;
            //while (!Units[CurrentPlayer][CurrentUnitID].Stats.Alive)
            //    CurrentUnitID++;

            //UpdateUnitUI();

            if (IsHost)
            {
                var w = new DataWriter(7);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetPlayer);
                w.Write(CurrentPlayer);
                MatchServer.Send(w);
            }

            if (MatchInfo.PlayerInfos[CurrentPlayer].AIController != null)
                MatchInfo.PlayerInfos[CurrentPlayer].AIController.StartTurn();
        }

        public static void UpdateUnitUI()
        {
            Map.ClearHighlight();
            if (State == ActionState.Move)
                Units[CurrentPlayer][CurrentUnitID].SelectMovement();
            else if (State == ActionState.Attack)
                Units[CurrentPlayer][CurrentUnitID].SelectAttack();
            else if (State == ActionState.Inventory)
                Units[CurrentPlayer][CurrentUnitID].SelectInventory(); // Let them look at the inventory
            else if (State == ActionState.Potion)
                Units[CurrentPlayer][CurrentUnitID].SelectPotion();
        }

        public static void ReqSetUnit(byte id)
        {
            if (IsHost)
                SetUnit(id);
            else
            {
                var w = new DataWriter(7);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.ReqSetUnit);
                w.Write(id);
                MatchClient.Send(w);
            }
        }

        public static void SetUnit(byte id)
        {
            if (id < 0 && id >= Units[CurrentPlayer].Count || !Units[CurrentPlayer][id].Stats.Alive)
                return;
            CurrentUnitID = id;
            State = ActionState.None;

            if (CameraScript != null) /// Issue when initially spawning since will not target first active unit
                CameraScript.TargetPosition = new Vector3(Units[CurrentPlayer][id].X + 0.5f, CameraScript.TargetPosition.y, Units[CurrentPlayer][id].Y + 0.5f);

            UpdateUnitUI();

            if (IsHost)
            {
                var w = new DataWriter(7);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetUnit);
                w.Write(CurrentUnitID);
                MatchServer.Send(w);
            }
        }

        public static void EndTurn()
        {
            /*if (IsMyTurn)
            {
                if (IsHost)
                    NextPlayer();
                else
                {
                    var w = new DataWriter(6);
                    w.Write((byte)NetCodes.Event);
                    w.Write(eh.HandlerID);
                    w.Write((byte)EventType.EndTurn);
                    MatchClient.Send(w);

                    Debug.Log("Client End Turn");
                }
            }*/
            if (!IsHost)
            {
                var w = new DataWriter(6);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.EndTurn);
                MatchClient.Send(w);

                Debug.Log("Client End Turn");
            }
            else
            {
                if (PlayerQueue.Count == 0)
                {
                    CreateUnitQueue();
                }
                SetPlayer(PlayerQueue.Dequeue());
                SetUnit(UnitQueue.Dequeue());
            }
        }

        public static void NextPlayer()
        {
            do
            {
                CurrentPlayer++;
                if (CurrentPlayer >= MatchInfo.PlayerInfos.Length)
                {
                    CurrentPlayer = 0;
                }
            }
            while (!PlayerHasAliveUnits(CurrentPlayer));

            SetPlayer(CurrentPlayer);
        }

        public static void SetState(ActionState state)
        {
            Debug.Log("SetState: " + state);
            State = state;
            if (state == ActionState.Move)
                Units[CurrentPlayer][CurrentUnitID].ComputePossibleMovementTiles();
            UpdateUnitUI();
            if (IsHost)
            {
                var w = new DataWriter(7);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.SetActionState);
                w.Write((byte)state);
                MatchServer.Send(w);
            }
        }

        public static void ReqSetState(ActionState state)
        {
            if (IsHost)
                SetState(state);
            else
            {
                var w = new DataWriter(7);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.ReqSetActionState);
                w.Write((byte)state);
                MatchClient.Send(w);
            }
        }

        public static Unit GetUnitOnTile(Int2 tile)
        {
            for (int i = 0; i < Units.Length; i++)
                foreach (var u in Units[i])
                    if (u.X == tile.X && u.Y == tile.Y && u.Stats.Alive)
                        return u;

            return null;
        }
    }
}
