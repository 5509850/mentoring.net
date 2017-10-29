using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Module_02_Expressions_and_IQueryable_task_1
{
    /// <summary>
    /// 
    /// Создайте класс-трансформатор на основе ExpressionVisitor, выполняющий следующие 2 вида преобразований дерева выражений:
    ///	Замену выражений вида<переменная> + 1 / <переменная> - 1 на операции инкремента и декремента
    ///	Замену параметров, входящих в lambda-выражение, на константы(в качестве параметров такого преобразования передавать:
    /// Исходное выражение
    ///   Список пар <имя параметра: значение для замены>
    ///Для контроля полученное дерево выводить в консоль или смотреть результат под отладчиком, использую ExpressionTree Visualizer, 
    ///а также компилировать его и вызывать полученный метод.
    ///Создайте класс-трансформатор на основе ExpressionVisitor, выполняющий следующие 2 вида преобразований дерева выражений:
    ///	Замену выражений вида<переменная> + 1 / <переменная> - 1 на операции инкремента и декремента
    ///	Замену параметров, входящих в lambda-выражение, на константы (в качестве параметров такого преобразования передавать:
    /// Исходное выражение
    ///   Список пар <имя параметра: значение для замены>
    ///Для контроля полученное дерево выводить в консоль или смотреть результат под отладчиком, использую ExpressionTree Visualizer, 
    ///а также компилировать его и вызывать полученный метод.

    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var pair_replace = new Dictionary<string, object>();
            pair_replace.Add("C", 2);
            pair_replace.Add("i", 4);
            pair_replace.Add("n", 5);
            pair_replace.Add("M", 3);

            Expression<Func<int, int, int, int, int>> exp = (int C, int i, int n, int M) => -(C + 1) * (i + 4) * (1 + n) * (n - 2) * (M - 1) * (1 - M);
            var result_one = new Transform().VisitAndConvert(exp, "");
            var result_two = new Transform(exp, pair_replace).VisitAndConvert(exp, "");            

            Console.WriteLine("value");
            Console.WriteLine("C = 2, i = 4, n = 5, M = 3");
            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine(exp);
            Console.WriteLine();
            Console.WriteLine("Expression.compile");
            Console.WriteLine(exp.Compile().Invoke(2, 4, 5, 3));
            Console.WriteLine();

            Console.WriteLine("result_one");
            Console.WriteLine(result_one);
            Console.WriteLine();
            Console.WriteLine("result_one.compile");
            Console.WriteLine(result_one.Compile().Invoke(2, 4, 5, 3));
            Console.WriteLine();

            Console.WriteLine("result_two from Expression");
            Console.WriteLine(result_two);
            Console.WriteLine("result_two.compile");
            Console.WriteLine(result_two.Compile().Invoke(0, 0, 0, 0));
            Console.WriteLine();
            Console.ReadKey();
        }

        private class Transform : ExpressionVisitor
        {
            private readonly Dictionary<string, object> memberObject = null; 
             
            public Transform()
            {             
            }           

            public Transform(Expression expression, Dictionary<string, object> memberObject)
            {                
                this.memberObject = memberObject;               
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (memberObject == null)
                {
                   return TransformPart1(node);
                }
                return TransformPart2(node);
            }         

            private Expression TransformPart1(BinaryExpression node)
            {
                ParameterExpression param = null;
                ConstantExpression constant = null;
                if (node.NodeType == ExpressionType.Add)
                {
                    if (node.Left.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Left;
                    }
                    else if (node.Left.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Left;
                    }

                    if (node.Right.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Right;
                    }
                    else if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Right;
                    }

                    if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                    {
                        return Expression.Increment(param);
                    }

                }
                else if (node.NodeType == ExpressionType.Subtract)
                {
                    if (node.Left.NodeType == ExpressionType.Parameter)
                    {
                        param = (ParameterExpression)node.Left;
                    }

                    if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Right;
                    }
                   
                    if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                    {
                        return Expression.Decrement(param);
                    }
                }
                return base.VisitBinary(node);
            }

            private Expression TransformPart2(BinaryExpression node)
            {
                ParameterExpression param = null;
                ConstantExpression constant = null;
                object pair;
                if (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
                {
                    if (node.Left.NodeType == ExpressionType.Parameter && node.Right.NodeType == ExpressionType.Constant)
                    {
                        param = (ParameterExpression)node.Left;
                        constant = (ConstantExpression)node.Right;
                        if (memberObject != null && param != null && constant != null)
                        {
                            if (memberObject.TryGetValue(param.Name, out pair))
                            {
                                var cons = Expression.Constant(value: pair, type: pair.GetType());
                                if (node.NodeType == ExpressionType.Add)
                                {
                                    return Expression.Add(cons, constant);
                                }
                                else
                                {
                                    return Expression.Subtract(cons, constant);
                                }                                
                            }
                        }
                    }
                    else if (node.Right.NodeType == ExpressionType.Parameter && node.Left.NodeType == ExpressionType.Constant)
                    {
                        param = (ParameterExpression)node.Right;
                        constant = (ConstantExpression)node.Left;
                        if (memberObject != null && param != null && constant != null)
                        {
                            if (memberObject.TryGetValue(param.Name, out pair))
                            {
                                var cons = Expression.Constant(value: pair, type: pair.GetType());
                                if (node.NodeType == ExpressionType.Add)
                                {
                                    return Expression.Add(constant, cons);
                                }
                                else
                                {
                                    return Expression.Subtract(constant, cons);
                                }                                   
                            }
                        }
                    }
                }               
                return base.VisitBinary(node);
            }          
        }
    }
}
