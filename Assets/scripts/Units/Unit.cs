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
        /// <summary>
        /// The network clonable object ID.
        /// </summary>
        public const ushort CLONE_OBJ_ID = 0;

        private string name;
        /// <summary>
        /// The displayed name of the Unit.
        /// </summary>
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

        /// <summary>
        /// The ID of the NetEvents handler.
        /// </summary>
        public int HandlerID { get; private set; }

        /// <summary>
        /// The X-coordinate on the map.
        /// </summary>
        public int X = -1;
        /// <summary>
        /// The Y-coordinate on the map.
        /// </summary>
        public int Y = -1;

        /// <summary>
        /// This Units team ID.
        /// </summary>
        public byte TeamID
        {
            get
            {
                return GameLogic.MatchInfo.PlayerInfos[PlayerID].Team;
            }
        }
        /// <summary>
        /// This Units player ID.
        /// </summary>
        public byte PlayerID;

        /// <summary>
        /// This Units stats.
        /// </summary>
        public UnitStats Stats;
        private GameObject spriteObj;
        /// <summary>
        /// The sprite of this Unit.  This Units graphical representation rendered in the game.
        /// </summary>
        public PlayerSprite Sprite;

        /// <summary>
        /// List of possible tiles for movement.
        /// </summary>
        public List<Int2> PossibleMovementTiles;

        /// <summary>
        /// True if the Unit has already moved this turn.
        /// </summary>
        public bool HasMoved;
        /// <summary>
        /// True if the Unit has already attacked this turn.
        /// </summary>
        public bool HasAttacked;
        /// <summary>
        /// True if unit has not already used its potion
        /// </summary>
        public bool potionReady;

        /// <summary>
        /// This Units inventory.
        /// </summary>
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

        /// <summary>
        /// Sets the position of the Unit without animation.
        /// </summary>
        /// <param name="x">The X-coordinate to set.</param>
        /// <param name="y">The Y-coordinate to set.</param>
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

        /// <summary>
        /// Sets the position of the Unit with animation.
        /// </summary>
        /// <param name="x">The X-coordinate to set.</param>
        /// <param name="y">The Y-coordinate to set.</param>
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

        /// <summary>
        /// Heals the Unit by a specified amount.
        /// </summary>
        /// <param name="amount">The amount to heal by.</param>
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

        /// <summary>
        /// Damages the Unit by a specified amount.
        /// </summary>
        /// <param name="amount">The amount to damage by.</param>
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

        /// <summary>
        /// Highlights the possible movement tiles on the map.
        /// </summary>
        public void SelectMovement()
        {
            if (PossibleMovementTiles == null)
                return;
            foreach (var t in PossibleMovementTiles)
                GameLogic.Map.HighlightTile(t.X, t.Y, 1);
            GameLogic.Map.ApplyHightlight();
        }

        /// <summary>
        /// Highlights the possible attack tiles on the map.
        /// </summary>
        public void SelectAttack()
        {
            if (Inventory.SelectedWeapon == null)
                return;
            var at = Inventory.SelectedWeapon.GetAttackTiles(this);
            foreach (var t in at)
                GameLogic.Map.HighlightTile(t.X, t.Y, 2);
            GameLogic.Map.ApplyHightlight();
        }

        /// <summary>
        /// Displays inventory UI
        /// </summary>
        public void SelectInventory()
        {
            /// Spawn two buttons for potions
            for (int i = 0; i < 2; i++)
            {

                /// GameLogic.Units[GameLogic.CurrentPlayer][GameLogic.CurrentUnitID].Inventory.NumItems
            }
                
        }

        public void SelectPotion()
        {
            GameLogic.Units[GameLogic.CurrentPlayer][GameLogic.CurrentUnitID].Heal(10);
            GameLogic.EndTurn();
        }

        /// <summary>
        /// Called before the Unit's turn begins, resets flags to default state.
        /// </summary>
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
