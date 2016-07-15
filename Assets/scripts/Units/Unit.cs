using System;
using UnityEngine;
using VGDC_RPG.Networking;
using VGDC_RPG.TileObjects;

namespace VGDC_RPG.Units
{
    public class Unit : INetClonable, INetEventHandler
    {
        public const ushort CLONE_OBJ_ID = 1;

        public string Name { get; set; }

        public int HandlerID { get; private set; }

        public int X = -1;
        public int Y = -1;

        public UnitStats Stats;
        private GameObject spriteObj;
        public PlayerSprite Sprite;

        public Unit()
        {
            HandlerID = NetEvents.NextID();
            Stats = new UnitStats();
            Name = "No Name";
            CreateSprite();
        }

        public Unit(DataReader r)
        {
            Debug.Log("UCL: " + r.Length);
            HandlerID = r.ReadInt32();
            X = r.ReadInt32();
            Y = r.ReadInt32();
            Name = r.ReadString();
            Stats = new UnitStats(r);
            CreateSprite();
            Sprite.SetSpriteSet(r.ReadString());

            SetPosition(X, Y);
            NetEvents.RegisterHandler(this);
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
            w.Write(X);
            w.Write(Y);
            w.Write(Name);
            Stats.NetAppend(w);
            Debug.Log("SANL: " + Sprite.AssetName.Length);
            w.Write(Sprite.AssetName);
        }

        public void HandleEvent(DataReader r)
        {
            if (!GameLogic.Instance.IsHost)
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
            else
                throw new Exception("Server received event?");
        }

        public void SetPosition(int x, int y)
        {
            if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.SetPos);
                w.Write(x);
                w.Write(y);
                
                MatchServer.Send(w);
            }

            X = x;
            Y = y;

            Sprite.transform.localPosition = new Vector3(X + 0.5f, Sprite.transform.localPosition.y, Y + 0.5f);
        }

        public void GoTo(int x, int y)
        {
            if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.GoTo);
                w.Write(x);
                w.Write(y);

                MatchServer.Send(w);
            }

            var path = Map.Pathfinding.AStarSearch.FindPath(GameLogic.Instance.Map, new Int2(X, Y), new Int2(x, y));

            if (path != null)
                Sprite.MoveOnPath(path);
            if (!GameLogic.Instance.IsHost)
            {
                SetPosition(x, y);
            }

            X = x;
            Y = y;

            ///* TEMP */ // TODO
            //Sprite.transform.localPosition = new Vector3(X + 0.5f, Sprite.transform.localPosition.y, Y + 0.5f);
            ///* TEMP */
        }

        public void Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount", amount, "Heal amount was negative.");

            if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
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
        }

        public void Damage(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount", amount, "Damage amount was negative.");

            if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            {
                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.Damage);
                w.Write(amount);

                MatchServer.Send(w);
            }

            Stats.HitPoints -= amount;
            if (Stats.HitPoints < 0)
            {
                Stats.HitPoints = 0;
                Stats.Alive = false; //TODO Killed
            }
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
