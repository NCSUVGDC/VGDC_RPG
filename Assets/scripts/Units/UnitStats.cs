using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units
{
    public struct UnitStats
    {
        public int HitPoints;

        public UnitStats(DataReader r)
        {
            HitPoints = r.ReadInt32();
        }

        public void NetAppend(DataWriter w)
        {
            w.Write(HitPoints);
        }
    }
}
