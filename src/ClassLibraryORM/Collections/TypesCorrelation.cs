using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibraryORM.Collections
{
    public sealed class TypesCorrelation
    {
        private readonly Dictionary<Type, HashSet<Type>> correlations;

        public TypesCorrelation()
        {
            correlations = new Dictionary<Type, HashSet<Type>>();
        }

        public void Add(Type typeA, Type typeB)
        {
            if (typeA == null)
            {
                throw new ArgumentNullException(nameof(typeA));
            }

            if (typeB == null)
            {
                throw new ArgumentNullException(nameof(typeB));
            }

            if (!correlations.ContainsKey(typeA))
            {
                correlations.Add(typeA, new HashSet<Type>());
            }

            if (!correlations.ContainsKey(typeB))
            {
                correlations.Add(typeB, new HashSet<Type>());
            }

            correlations[typeA].Add(typeB);
            correlations[typeB].Add(typeA);
        }

        public IEnumerable<Type> GetShallowCorrelations(Type type) => new List<Type>(correlations[type]);

        public IEnumerable<Type> GetDeepCorrelations(Type type)
        {
            var relations = new HashSet<Type> { type };

            GetRecurssiveCorrelations(type, ref relations);

            relations.Remove(type);

            return relations;
        }

        private void GetRecurssiveCorrelations(Type type, ref HashSet<Type> relations)
        {
            if (!correlations.ContainsKey(type))
            {
                return;
            }

            var values = correlations[type];
            foreach (var item in values)
            {
                if (!relations.Contains(item))
                {
                    relations.Add(item);
                    GetRecurssiveCorrelations(item, ref relations);
                }
            }
        }

        public IReadOnlyDictionary<Type, HashSet<Type>> AsReadOnlyDictionary() => correlations.ToDictionary(q => q.Key, q => new HashSet<Type>(q.Value));
    }
}
