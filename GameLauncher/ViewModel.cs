using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher
{
    public class GLViewModel
    {
        public ObservableCollection<Item> Items { get; set; }

        public GLViewModel()
        {
            Items = new ObservableCollection<Item>
            {
                new Item
                {
                    Name = "Test",
                    Category = "Official"
                },
                new Item
                {
                    Name = "Test 2",
                    Category = "Official"
                },
                new Item
                {
                    Name = "Test 3",
                    Category = "POWER"
                }
            };
        }
    }


    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
