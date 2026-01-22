

namespace MedORM.ORM
{
    public class ChangeTracker
    {
        private readonly List<object> _added = new();
        private readonly List<object> _modified = new();
        private readonly List<object> _deleted = new();

        public void TrackAdded(object entity) => _added.Add(entity);
        public void TrackModified(object entity) => _modified.Add(entity);
        public void TrackDeleted(object entity) => _deleted.Add(entity);

        public IEnumerable<object> GetAdded() => _added;
        public IEnumerable<object> GetModified() => _modified;
        public IEnumerable<object> GetDeleted() => _deleted;

        public void Clear()
        {
            _added.Clear();
            _modified.Clear();
            _deleted.Clear();
        }
    }
}
