using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    public class Inventory : INetEventHandler, INetClonable, IDisposable
    {
        public const ushort CLONE_OBJ_ID = 1;

        public int HandlerID { get; private set; }

        private List<int> items;

        public Weapon SelectedWeapon;

        public Inventory()
        {
            HandlerID = NetEvents.NextID();
            items = new List<int>();
            NetEvents.RegisterHandler(this);

            //===TEMP===
            //var w = new DataWriter(512);
            var melee = new StandardMelee();
            //melee.Clone(w);
            //MatchServer.Send(w);
            if (GameLogic.IsHost)
            {
                var w = new DataWriter(256);
                Clone(w);
                MatchServer.Send(w);
            }

            AddItem(melee.HandlerID, false);
            SelectWeapon(melee.HandlerID, false);
            //==========



        }

        public Inventory(DataReader r)
        {
            HandlerID = r.ReadInt32();
            Debug.Log("Creating inventory with id: " + HandlerID);
            var count = r.ReadInt32();
            items = new List<int>();
            for (int i = 0; i < count; i++)
                items.Add(r.ReadInt32());
            int selectedWeapon = r.ReadInt32();
            if (selectedWeapon != -1)
                SelectedWeapon = NetEvents.GetHandler(selectedWeapon) as Weapon;
            NetEvents.RegisterHandler(this);
        }

        public void Clone(DataWriter w)
        {
            w.Write((byte)NetCodes.Clone);
            w.Write(CLONE_OBJ_ID);
            w.Write(HandlerID);
            w.Write(items.Count);
            foreach (var i in items)
                w.Write(i);
            w.Write(SelectedWeapon == null ? -1 : SelectedWeapon.HandlerID);
        }

        public void HandleEvent(int cid, DataReader r)
        {
            var et = (EventType)r.ReadByte();

            switch (et)
            {
                case EventType.AddItem:
                    AddItem(r.ReadInt32(), false);
                    break;
                case EventType.RemoveItem:
                    RemoveItem(r.ReadInt32(), false);
                    break;
                case EventType.SelectWeapon:
                    SelectWeapon(r.ReadInt32(), false);
                    break;
                default:
                    throw new Exception("Invalid event type: " + et.ToString());
            }
        }

        public void AddItem(int item, bool netevent)
        {
            items.Add(item);
            //if (netevent)
            {
                var w = new DataWriter(10);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.AddItem);
                w.Write(item);
                if (GameLogic.IsHost)
                    MatchServer.Send(w);
                else
                    MatchClient.Send(w);
            }
        }

        public void RemoveItem(int item, bool netevent)
        {
            items.Remove(item);
            //if (netevent)
            {
                var w = new DataWriter(10);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.RemoveItem);
                w.Write(item);
                if (GameLogic.IsHost)
                    MatchServer.Send(w);
                else
                    MatchClient.Send(w);
            }
        }

        public void SelectWeapon(int item, bool netevent)
        {
            SelectedWeapon = NetEvents.GetHandler(item) as Weapon;
            //if (netevent)
            {
                var w = new DataWriter(10);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.SelectWeapon);
                w.Write(item);
                if (GameLogic.IsHost)
                    MatchServer.Send(w);
                else
                    MatchClient.Send(w);
            }
        }

        public void Dispose()
        {
            NetEvents.RemoveHandler(this);
        }

        public override string ToString()
        {
            return "Inventory";
        }

        private enum EventType : byte
        {
            ERROR = 0,
            AddItem,
            RemoveItem,
            SelectWeapon
        }
    }
}
