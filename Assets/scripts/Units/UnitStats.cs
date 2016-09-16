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

        public UnitStats(DataReader r)
        {
            MaxHitPoints = r.ReadInt32();
            HitPoints = r.ReadInt32();
            MovementRange = r.ReadInt32();
            Range = r.ReadInt32();
            Initiative = r.ReadInt32();
            Alive = r.ReadByte() != 0;
        }

        public void NetAppend(DataWriter w)
        {
            w.Write(MaxHitPoints);
            w.Write(HitPoints);
            w.Write(MovementRange);
            w.Write(Range);
            w.Write(Initiative);
            w.Write((byte)(Alive ? 1 : 0));
        }
    }
}
