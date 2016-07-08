namespace VGDC_RPG.Items
{
    public abstract class Item
    {
        public abstract string GUIName { get; }
        public abstract string Category { get; }

        //public abstract int InstantHP { get; }
        public abstract Players.PlayerEffect UseEffect { get; }

        public abstract bool Consumable { get; }
        public abstract bool RequiresAction { get; }

        public virtual void Use(Players.Player useOn)
        {
            //if (InstantHP != 0 && useOn.HitPoints > 0)
            //    if (InstantHP > 0)
            //        useOn.Heal(InstantHP);
            //    else
            //        useOn.Damage(InstantHP);
            useOn.AddEffect(UseEffect);
        }
    }
}
