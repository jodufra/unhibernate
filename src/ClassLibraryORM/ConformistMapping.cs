using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            typeof(IClassMapper<dynamic>),
            typeof(IClassAttributesMapper<dynamic>),
            typeof(IEntityAttributesMapper),
            typeof(IEntitySqlsMapper),
            typeof(IPropertyContainerMapper<dynamic>),
            typeof(ICollectionPropertiesContainerMapper<dynamic>),
            typeof(IPlainPropertyContainerMapper<dynamic>),
            typeof(IBasePlainPropertyContainerMapper<dynamic>),
            typeof(IMinimalPlainPropertyContainerMapper<dynamic>),
            typeof(IConformistHoldersProvider)
        };

        private readonly ClassMappingProxy proxy;
        private readonly IClassMapper<dynamic> proxyMapper;

        public ConformistMapping() : this(TypeManager.Instance) { }

        public ConformistMapping(TypeManager manager)
        {
            var pair = manager.GetInterfaceClassPair(typeof(T));
            proxy = new ClassMappingProxy(pair.Class);
            proxyMapper = proxy.ActLike<IClassMapper<dynamic>>(interfaceTypes);

            foreach (var action in ClassMappingManager.Instance.GetActionsForType<T>())
            {
                action?.Invoke(proxyMapper);
            }
        }

        public ICustomizersHolder CustomizersHolder => (proxy.ClassMapping as IConformistHoldersProvider).CustomizersHolder;

        public IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder => (proxy.ClassMapping as IConformistHoldersProvider).ExplicitDeclarationsHolder;

        private class ClassMappingProxy : DynamicObject
        {
            public Type GenericType { get; private set; }
            public Type ClassMappingType { get; private set; }
            public object ClassMapping { get; private set; }

            public ClassMappingProxy(Type classType)
            {
                GenericType = classType;
                ClassMappingType = typeof(ClassMapping<>).MakeGenericType(classType);
                ClassMapping = Activator.CreateInstance(ClassMappingType);
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var processedArgTypes = new List<ArgumentType>();
                foreach (var arg in args)
                {
                    processedArgTypes.Add(ProcessArgumentType(new ArgumentType { Arg = arg, Type = arg.GetType() }));
                }

                var processedArgs = processedArgTypes.Select(q => q.Arg).ToArray();
                var types = processedArgTypes.Select(q => q.Type).ToArray();

                var method = ClassMappingType.GetMethod(binder.Name, types);
                if(method == null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Unable to find method '{binder.Name}(...)' in '{ClassMappingType.Name}<{GenericType.Name}>' accepting the following argument types:");
                    foreach (var type in types)
                    {
                        sb.AppendLine(type.FullName);
                    }
                    throw new ArgumentException(sb.ToString());
                }

                result = method.Invoke(ClassMapping, processedArgs);
                return true;
            }

            private IEnumerable<ArgumentType> ProcessArgumentTypes(IEnumerable<ArgumentType> argumentTypes)
            {
                foreach (var argumentType in argumentTypes)
                {
                    yield return ProcessArgumentType(argumentType);
                }
            }

            private ArgumentType ProcessArgumentType(ArgumentType argumentType)
            {
                // TODO

                return argumentType;
            }

            private struct ArgumentType
            {
                public object Arg { get; set; }
                public Type Type { get; set; }
            }
        }
    }
}
