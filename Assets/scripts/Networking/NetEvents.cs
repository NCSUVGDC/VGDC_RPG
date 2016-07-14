using System;
using System.Collections.Generic;
using UnityEngine;

namespace VGDC_RPG.Networking
{
    public static class NetEvents
    {
        private static Dictionary<int, INetEventHandler> handlers = new Dictionary<int, INetEventHandler>();

        public static void HandleEvent(DataReader r)
        {
            var id = r.ReadInt32();
            if (!handlers.ContainsKey(id))
                throw new Exception("Handler ID not found.");
            handlers[id].HandleEvent(r);
        }

        public static void RegisterHandler(INetEventHandler handler)
        {
#if LOG_NET
            Debug.Log("Registering handler with id: " + handler.HandlerID);
#endif
            handlers.Add(handler.HandlerID, handler);
        }

        public static void RemoveHandler(INetEventHandler handler)
        {
            RemoveHandler(handler.HandlerID);
        }

        public static void RemoveHandler(int id)
        {
#if LOG_NET
            Debug.Log("Removing handler with id: " + id);
#endif
            handlers.Remove(id);
        }
    }
}
