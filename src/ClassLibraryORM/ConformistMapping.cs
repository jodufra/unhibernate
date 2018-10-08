using ClassLibraryORM.Managers;
using ImpromptuInterface;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode.Impl;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ClassLibraryORM
{
    public class ConformistMapping<T> : IConformistHoldersProvider
    {
        private readonly ClassMappingProxy proxy;

        public ConformistMapping() : this(TypeManager.Instance) { }

        public ConformistMapping(TypeManager manager)
        {
            var pair = manager.GetInterfaceClassPair(typeof(T));
            proxy = new ClassMappingProxy(pair.Class);

            var interfaces = new Type[] {
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

            foreach (var action in ClassMappingManager.Instance.GetActionsForType<T>())
            {
                action?.Invoke(proxy.ActLike<IClassMapper<object>>(interfaces));
            }
        }

        public ICustomizersHolder CustomizersHolder => (proxy.ClassMapping as IConformistHoldersProvider).CustomizersHolder;

        public IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder => (proxy.ClassMapping as IConformistHoldersProvider).ExplicitDeclarationsHolder;

        private class ClassMappingProxy : DynamicObject
        {
            public ClassMappingProxy(Type classType)
            {
                ClassMappingType = typeof(ClassMapping<>).MakeGenericType(classType);
                ClassMapping = Activator.CreateInstance(ClassMappingType);
            }

            public object ClassMapping { get; private set; }
            public Type ClassMappingType { get; private set; }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = ClassMappingType.GetMethod(binder.Name, args.Select(q => q.GetType()).ToArray()).Invoke(ClassMapping, args);
                return true;
            }
        }
    }
}
