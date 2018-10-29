using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnHibernate.Converters;
using UnHibernate.Managers;
using ImpromptuInterface;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode.Impl;
using UnHibernate.Exceptions;

namespace UnHibernate
{
    public class ConformistMapping<T> : IConformistHoldersProvider
    {
        private readonly ClassMappingProxy proxy;
        private readonly IClassMapper<object> proxyMapper;

        public ConformistMapping() : this(TypeManager.Instance) { }

        public ConformistMapping(TypeManager manager)
        {
            var pair = manager.GetInterfaceClassPair(typeof(T));

            proxy = new ClassMappingProxy(pair.Class);
            proxyMapper = proxy.ActLike<IClassMapper<object>>();

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
                    throw new MappingMethodNotFoundException(binder, ClassMappingType, EntityType, processedTypes);
                }

                result = method.Invoke(ClassMapping, processedArgs);
                return true;
            }

            private IEnumerable<ObjectType> ProcessArgumentTypes(IEnumerable<ObjectType> argumentTypes)
            {
                foreach (var argumentType in argumentTypes)
                {
                    yield return ObjectTypeConverter.Convert(ClassMapping, argumentType, EntityType);
                }
            }
        }
    }
}
