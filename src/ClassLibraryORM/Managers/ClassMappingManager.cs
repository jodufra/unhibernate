using ClassLibraryORM.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibraryORM.Managers
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
                    var sb = new StringBuilder();
                    sb.AppendLine($"Type {type.FullName} must not be deep assignable from multiple types.");
                    sb.AppendLine($"Ensure the type can't be assignable from multiple types or ensure all those multiple types are assignable from a single type.");
                    sb.AppendLine($"Deep assignable types:");
                    foreach (var at in assignableTypes)
                    {
                        sb.AppendLine(at.FullName);
                    }

                    throw new Exception(sb.ToString());
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
                var sb = new StringBuilder();
                sb.AppendLine($"Found multiple heir types with the same name.");
                sb.AppendLine($"Repeated types:");
                foreach (var rt in repeatedTypes.OrderBy(q => q.Name))
                {
                    sb.AppendLine(rt.FullName);
                }

                throw new Exception(sb.ToString());
            }

            isTypesTraversed = true;
        }
    }
}
