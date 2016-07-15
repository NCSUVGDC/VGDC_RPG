using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units
{
    public struct UnitStats
    {
        public int MaxHitPoints;
        public int HitPoints;

        public bool Alive;

        public UnitStats(DataReader r)
        {
            MaxHitPoints = r.ReadInt32();
            HitPoints = r.ReadInt32();
            Alive = r.ReadByte() != 0;
        }

        public void NetAppend(DataWriter w)
        {
            w.Write(MaxHitPoints);
            w.Write(HitPoints);
            w.Write((byte)(Alive ? 1 : 0));
        }
    }
}
