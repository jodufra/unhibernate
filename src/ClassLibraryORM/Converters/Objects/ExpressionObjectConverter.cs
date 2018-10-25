using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ExpressionObjectConverter : ObjectConverter
    {
        public ExpressionObjectConverter(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
        {
            if (!objectType.Type.IsExpressionType())
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

            var returnType = expression.ReturnType;
            var newReturnType = ObjectTypeConverter.Convert(returnType, targetType);

            if (returnType == newReturnType)
            {
                return newObjectType;
            }
            
            var typeOfVisitor = typeof(ReturnTypeVisitor);
            var visitor = (ExpressionVisitor)Activator.CreateInstance(typeOfVisitor, expression.Parameters.Select(q => q.Type), newReturnType);
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

        private class ReturnTypeVisitor : ExpressionVisitor
        {
            private readonly IEnumerable<Type> argumentTypes;
            private readonly Type returnType;

            public ReturnTypeVisitor(IEnumerable<Type> argumentTypes, Type returnType)
            {
                this.argumentTypes = argumentTypes ?? throw new ArgumentNullException(nameof(argumentTypes));
                this.returnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var types = new List<Type>(argumentTypes)
                {
                    returnType
                }.ToArray();

                var delegateType = typeof(Func<>).MakeGenericType(types);
                var body = Visit(node.Body);

                return Expression.Lambda(delegateType, body, node.Parameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.DeclaringType != typeof(object))
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
