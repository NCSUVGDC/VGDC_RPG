using System;
using System.Collections.Generic;
using UnityEngine;
using VGDC_RPG.Networking;
using VGDC_RPG.TileObjects;
using VGDC_RPG.Units.Items;

namespace VGDC_RPG.Units
{
    public class Unit : INetClonable, INetEventHandler
    {
        public const ushort CLONE_OBJ_ID = 0;

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (Sprite != null)
                    Sprite.SetName(name);
            }
        }

        public int HandlerID { get; private set; }

        public int X = -1;
        public int Y = -1;

        public byte TeamID
        {
            get
            {
                return GameLogic.MatchInfo.PlayerInfos[PlayerID].Team;
            }
        }
        public byte PlayerID;

        public UnitStats Stats;
        private GameObject spriteObj;
        public PlayerSprite Sprite;

        public List<Int2> PossibleMovementTiles;

        public bool HasMoved;
        public bool HasAttacked;

        public Inventory Inventory;

        public Unit()
        {
            HandlerID = NetEvents.NextID();
            Stats = new UnitStats();
            CreateSprite();
            Name = "No Name";

            Inventory = new Inventory();
        }

        public Unit(DataReader r)
        {
            Debug.Log("UCL: " + r.Length);
            HandlerID = r.ReadInt32();
            PlayerID = r.ReadByte();
            CreateSprite();
            Sprite.PlayerID = PlayerID;
            Sprite.UnitID = r.ReadByte();
            X = r.ReadInt32();
            Y = r.ReadInt32();
            Name = r.ReadString();
            Stats = new UnitStats(r);
            Sprite.SetSpriteSet(r.ReadString());
            Inventory = NetEvents.GetHandler(r.ReadInt32()) as Inventory;

            SetPosition(X, Y);
            NetEvents.RegisterHandler(this);

            GameLogic.AddUnit(PlayerID, this);
        }

        private void CreateSprite()
        {
            spriteObj = UnityEngine.Object.Instantiate(Resources.Load("playersprite") as GameObject);
            spriteObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Sprite = spriteObj.GetComponent<PlayerSprite>();
        }

        public void Clone(DataWriter w)
        {
            w.Write((byte)NetCodes.Clone);
            w.Write(CLONE_OBJ_ID);
            w.Write(HandlerID);
            w.Write(PlayerID);
            w.Write(Sprite.UnitID);
            w.Write(X);
            w.Write(Y);
            w.Write(Name);
            Stats.NetAppend(w);
            Debug.Log("SANL: " + Sprite.AssetName.Length);
            w.Write(Sprite.AssetName);
            w.Write(Inventory.HandlerID);
        }

        public void HandleEvent(int cid, DataReader r)
        {
            if (!GameLogic.IsHost)
            {
                var et = (EventType)r.ReadByte();

                switch (et)
                {
                    case EventType.GoTo:
                        GoTo(r.ReadInt32(), r.ReadInt32());
                        break;
                    case EventType.SetPos:
                        SetPosition(r.ReadInt32(), r.ReadInt32());
                        break;
                    case EventType.Heal:
                        Heal(r.ReadInt32());
                        break;
                    case EventType.Damage:
                        Damage(r.ReadInt32());
                        break;
                    default:
                        throw new Exception("Invalid event type: " + et.ToString());
                }
            }
            //else
            //    throw new Exception("Server received event?");
        }

        public void SetPosition(int x, int y)
        {
            if (GameLogic.IsHost)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.SetPos);
                w.Write(x);
                w.Write(y);
                
                MatchServer.Send(w);
            }

            if (X != -1 && Y != -1)
                GameLogic.Map.UnblockTile(X, Y);
            X = x;
            Y = y;
            if (X != -1 && Y != -1)
                GameLogic.Map.BlockTile(X, Y);

            Sprite.transform.localPosition = new Vector3(X + 0.5f, Sprite.transform.localPosition.y, Y + 0.5f);
        }

        public void GoTo(int x, int y)
        {
            HasMoved = true;

            if (GameLogic.IsHost)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.GoTo);
                w.Write(x);
                w.Write(y);

                MatchServer.Send(w);
            }

            var path = Map.Pathfinding.AStarSearch.FindPath(GameLogic.Map, new Int2(X, Y), new Int2(x, y));

            if (path != null)
                Sprite.MoveOnPath(path);
            //if (!GameLogic.IsHost)
            {
                SetPosition(x, y);
            }

            //X = x;
            //Y = y;

            ///* TEMP */ // TODO
            //Sprite.transform.localPosition = new Vector3(X + 0.5f, Sprite.transform.localPosition.y, Y + 0.5f);
            ///* TEMP */
        }

        public void Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount", amount, "Heal amount was negative.");

            if (GameLogic.IsHost)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.Heal);
                w.Write(amount);

                MatchServer.Send(w);
            }

            Stats.HitPoints += amount;
            if (Stats.HitPoints > Stats.MaxHitPoints)
                Stats.HitPoints = Stats.MaxHitPoints;

            Sprite.SetHealth(Stats.HitPoints, Stats.MaxHitPoints);
        }

        public void Damage(int amount)
        {
            Debug.Log("Unit damaged: " + amount);

            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount", amount, "Damage amount was negative.");

            if (GameLogic.IsHost)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.Damage);
                w.Write(amount);

                MatchServer.Send(w);
            }

            Stats.HitPoints -= amount;
            if (Stats.HitPoints <= 0)
            {
                Stats.HitPoints = 0;
                if (Stats.Alive)
                {
                    Stats.Alive = false;
                    Sprite.SetAlive(Stats.Alive);
                    GameLogic.Map.UnblockTile(X, Y);
                }
            }

            Sprite.SetHealth(Stats.HitPoints, Stats.MaxHitPoints);
        }

        internal void ComputePossibleMovementTiles()
        {
            PossibleMovementTiles = Map.Pathfinding.AStarSearch.FindHighlight(GameLogic.Map, new Int2(X, Y), Stats.MovementRange);//PathFinder.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);
        }

        public void SelectMovement()
        {
            if (PossibleMovementTiles == null)
                return;
            foreach (var t in PossibleMovementTiles)
                GameLogic.Map.HighlightTile(t.X, t.Y, 1);
            GameLogic.Map.ApplyHightlight();
        }

        public void SelectAttack()
        {
            if (Inventory.SelectedWeapon == null)
                return;
            var at = Inventory.SelectedWeapon.GetAttackTiles(this);
            foreach (var t in at)
                GameLogic.Map.HighlightTile(t.X, t.Y, 2);
            GameLogic.Map.ApplyHightlight();
        }

        public void TurnReset()
        {
            HasMoved = false;
            HasAttacked = false;
        }

        public override string ToString()
        {
            return "Unit:" + Name + ":" + Inventory.HandlerID;
        }

        private enum EventType : byte
        {
            ERROR = 0,
            SetPos,
            GoTo,
            Heal,
            Damage
        }
    }
}
