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

        public int NumItems { get { return items.Count; } }

        public Inventory()
        {
            HandlerID = NetEvents.NextID();
            items = new List<int>();
            NetEvents.RegisterHandler(this);

            //===TEMP===
            //var w = new DataWriter(512);
            var melee = new StandardMelee();
            //melee.Clone(w);

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

        }

        public void RemoveItem(int item, bool netevent)
        {
            items.Remove(item);

        }

        public void SelectWeapon(int item, bool netevent)
        {
            SelectedWeapon = NetEvents.GetHandler(item) as Weapon;
        }

        public Item GetItemAtIndex(int index)
        {
            return (Item) NetEvents.GetHandler(items[index]);
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
