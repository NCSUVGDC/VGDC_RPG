using System;
using UnityEngine;

namespace VGDC_RPG.Networking
{
    public static class NetCloner
    {
        private static Type[] typeList = new Type[]
        {
            typeof(GameObject)
        };

        public static void HandleClone(DataReader r)
        {
            var objType = r.ReadUInt16();
#if LOG_NET
            Debug.Log("Cloning object type id: " + objType);
#endif

            if (objType >= typeList.Length)
                throw new Exception("Invalid object type ID.");

            Activator.CreateInstance(typeList[objType], r);
        }
    }
}
