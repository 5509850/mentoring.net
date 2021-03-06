﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sample03
{
    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
            var name = node.Method.DeclaringType;

            if (node.Method.DeclaringType == typeof(Queryable)
				&& node.Method.Name == "Where")
			{                
                var predicate = node.Arguments[1];                
                Visit(predicate);

				return node;
			}
            if (node.Method.DeclaringType == typeof(System.String)
               && node.Method.Name == "StartsWith")
            {
                var predicate = node.Arguments[0];
                resultString.Append("(");
                Visit(predicate);
                resultString.Append("*)");

                return base.VisitMethodCall(node); //node;
            }
            if (node.Method.DeclaringType == typeof(System.String)
               && node.Method.Name == "EndsWith")
            {
                var predicate = node.Arguments[0];
                resultString.Append("(*");
                Visit(predicate);
                resultString.Append(")");

                return base.VisitMethodCall(node); //node;
            }
            if (node.Method.DeclaringType == typeof(System.String)
              && node.Method.Name == "Contains")
            {
                var predicate = node.Arguments[0];
                resultString.Append("(*");
                Visit(predicate);
                resultString.Append("*)");

                return base.VisitMethodCall(node); //node;
            }            
            return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
           
            switch (node.NodeType)
			{
				case ExpressionType.Equal:					
                    if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Left);
                        resultString.Append("(");
                        Visit(node.Right);
                        resultString.Append(")");
                    }
                    else if (node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
                    {
                        Visit(node.Right);
                        resultString.Append("(");
                        Visit(node.Left);
                        resultString.Append(")");
                    }
                    break;
                case ExpressionType.And:
                    {
                        Visit(node.Left);
                        resultString.Append(", ");
                        Visit(node.Right);
                    }
                    break;
                default:
					throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
			};

			return node;
		}       

        protected override Expression VisitMember(MemberExpression node)
		{
			resultString.Append(node.Member.Name).Append(":");

			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
            if (resultString != null && resultString.Length != 0)
            {
                string res = resultString.ToString();
                if (res.Contains(node.Value.ToString()))
                {
                    string[] s = res.Split(')');
                    resultString.Clear();
                    resultString.Append($"{s[1]}{s[0]})");
                }
                else
                {
                    resultString.Append(node.Value);
                }
            }
            else
            {
                resultString.Append(node.Value);
            }

			return node;
		}
	}
}
