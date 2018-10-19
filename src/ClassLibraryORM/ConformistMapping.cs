using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ClassLibraryORM.Converters;
using ClassLibraryORM.Managers;
using ImpromptuInterface;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode.Impl;

namespace ClassLibraryORM
{
    public class ConformistMapping<T> : IConformistHoldersProvider
    {
        private static readonly Type[] interfaceTypes = new Type[] {
            typeof(IClassMapper<object>),
            typeof(IClassAttributesMapper<object>),
            typeof(IEntityAttributesMapper),
            typeof(IEntitySqlsMapper),
            typeof(IPropertyContainerMapper<object>),
            typeof(ICollectionPropertiesContainerMapper<object>),
            typeof(IPlainPropertyContainerMapper<object>),
            typeof(IBasePlainPropertyContainerMapper<object>),
            typeof(IMinimalPlainPropertyContainerMapper<object>),
            typeof(IConformistHoldersProvider)
        };

        private readonly ClassMappingProxy proxy;
        private readonly IClassMapper<object> proxyMapper;

        public ConformistMapping() : this(TypeManager.Instance) { }

        public ConformistMapping(TypeManager manager)
        {
            var pair = manager.GetInterfaceClassPair(typeof(T));
            proxy = new ClassMappingProxy(pair.Class);
            proxyMapper = proxy.ActLike<IClassMapper<object>>(interfaceTypes);

            foreach (var action in ClassMappingManager.Instance.GetActionsForType<T>())
            {
                action?.Invoke(proxyMapper);
            }
        }

        public ICustomizersHolder CustomizersHolder => (proxy.ClassMapping as IConformistHoldersProvider).CustomizersHolder;

        public IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder => (proxy.ClassMapping as IConformistHoldersProvider).ExplicitDeclarationsHolder;

        private class ClassMappingProxy : DynamicObject
        {
            public Type EntityType { get; private set; }
            public Type ClassMappingType { get; private set; }
            public object ClassMapping { get; private set; }

            public ClassMappingProxy(Type entityType)
            {
                EntityType = entityType;
                ClassMappingType = typeof(ClassMapping<>).MakeGenericType(entityType);
                ClassMapping = Activator.CreateInstance(ClassMappingType);
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var argTypes = args.Select(arg => new ObjectType(arg));
                var processedArgTypes = ProcessArgumentTypes(argTypes);

                var processedArgs = processedArgTypes.Select(q => q.Obj).ToArray();
                var processedTypes = processedArgTypes.Select(q => q.Type).ToArray();

                var method = ClassMappingType.GetMethod(binder.Name, processedTypes);
                if(method == null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Unable to find method '{binder.Name}(...)' in '{ClassMappingType.Name}<{EntityType.Name}>' accepting the following argument types:");
                    foreach (var type in processedTypes)
                    {
                        sb.AppendLine(type.FullName);
                    }
                    throw new ArgumentException(sb.ToString());
                }

                result = method.Invoke(ClassMapping, processedArgs);
                return true;
            }

            private IEnumerable<ObjectType> ProcessArgumentTypes(IEnumerable<ObjectType> argumentTypes)
            {
                foreach (var argumentType in argumentTypes)
                {
                    yield return ObjectTypeConverter.Convert(argumentType, EntityType);
                }
            }
        }
    }
}
