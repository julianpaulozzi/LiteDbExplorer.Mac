using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LiteDbExplorer.Mac.Models
{
    public class SessionData
    {
        private static readonly Lazy<SessionData> _current = new Lazy<SessionData>(() => new SessionData());

        private SessionData()
        {
        }

        public static SessionData Current => _current.Value;

        public ObservableCollection<DatabaseReference> Databases
        {
            get; private set;
        } = new ObservableCollection<DatabaseReference>();

        public void AddDatabase(DatabaseReference databaseReference)
        {
            Databases.Add(databaseReference);
        }

        public void CloseDatabases()
        {
            foreach (var db in Databases)
            {
                db.Dispose();
            }

            Databases = new ObservableCollection<DatabaseReference>();
        }

        public bool HasDatabaseReference(string path)
        {
            return Databases.FirstOrDefault(a => a.Location == path) != null;
        }
    }
}
