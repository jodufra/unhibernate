using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using FluentNHibernate.Mapping;
using ImpromptuInterface;

namespace ClassLibraryORM
{
    public class DynamicClassMap<T> : DynamicObject where T : class
    {
        private static Dictionary<int, object> cache = new Dictionary<int, object>();

        public static IReadOnlyDictionary<int, object> Instances => cache;

        public static ClassMap<T> Instance
        {
            get
            {
                var hash = typeof(T).GetHashCode();
                if (!cache.ContainsKey(hash))
                {
                    cache.Add(hash, new DynamicClassMap<T>().ActLike<ClassMap<T>>());
                }

                return cache[hash] as ClassMap<T>;
            }
        }

        private readonly Type type;
        private readonly int hash;

        protected DynamicClassMap()
        {
            type = typeof(T);
            hash = type.GetHashCode();
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.ReturnType == typeof(ClassMap<T>))
            {
                if (cache.ContainsKey(hash))
                {
                    result = cache[hash];
                }
                else
                {
                    result = this.ActLike<ClassMap<T>>();
                }
            }
            else
            {
                result = Activator.CreateInstance(binder.ReturnType);
            }


            return base.TryInvokeMember(binder, args, out result);
        }
    }
}
