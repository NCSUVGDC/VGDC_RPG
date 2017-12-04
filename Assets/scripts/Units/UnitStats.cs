using UnityEngine;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units
{
    public struct UnitStats
    {
        public int MaxHitPoints;
        public int HitPoints;

        public int Defense;

        public int Damage;

        public int MovementRange;

        public int Range;

        public int Initiative;

        public bool Alive;

        public byte SelectedStone;

        public byte Type;

        public UnitStats(DataReader r)
        {
            MaxHitPoints = r.ReadInt32();
            HitPoints = r.ReadInt32();
            MovementRange = r.ReadInt32();
            Range = r.ReadInt32();
            Initiative = r.ReadInt32();
            Alive = r.ReadByte() != 0;
            SelectedStone = r.ReadByte();
            Defense = r.ReadInt32();
            Damage = r.ReadInt32();
            Type = r.ReadByte();
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
            w.Write(Defense);
            w.Write(Damage);
        }

        public int GetAttackDmg(int wpnDmg, UnitStats other)
        {
            Debug.Log("Unit's Stone: " + this.SelectedStone + " vs. Enemy's Stone: " + other.SelectedStone);
            Debug.Log("Damage before bonuses: " + (Damage + wpnDmg));
            Debug.Log("Effective bonus: " + Stones.Effectiveness[SelectedStone - 1, other.SelectedStone - 1]);

            int damageBonus = Mathf.CeilToInt(Damage * (Stones.Damage[Type, SelectedStone - 1] / 2));
            float stoneBonus = Stones.Effectiveness[SelectedStone - 1, other.SelectedStone - 1];
            int defenseBonus = Mathf.FloorToInt(other.Defense * (Stones.Defense[other.Type, other.SelectedStone - 1]));

            // Test total damage dealt
            Debug.Log("Total Damage: " + Mathf.FloorToInt((Damage + damageBonus + wpnDmg) * stoneBonus - (other.Defense + defenseBonus)));

            // TotalDamage = Damage + wpnDamage * StoneEffectiveness - target_defense
            return Mathf.FloorToInt((Damage + damageBonus + wpnDmg) * stoneBonus - (other.Defense + defenseBonus));
        }
    }
}
