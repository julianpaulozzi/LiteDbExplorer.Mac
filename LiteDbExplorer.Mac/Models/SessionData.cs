using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiteDbExplorer.Mac.Extensions;

namespace LiteDbExplorer.Mac.Models
{
    public class SessionData
    {
        private static readonly Lazy<SessionData> _current = new Lazy<SessionData>(() => new SessionData());

        private CollectionReference _selectedCollection;
        private DatabaseReference _selectedDatabase;

        public event EventHandler<EventArgs<DatabaseReference>> SelectedDatabaseChange;
        public event EventHandler<EventArgs<CollectionReference>> SelectedCollectionChange;

        private SessionData()
        {
            Databases = new ObservableCollection<DatabaseReference>();
        }

        public static SessionData Current => _current.Value;

        public ObservableCollection<DatabaseReference> Databases { get; private set; }

        public DatabaseReference SelectedDatabase
        {
            get => _selectedDatabase;
            private set
            {
                _selectedDatabase = value;
                OnSelectedDatabaseChange();
            }
        }

        public CollectionReference SelectedCollection
        {
            get => _selectedCollection;
            private set
            {
                _selectedCollection = value;
                OnSelectedCollectionChange();
            }
        }

        public void AddDatabase(DatabaseReference databaseReference)
        {
            Databases.Add(databaseReference);
        }

        public void CloseDatabases()
        {
            SelectedCollection = null;
            SelectedDatabase = null;

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

        public void SelectNode(DbNavigationNodeType nodeType, string instanceId)
        {
            switch (nodeType)
            {
                case DbNavigationNodeType.Database:
                {
                    SelectedDatabase = Databases.FirstOrDefault(p => p.InstanceId.Equals(instanceId));
                    break;
                }
                case DbNavigationNodeType.Collection:
                case DbNavigationNodeType.FileCollection:
                {
                    var selectedCollection = Databases
                        .SelectMany(p => p.Collections)
                        .FirstOrDefault(p => p.InstanceId.Equals(instanceId));

                    SelectedDatabase = selectedCollection?.Database;
                    SelectedCollection = selectedCollection;

                    break;
                }
            }
        }

        protected virtual void OnSelectedDatabaseChange()
        {
            SelectedDatabaseChange?.Invoke(this, new EventArgs<DatabaseReference>(_selectedDatabase));
        }

        protected virtual void OnSelectedCollectionChange()
        {
            SelectedCollectionChange?.Invoke(this, new EventArgs<CollectionReference>(_selectedCollection));
        }
    }
}