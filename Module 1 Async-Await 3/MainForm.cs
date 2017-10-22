using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Module_1_Async_Await_3
{
    public partial class MainForm : Form
    {
        List<Item> items = new List<Item>();
       
        public MainForm()
        {
            InitializeComponent();
            InitItems();
        }

        private void InitItems()
        {
            items.Add(new Item {
                Id = 1,
                Description = "Food number 1",
                Price = 5
            });
            label3.Text = $"Price:  ${items[items.Count -1].Price}";            
            items.Add(new Item
            {
                Id = 2,
                Description = "Food number 2",
                Price = 15
            });
            label6.Text = $"Price:  ${items[items.Count - 1].Price}";
            items.Add(new Item
            {
                Id = 3,
                Description = "Food number 3",
                Price = 3
            });
            label8.Text = $"Price:  ${items[items.Count - 1].Price}";
            items.Add(new Item
            {
                Id = 4,
                Description = "Food number 4",
                Price = 6
            });
            label10.Text = $"Price:  ${items[items.Count - 1].Price}";
            items.Add(new Item
            {
                Id = 5,
                Description = "Food number 5",
                Price = 19
            });
            label12.Text = $"Price:  ${items[items.Count - 1].Price}";
            items.Add(new Item
            {
                Id = 6,
                Description = "Food number 6",
                Price = 24
            });
            label14.Text = $"Price:  ${items[items.Count - 1].Price}";
        }

        private void button_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"3.Напишите простейший магазин по заказу еды. 
Пользователь может выбрать товар, и он добавляется в корзину. 
При изменении товаров происходит автоматический пересчет стоимости. 
Любые действия пользователя с меню или корзиной не должны влиять 
на производительность UI (замораживать)..", "Описание программы", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(1, checkBox1.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(2, checkBox2.Checked);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(3, checkBox3.Checked);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(4, checkBox4.Checked);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(5, checkBox5.Checked);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCart(6, checkBox6.Checked);
        }

        private async void ChangeCart(int id, bool ischecked)
        {
            Cart cart = new Cart();
            if (ischecked)
            {              
                cart = await Server.AddItemAsync(GetItemById(id));
            }
            else
            {
                cart = await Server.RemoveItemAsync(GetItemById(id));
            }
            label1.Text = $"Subtotal({cart.CountItem} item)";
            label2.Text = $"{cart.TotalCost}.00";
        }

        private Item GetItemById(int id)
        {
            foreach (var item in items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        } 
    }
}
