using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Module_1_Async_Await_3
{
    public static class Server
    {
        private static Cart cart;
        private static List<Item> listItems;
        private static Mutex sync = new Mutex();
        public static async Task<Cart> AddItemAsync(Item item)
        {
            await Task.Delay(1450);
            try
            {
                sync.WaitOne();
                if (listItems == null)
                {
                    listItems = new List<Item>();
                }
                if (!listItems.Contains(item))
                {
                    listItems.Add(item);
                }
                cart = new Cart();
                foreach (var i in listItems)
                {
                    cart.CountItem++;
                    cart.TotalCost += i.Price;
                }
                sync.ReleaseMutex();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }
            
            return await Complete(cart);
        }
        public static async Task<Cart> RemoveItemAsync(Item item)
        {
            await Task.Delay(1250);
            try
            {
                sync.WaitOne();
                if (listItems == null)
                {
                    listItems = new List<Item>();
                }
                if (listItems.Contains(item))
                {
                    listItems.Remove(item);
                }
                cart = new Cart();
                foreach (var i in listItems)
                {
                    cart.CountItem++;
                    cart.TotalCost += i.Price;
                }
                sync.ReleaseMutex();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }

            return await Complete(cart);
        }

         private static Task<T> Complete<T>(T result)
        {
            var r = new TaskCompletionSource<T>();
            r.SetResult(result);
            return r.Task;
        }
    }
}
