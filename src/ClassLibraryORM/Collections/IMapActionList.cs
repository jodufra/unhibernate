using NHibernate.Mapping.ByCode.Conformist;
using System.Collections;

namespace ClassLibraryORM.Collections
{
    public interface IMapActionList : IList
    {
        void InvokeAll(object classMap);
        void InvokeAll<T>(ClassMapping<T> classMap) where T : class;
    }
}