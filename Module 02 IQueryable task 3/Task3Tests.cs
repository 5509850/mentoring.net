using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample03.E3SClient.Entities;
using Sample03.E3SClient;
using System.Configuration;
using System.Linq;

namespace Sample03
{
    /// <summary>
    /// Task 3
    /// </summary>


    //Доработайте приведенный на лекции LINQ провайдер.
    //В частности, требуется добавить следующее:
    
    //1)Снять текущее ограничение на порядок операндов выражения.Должны быть допустимы:
    //<имя фильтруемого поля> == <константа> (сейчас доступен только этот)
    //<константа> == <имя фильтруемого поля>
    
    //2) Добавить поддержку операций включения(т.е.не точное совпадение со строкой, а частичное). При этом в LINQ-нотации они должны выглядеть как обращение к методам класса string: StartsWith, EndsWith, Contains, а точнее
    //Выражение Транслируется в запрос
    //Where(e => e.workstation.StartsWith("EPRUIZHW006"))	workstation:(EPRUIZHW006*)
    //Where(e => e.workstation.EndsWith("IZHW0060"))	workstation:(* IZHW0060)
    //Where(e => e.workstation.Contains("IZHW006"))	workstation:(* IZHW006*)

    //3) Добавить поддержку оператора AND(потребует доработки также самого E3SQueryClient). Организацию оператора AND в запросе к E3S смотрите на странице документации(раздел FTS Request Syntax)


    [TestClass]
	public class Task3Tests
	{
		[TestMethod]
		public void WithoutProvider()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"] , ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

			foreach (var emp in res)
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
			}
		}

		[TestMethod]
		public void WithoutProviderNonGeneric()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

			foreach (var emp in res.OfType<EmployeeEntity>())
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
			}
		}

//TASK 3. Need edit <user> and <password> in "App.config"!!!!!!!!!!!!!!!!!
        [TestMethod]
		public void WithProvider()
		{
            int count = 0;

            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            Console.WriteLine("Part one");
            //<имя фильтруемого поля> == <константа>
            foreach (var emp in employees.Where(e => e.workstation == "EPRUIZHW0249"))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
            }
//PART 1
            //<константа> == <имя фильтруемого поля>
            foreach (var emp in employees.Where(e => "EPRUIZHW0249" == e.workstation))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
            }
            Console.WriteLine("**************************");
//PART 2
            Console.WriteLine("Part one");
            //StartsWith
            foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPRUIZHW024")))
            {
                Console.WriteLine("{0} {1} {2}", emp.nativename, emp.shortStartWorkDate, emp.workstation);
                count++;
            }
            Console.WriteLine("**************************");
            Console.WriteLine($"StartsWith count = {count}");
            Console.WriteLine("**************************");

            count = 0;
            //EndsWith
            foreach (var emp in employees.Where(e => e.workstation.EndsWith("IZHW0249")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
                count++;
            }
            Console.WriteLine("**************************");
            Console.WriteLine($"EndsWith count = {count}");
            Console.WriteLine("**************************");
            count = 0;
            //Contains
            foreach (var emp in employees.Where(e => e.workstation.Contains("IZHW024")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.shortStartWorkDate);
                count++;
            }
            Console.WriteLine("**************************");
            Console.WriteLine($"Contains count = {count}");
        }
    }
}
