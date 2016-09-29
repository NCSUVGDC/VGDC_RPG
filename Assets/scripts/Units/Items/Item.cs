using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    /// <summary>
    /// Base class for all inventory items.
    /// </summary>
    public class Item : INetEventHandler, IDisposable, INetClonable
    {
        /// <summary>
        /// ID for this items NetEvent handler.
        /// </summary>
        public int HandlerID { get; private set; }
        /// <summary>
        /// The displayed name of the item.
        /// </summary>
        public string Name;

        /// <summary>
        /// Construct a new Item and registers it as a NetEvent handler.
        /// </summary>
        public Item()
        {
            HandlerID = NetEvents.NextID();
            NetEvents.RegisterHandler(this);
        }

        /// <summary>
        /// Constructs an Item from the network data in a DataReader.
        /// </summary>
        /// <param name="r">The DataReader containing the data to create from.</param>
        public Item(DataReader r)
        {
            HandlerID = r.ReadInt32();
            Name = r.ReadString();
            NetEvents.RegisterHandler(this);
        }

        /// <summary>
        /// NetEvent handler.
        /// </summary>
        /// <param name="cid">The ID of the connection where the event originates.</param>
        /// <param name="r">The DataReader containing the network data.</param>
        public virtual void HandleEvent(int cid, DataReader r)
        {
            throw new Exception("Item event not handled.");
        }

        public void Dispose()
        {
            NetEvents.RemoveHandler(this);
        }

        /// <summary>
        /// Clones the items data into a DataWriter to be contructed over the network.
        /// </summary>
        /// <param name="w">The DataWriter to write the items data to.</param>
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
