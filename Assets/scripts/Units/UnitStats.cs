using UnityEngine;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units
{
    public struct UnitStats
    {
        public int MaxHitPoints;
        public int HitPoints;

        public int MovementRange;

        public int Range;

        public int Initiative;

        public bool Alive;

        public byte SelectedStone;

        public UnitStats(DataReader r)
        {
            MaxHitPoints = r.ReadInt32();
            HitPoints = r.ReadInt32();
            MovementRange = r.ReadInt32();
            Range = r.ReadInt32();
            Initiative = r.ReadInt32();
            Alive = r.ReadByte() != 0;
            SelectedStone = r.ReadByte();
        }

        public void NetAppend(DataWriter w)
        {
            w.Write(MaxHitPoints);
            w.Write(HitPoints);
            w.Write(MovementRange);
            w.Write(Range);
            w.Write(Initiative);
            w.Write((byte)(Alive ? 1 : 0));
            w.Write(SelectedStone);
        }

        public int GetAttackDmg(int baseDmg, UnitStats other)
        {
            Debug.Log("Getting attack damage.\nUnit's Stone: " + Stones.UIText[this.SelectedStone] + " vs. Enemy's Stone: " + Stones.UIText[other.SelectedStone]);
            Debug.Log("Effectiveness: " + Stones.Effectiveness[SelectedStone, other.SelectedStone]);
            return Mathf.FloorToInt(baseDmg * Stones.Effectiveness[SelectedStone, other.SelectedStone]);
        }
    }
}
