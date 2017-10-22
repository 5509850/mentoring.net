using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Module_1_Async_Await_4
{
    public static class DataBase
    {
        private static List<UserEntity> listItems;
        private static Mutex sync = new Mutex();

        public static async Task<List<UserEntity>> GetAsync()
        {           
            if (listItems == null)
            {
                return null;
            }
            return await Complete(listItems);
        }
        public static async Task<int> InsertAsync(UserEntity item)
        {           
            int id = 1;
            try
            {
                sync.WaitOne();
                if (listItems == null)
                {
                    listItems = new List<UserEntity>();
                }
                if (listItems.Count > 0)
                {
                    id = listItems[listItems.Count - 1].Id + 1;
                }
                item.Id = id;
                listItems.Add(item);                         
                sync.ReleaseMutex();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }

            return id;
        }

        public static async Task<int> DeleteAsync(UserEntity item)
        {           
            int count = 0;
            try
            {
                sync.WaitOne();
                if (listItems == null || listItems.Count == 0)
                {
                    return count;
                }
                foreach (var it in listItems)
                {
                    if (it.Id == item.Id)
                    {
                        count = 1;
                    }
                }
                if (count != 0)
                {
                    listItems.Remove(item);
                }                
                sync.ReleaseMutex();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }
            return count;
        }

        public static async Task<int> UpdateAsync(UserEntity item)
        {
            int count = 0;
            try
            {
                sync.WaitOne();
                if (listItems == null || listItems.Count == 0)
                {
                    return count;
                }
                List<UserEntity> newlist = new List<UserEntity>();
                foreach (var it in listItems)
                {
                    if (it.Id == item.Id)
                    {
                        count = 1;
                        newlist.Add(item);
                    }
                    else
                    {
                        newlist.Add(it);
                    }
                }
                if (count != 0)
                {
                    listItems = newlist;
                }               
                sync.ReleaseMutex();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }
            return count;
        }

        private static Task<T> Complete<T>(T result)
        {
            var r = new TaskCompletionSource<T>();
            r.SetResult(result);
            return r.Task;
        }
    }
}
