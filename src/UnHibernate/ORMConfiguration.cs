using UnHibernate.Managers;

namespace UnHibernate
{
    public abstract class ORMConfiguration
    {
        protected abstract void InitializeClassMappings(ClassMappingManager manager);

        protected virtual void InitializeTypePairs(TypeManager manager) { }

        public void Initialize()
        {
            InitializeClassMappings(ClassMappingManager.Instance);
            InitializeTypePairs(TypeManager.Instance);
        }
    }
}
