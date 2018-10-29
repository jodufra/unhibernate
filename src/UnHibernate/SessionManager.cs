using UnHibernate.Managers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.Mapping.ByCode;
using System.Collections;

namespace UnHibernate
{
    public static class SessionManager
    {
        public static string SessionKey { get; set; } = "_NHibernateSession";
        public static string ConnectionStringName { get; set; } = "Connection";

        private static ISessionFactory _sessionFactory;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = GetConfiguration().BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public static ISession Session
        {
            get
            {
                var dictionary = GetDictionary();
                if (dictionary != null)
                {
                    if (dictionary[SessionKey] is ISession s)
                    {
                        return s;
                    }

                    var session = SessionFactory.OpenSession();
                    dictionary[SessionKey] = session;
                    return session;
                }

                return SessionFactory.OpenSession();
            }
        }

        private static IDictionary GetDictionary() => System.Web.HttpContext.Current?.Items;

        private static Configuration GetConfiguration()
        {
            var serializer = new HbmSerializer { Validate = true };

            var cfg = new Configuration();
            cfg.SetInterceptor(new GenericInterceptor(TypeManager.Instance));
            cfg.DataBaseIntegration(db =>
            {
                db.ConnectionStringName = ConnectionStringName;
                db.Dialect<NHibernate.Dialect.MySQL55Dialect>();
                db.Driver<NHibernate.Driver.MySqlDataDriver>();
                db.HqlToSqlSubstitutions = "true 1, false 0, yes 'Y', no 'N'";
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.SchemaAction = SchemaAutoAction.Validate;
            });

            var mapper = new ModelMapper();
            mapper.AddMappings(ClassMappingManager.Instance.ClassMappings);
            var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddDeserializedMapping(mappings, null);

            return cfg;
        }
    }
}
