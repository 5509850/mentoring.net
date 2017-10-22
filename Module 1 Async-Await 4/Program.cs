using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_1_Async_Await_4
{
    /// <summary>
    /// 4.	У вас есть Entity, которое описывает класс пользователя, 
    /// хранящийся в БД. Пользователь хранит информацию об Имени, Фамилии, Возрасте. 
    /// Напишите пример асинхронных CRUD операций для этого класса
    /// </summary>
    class Program
    {        
        static void Main(string[] args)
        {
            Go().Wait();
            Console.ReadKey();
        }

        private static async Task Go()
        {
            UserEntity user = new UserEntity
            {
                Age = 22,
                Name = "A",
                SurName = "S"
            };
            var id = await Insert(user);
            user = new UserEntity
            {
                Age = 33,
                Name = "B",
                SurName = "Sb"
            };
            var id2 = await Insert(user);
            var list = await Get();
            foreach (var item in list)
            {
                Print(item);
            }
            user.Name = "edited";
            Console.WriteLine("Update");
            var a = await Update(user);
            list = await Get();
            foreach (var item in list)
            {
                Print(item);
            }
            a = await Delete(user);
            Console.WriteLine("Delete");
            list = await Get();
            foreach (var item in list)
            {
                Print(item);
            }
            Console.ReadKey();
        }

        private static async Task<List<UserEntity>> Get()
        {
            try
            {
                return await DataBase.GetAsync();
            }
            catch (Exception)
            {

                return null;
            }
        }

        private static async Task<int> Insert(UserEntity item)
        {
            int result = 0;
            try
            {
                result = await DataBase.InsertAsync(item);
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                result = -1;
            }
            return result;
        }

        private static async Task<int> Delete(UserEntity item)
        {
            int result = 0;
            try
            {
                result = await DataBase.DeleteAsync(item);
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        private static async Task<int> Update(UserEntity item)
        {
            int result = 0;
            try
            {
                result = await DataBase.UpdateAsync(item);
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        private static void Print(UserEntity user)
        {
            Console.WriteLine($"id = {user.Id} {user.Name} {user.SurName} {user.Age} years");
        }
    }
}
