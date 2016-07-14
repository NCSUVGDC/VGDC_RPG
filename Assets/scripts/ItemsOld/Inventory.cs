//using System.Collections;
//using System.Collections.Generic;
//using VGDC_RPG.Players;

//namespace VGDC_RPG.Items
//{
//    public class Inventory : IEnumerable<Item>
//    {
//        private List<Item> items;

//        public Inventory()
//        {
//            items = new List<Item>();
//        }

//        public void Add(Item item)
//        {
//            items.Add(item);
//        }

//        public IList<Item> this[string category]
//        {
//            get
//            {
//                var r = new List<Item>();
//                foreach (var i in items)
//                    if (i.Category == category)
//                        r.Add(i);
//                return r;
//            }
//        }

//        public IList<string> GetCategories()
//        {
//            var r = new List<string>();
//            foreach (var i in items)
//                if (!r.Contains(i.Category))
//                    r.Add(i.Category);
//            return r;
//        }

//        public IEnumerator<Item> GetEnumerator()
//        {
//            return items.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return items.GetEnumerator();
//        }
        
//        public void Use(Item item, Player useOn)
//        {
//            item.Use(useOn);
//            if (item.Consumable)
//                Remove(item);
//        }

//        public void Remove(Item item)
//        {
//            items.Remove(item);
//        }

//        public int Count
//        {
//            get { return items.Count; }
//        }

//        public Item this[int i]
//        {
//            get { return items[i]; }
//        }
//    }
//}
