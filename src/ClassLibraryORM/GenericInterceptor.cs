using System;
using System.Runtime.Serialization;
using ClassLibraryORM.Managers;
using NHibernate;
using NHibernate.Type;

namespace ClassLibraryORM
{
    public class GenericInterceptor : EmptyInterceptor
    {
        private readonly TypeManager typeHandler;

        public GenericInterceptor(TypeManager typeHandler)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
        }

        public override object Instantiate(string clazz, object id)
        {
            if (typeHandler.TryGetInterfaceClassPair(clazz, out var handler))
            {
                var instance = FormatterServices.GetUninitializedObject(handler.Class);

                SessionManager.SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id);

                return instance;
            }

            return base.Instantiate(clazz, id);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if ("DateCreated".Equals(propertyNames[i]))
                {
                    state[i] = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    return true;
                }
            }

            return false;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if ("DateUpdated".Equals(propertyNames[i]))
                {
                    currentState[i] = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    return true;
                }
            }

            return false;
        }
    }
}
