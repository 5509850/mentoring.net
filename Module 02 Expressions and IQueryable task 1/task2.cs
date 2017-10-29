using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Module_02_Expressions_and_IQueryable_task_1
{
    /// <summary>
    /// 
    ///Используя возможность конструировать Expression Tree и выполнять его код, создайте собственный механизм маппинга (копирующего поля (свойства) одного класса в другой).
    ///Приблизительный интерфейс и пример использования приведен ниже
    ///(MapperGenerator – фабрика мапперов, Mapper – класс маппинга). 
    ///Обратите внимание, что в данном примере создается только новый экземпляр конечного класса, 
    ///но сами данные не копируются.
    ///    
    /// </summary>

    public class Task2
    {
        public void Run()
        {
            Console.Clear();
            Console.WriteLine("task 2");
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var res = mapper.Map(new Foo() { Name = "alex", Value = 5 });
            Console.WriteLine($"{res.GetType().Name}: Name = {res.Name}; Value = {res.Value}");          
            Console.ReadKey();
        }

        public class Mapper<TSource, TDestination>
        {
            Func<TSource, TDestination> mapFunction;
            internal Mapper(Func<TSource, TDestination> func)
            {
                mapFunction = func;
            }
            public TDestination Map(TSource source)
            {
                return mapFunction(source);
            }
        }
        public class MappingGenerator
        {
            public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
            {
                var type = typeof(TDestination);
                var newExpression = Expression.New(type);
                var sourceParam = Expression.Parameter(typeof(TSource));
                var list = new List<MemberBinding>();
                var propertyInfos = type.GetProperties(BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.SetProperty);

                foreach (var propertyInfo in propertyInfos)
                {
                    Expression call = Expression.Call(
                                                        typeof(FooExtension),
                                                         "GetValue", new[] { propertyInfo.PropertyType },
                                                        new Expression[]
                                                             {
                                                             sourceParam,
                                                             Expression.Constant(propertyInfo.Name)
                                                             });

                    MemberBinding mb = Expression.Bind(propertyInfo.GetSetMethod(), call);
                    list.Add(mb);
                }

                var mapFunction =
                    Expression.Lambda<Func<TSource, TDestination>>(
                    Expression.MemberInit(newExpression, list),
                    sourceParam
                    );

                return new Mapper<TSource, TDestination>(mapFunction.Compile());
            }
        }
    }

    static class FooExtension
    {
        public static TType GetValue<TType>(this Foo f, string name)
        {            
            PropertyDescriptor descr = TypeDescriptor.GetProperties(f)[name];
            object value = descr.GetValue(f);
            return (value != null) ? (TType)value : default(TType);
        }
    }
    public class Foo
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
    public class Bar
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
