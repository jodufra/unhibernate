using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;

namespace ClassLibraryORM.Collections
{
    internal class MapActionList<T> : List<Action<object>>, IMapActionList
    {
        public void InvokeAll<TEntity>(ClassMapping<TEntity> classMap) where TEntity : class
        {
            if (classMap == null)
            {
                throw new ArgumentNullException(nameof(classMap));
            }

            if (typeof(TEntity).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException($"The generic of {classMap.GetType().FullName}<{typeof(TEntity).FullName}> must be assignable from {typeof(T).FullName}.");
            }

            InvokeAll(classMap);
        }

        public void InvokeAll(object classMap)
        {
            if (classMap == null)
            {
                throw new ArgumentNullException(nameof(classMap));
            }

            foreach (var item in this)
            {
                item?.Invoke(classMap);
            }
        }
    }
}
