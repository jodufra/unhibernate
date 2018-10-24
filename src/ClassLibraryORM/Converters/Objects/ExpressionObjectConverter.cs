﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ExpressionObjectConverter : ObjectConverter
    {
        public ExpressionObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsExpression())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }

        public override ObjectType Convert()
        {
            if (!(objectType.Obj is LambdaExpression lambdaExpression))
            {
                return objectType;
            }

            if (lambdaExpression.Parameters.Count > 1)
            {
                throw new NotImplementedException();
            }

            var newObjectType = objectType;

            var returnType = lambdaExpression.ReturnType;
            if (returnType == typeof(object))
            {
                newObjectType = ConvertReturnType(newObjectType);
            }

            foreach (var parameter in lambdaExpression.Parameters)
            {
                if (parameter.Type == typeof(object))
                {
                    newObjectType = ConvertParamenter(parameter, newObjectType);
                }
            }

            return newObjectType;
        }

        protected ObjectType ConvertReturnType(ObjectType newObjectType)
        {
            var expression = objectType.Obj as LambdaExpression;

            var type = expression.ReturnType;
            var newType = ObjectTypeConverter.Convert(type, targetType);

            if (type == newType)
            {
                return newObjectType;
            }

            var typeArguments = new Type[] {
                expression.Parameters[0].Type,
                newType
            };

            var typeOfVisitor = typeof(ReturnTypeVisitor<,>).MakeGenericType(typeArguments);
            var visitor = (ExpressionVisitor)Activator.CreateInstance(typeOfVisitor);
            var newExpression = visitor.Visit(expression);

            return new ObjectType(newExpression);
        }

        protected ObjectType ConvertParamenter(ParameterExpression parameter, ObjectType newObjectType)
        {
            var type = parameter.Type;
            var newType = ObjectTypeConverter.Convert(type, targetType);

            if (type == newType)
            {
                return newObjectType;
            }

            var newParameter = Expression.Parameter(newType, parameter.Name);

            var typeOfVisitor = typeof(ParameterVisitor);
            var visitor = (ExpressionVisitor)Activator.CreateInstance(typeOfVisitor, newParameter);

            var expression = objectType.Obj as LambdaExpression;
            var newExpressionBody = visitor.Visit(expression.Body);
            var newExpression = Expression.Lambda(newExpressionBody, newParameter);

            return new ObjectType(newExpression);
        }

        private class ReturnTypeVisitor<TSource, TReturnValue> : ExpressionVisitor
        {
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var delegateType = typeof(Func<,>).MakeGenericType(typeof(TSource), typeof(TReturnValue));
                var body = Visit(node.Body);

                return Expression.Lambda(delegateType, body, node.Parameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.DeclaringType != typeof(TSource))
                {
                    return base.VisitMember(node);
                }

                var memberName = node.Member.Name;
                var inner = Visit(node.Expression);

                return Expression.Property(inner, memberName);
            }
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            public ParameterVisitor(ParameterExpression parameter)
            {
                _parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node.Type != typeof(object))
                {
                    return base.VisitParameter(node);
                }

                return _parameter;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.MemberType != MemberTypes.Property)
                {
                    return base.VisitMember(node);
                }

                var property = (PropertyInfo)node.Member;
                var inner = Visit(node.Expression);

                return Expression.Property(inner, property);
            }
        }
    }
}
