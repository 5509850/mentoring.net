using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Module_02_IQueryable_task_33
{
    /// <summary>
    /// Доработайте приведенный на лекции LINQ провайдер.
    ///Module 02 IQueryable task 3
    ///В частности, требуется добавить следующее:
    ///Снять текущее ограничение на порядок операндов выражения.Должны быть допустимы:
    ///<имя фильтруемого поля> == <константа> (сейчас доступен только этот)
    ///<константа> == <имя фильтруемого поля>
    ///Добавить поддержку операций включения(т.е.не точное совпадение со строкой, а частичное). 
    ///При этом в LINQ-нотации они должны выглядеть как обращение к методам класса string: StartsWith, EndsWith, Contains, 
    ///а точнее
    ///Выражение                                             Транслируется в запрос
    ///Where(e => e.workstation.StartsWith("EPRUIZHW006"))	workstation:(EPRUIZHW006*)
    ///Where(e => e.workstation.EndsWith("IZHW0060"))	    workstation:(* IZHW0060)
    ///Where(e => e.workstation.Contains("IZHW006"))	        workstation:(* IZHW006*)

    ///	Добавить поддержку оператора AND(потребует доработки также самого E3SQueryClient). 
    ///Организацию оператора AND в запросе к E3S смотрите на странице документации(раздел FTS Request Syntax)

    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Task 3");
            Console.ReadKey();
        }
    }    
}
