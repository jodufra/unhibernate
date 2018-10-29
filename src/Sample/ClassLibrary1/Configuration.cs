using UnHibernate;
using UnHibernate.Managers;
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
                map.Id(x => ((IUser)x).Id, x => { x.Column("Id"); x.Generator(Generators.Identity); });
                map.Property(x => ((IUser)x).Name, x => { x.Column("Name"); x.NotNullable(true); x.Length(50); });
                map.Property(x => ((IUser)x).DateCreated, x => { x.Column("DateCreated"); x.NotNullable(true); });
                map.Property(x => ((IUser)x).DateUpdated, x => { x.Column("DateUpdated"); x.NotNullable(false);  });
            });
        }
    }
}
