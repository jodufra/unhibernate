using ClassLibraryORM;
using ClassLibraryORM.Managers;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ClassLibrary1
{
    public class Configuration : ORMConfiguration
    {
        protected override void InitializeClassMappings(ClassMappingManager manager)
        {
            manager.Map<IUser, User>(map =>
            {
                map.Table("user");
                map.Id(x => (x as IUser).Id, x => { x.Column("Id"); x.Generator(Generators.Identity); });
                map.Property(x => (x as IUser).Name, x => { x.Column("Name"); x.NotNullable(true); x.Length(50); });
                map.Property(x => (x as IUser).DateCreated, x => { x.Column("DateCreated"); x.NotNullable(true); });
                map.Property(x => (x as IUser).DateUpdated, x => { x.Column("DateUpdated"); x.NotNullable(false);  });
            });
        }
    }
}
