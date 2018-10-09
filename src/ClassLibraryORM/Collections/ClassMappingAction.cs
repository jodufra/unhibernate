using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ClassLibraryORM.Collections
{
    public delegate void ClassMappingAction(IClassMapper<dynamic> map);
}
