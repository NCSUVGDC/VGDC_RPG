using System;
using UnityEngine;
using VGDC_RPG.Units;
using VGDC_RPG.Units.Items;

namespace VGDC_RPG.Networking
{
    public static class NetCloner
    {
        private static Type[] typeList = new Type[]
        {
            typeof(Unit),
            typeof(Inventory),
            typeof(StandardMelee),
            typeof(BowWeapon),
            typeof(GrenadeWeapon),
            typeof(HealdingStaff)
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
