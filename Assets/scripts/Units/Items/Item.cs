using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    public class Item : INetEventHandler, IDisposable, INetClonable
    {
        public int HandlerID { get; private set; }

        public string Name;

        public Item()
        {
            HandlerID = NetEvents.NextID();
            NetEvents.RegisterHandler(this);
        }

        public Item(DataReader r)
        {
            HandlerID = r.ReadInt32();
            Name = r.ReadString();
            NetEvents.RegisterHandler(this);
        }

        public virtual void HandleEvent(int cid, DataReader r)
        {
            throw new Exception("Item event not handled.");
        }

        public void Dispose()
        {
            NetEvents.RemoveHandler(this);
        }

        public virtual void Clone(DataWriter w)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Item:" + Name;
        }
    }
}
