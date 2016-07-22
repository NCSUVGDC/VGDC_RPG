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

                public PlayerInfo(string name, PlayerType type, byte team)
                {
                    PlayerName = name;
                    PlayerType = type;
                    Team = team;
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
                            StartMatch();
                            break;
                        case EventType.ClickTile:
                            ClickTile(CIDPlayers[cid], new Int2(r.ReadInt32(), r.ReadInt32()));
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
            ClickTile
        }

        public static TileMap Map;
        public static GameObject Camera;

        public static ushort[][,] mapConstructionData;

        public static bool IsHost, IsServer;

        public static List<Unit>[] Units;

        private static byte[] netBuffer = new byte[512];

        public static int TeamCount;
        public static byte CurrentPlayer;

        private static EventHandler eh;

        public static Dictionary<int, byte> CIDPlayers;
        public static int[] PlayersCID;
        
        public static void Init()
        {
            Units = new List<Unit>[MatchInfo.PlayerInfos.Length];
            CIDPlayers = new Dictionary<int, byte>();
            PlayersCID = new int[MatchInfo.PlayerInfos.Length];

            for (int i = 0; i < MatchInfo.PlayerInfos.Length; i++)
            {
                MatchInfo.PlayerInfos[i] = new MatchInfo.PlayerInfo("Empty", MatchInfo.PlayerType.None, 0);
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

        public static void SetMap(ushort[][,] data)
        {
            mapConstructionData = data;
            if (IsHost && IsServer)
                TileMapSender.SendToAll(data);
        }

        public static void BuildMap()
        {
            Map = TileMap.Construct(mapConstructionData);
        }

        public static void NextTeam()
        {
            do
            {
                CurrentPlayer++;
                if (CurrentPlayer >= MatchInfo.PlayerInfos.Length)
                    CurrentPlayer = 0;
            }
            while (PlayerHasAliveUnits(CurrentPlayer));
        }

        public static void AddUnit(int player, Unit unit)
        {
            Units[player].Add(unit);
            if (IsHost && IsServer)
            {
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

            if (IsHost && IsServer)
            {
                var w = new DataWriter(6);
                w.Write((byte)NetCodes.Event);
                w.Write(eh.HandlerID);
                w.Write((byte)EventType.StartMatch);
                MatchServer.Send(w);
            }
        }

        public static void ClickTile(Int2 t)
        {
            if (IsHost && IsServer)
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
        }
    }
}
