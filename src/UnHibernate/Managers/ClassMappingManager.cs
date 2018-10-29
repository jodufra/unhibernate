using UnHibernate.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnHibernate.Exceptions;

namespace UnHibernate.Managers
{
    public class ClassMappingManager
    {
        public static ClassMappingManager Instance { get; } = new ClassMappingManager(TypeManager.Instance);

        private readonly Dictionary<Type, ClassMappingActions> typeActions;
        private readonly TypeManager typeManager;
        private TypesCorrelation correlatedTypes;
        private HashSet<Type> heirTypes;
        private List<Type> classMappings;
        private bool isTypesTraversed;

        private ClassMappingManager(TypeManager typeManager)
        {
            this.typeManager = typeManager;
            typeActions = new Dictionary<Type, ClassMappingActions>();
        }

        public IEnumerable<Type> HeirTypes
        {
            get
            {
                TraverseTypes();
                return heirTypes;
            }
        }

        public TypesCorrelation CorrelatedTypes
        {
            get
            {
                TraverseTypes();
                return correlatedTypes;
            }
        }

        public IEnumerable<Type> ClassMappings
        {
            get
            {
                TraverseTypes();
                return classMappings;
            }
        }

        public void Map<TInterface, TClass>() where TClass : class, TInterface
        {
            typeManager.Add<TInterface, TClass>();

            var type = typeof(TInterface);

            if (!typeActions.ContainsKey(type))
            {
                typeActions.Add(type, new ClassMappingActions());
                isTypesTraversed = false;
            }
        }

        public void Map<TInterface, TClass>(ClassMappingAction action) where TClass : class, TInterface
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Map<TInterface, TClass>();

            typeActions[typeof(TInterface)].Add(action);
        }

        public IEnumerable<ClassMappingAction> GetActionsForType<T>() => GetActionsForType(typeof(T));

        public IEnumerable<ClassMappingAction> GetActionsForType(Type type)
        {
            TraverseTypes();

            var result = new List<ClassMappingAction>();

            if (typeActions.ContainsKey(type))
            {
                result.AddRange(typeActions[type]);
            }

            foreach (var t in correlatedTypes.GetDeepCorrelations(type))
            {
                if (typeActions.ContainsKey(t))
                {
                    result.AddRange(typeActions[t]);
                }
            }

            return result;
        }

        private void TraverseTypes()
        {
            if (isTypesTraversed)
            {
                return;
            }

            var types = typeActions.Keys.OrderBy(q => q.Name);

            heirTypes = new HashSet<Type>();
            correlatedTypes = new TypesCorrelation();
            classMappings = new List<Type>();

            foreach (var type in types)
            {
                var assignableTypes = type.GetDeepAssignableTypes(types);

                if (assignableTypes.Count() > 1)
                {
                    throw new TypeAmbiguousAssignmentException(type, assignableTypes);
                }

                var heir = assignableTypes.Any() ? assignableTypes.First() : type;

                heirTypes.Add(heir);
                correlatedTypes.Add(type, heir);
                classMappings.Add(typeof(ConformistMapping<>).MakeGenericType(heir));
            }

            var repeatedTypes = new HashSet<Type>();
            foreach (var type in types)
            {
                var sameNameTypes = heirTypes.Where(q => q != type && q.Name == type.Name);
                if (sameNameTypes.Any())
                {
                    repeatedTypes.Add(type);
                    foreach (var snt in sameNameTypes)
                    {
                        repeatedTypes.Add(snt);
                    }
                }
            }

            if (repeatedTypes.Any())
            {
                throw new AmbiguousTypesFoundException(repeatedTypes);
            }

            isTypesTraversed = true;
        }
    }
}
