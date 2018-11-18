using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Foundation;
using JetBrains.Annotations;
using LiteDB;

namespace LiteDbExplorer.Mac.Models
{
    [Register(nameof(CollectionReferenceVM))]
    public class CollectionReferenceVM : NSObject
    {
        private DatabaseReferenceVM _database;
        private string _name;

        private string _instanceId;
        [Export(nameof(InstanceId))]
        public string InstanceId
        {
            get => _instanceId;
        }

        [Export(nameof(Name))]
        public string Name
        {
            get => _name;
            set
            {
                WillChangeValue(nameof(Name));
                _name = value;
                DidChangeValue(nameof(Name));
            }
        }

        [Export(nameof(Database))]
        public DatabaseReferenceVM Database
        {
            get => _database;
            set
            {
                WillChangeValue(nameof(Database));
                _database = value;
                DidChangeValue(nameof(Database));
            }
        }
    }

    public class CollectionReference : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private ObservableCollection<DocumentReference> _items;
        private DatabaseReference _database;
        private string _name;

        public string InstanceId => Guid.NewGuid().ToString();

        public string Name
        {
            get => _name;
            set
            {
                OnPropertyChanging(nameof(Name));
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DatabaseReference Database
        {
            get => _database;
            set
            {
                OnPropertyChanging(nameof(Database));
                _database = value;
                OnPropertyChanged(nameof(Database));
            }
        }

        public ObservableCollection<DocumentReference> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<DocumentReference>();
                    foreach (var item in LiteCollection.FindAll().Select(a => new DocumentReference(a, this)))
                    {
                        _items.Add(item);
                    }
                }

                return _items;
            }

            set
            {
                OnPropertyChanging(nameof(Items));
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public LiteCollection<BsonDocument> LiteCollection => Database.LiteDatabase.GetCollection(Name);

        public CollectionReference(string name, DatabaseReference database)
        {
            Name = name;
            Database = database;
        }

        public virtual void UpdateItem(DocumentReference document)
        {
            LiteCollection.Update(document.LiteDocument);
        }

        public virtual void RemoveItem(DocumentReference document)
        {
            LiteCollection.Delete(document.LiteDocument["_id"]);
            Items.Remove(document);
        }

        public virtual void RemoveItems(IEnumerable<DocumentReference> documents)
        {
            foreach (var doc in documents)
            {
                RemoveItem(doc);
            }
        }

        public virtual DocumentReference AddItem(BsonDocument document)
        {
            LiteCollection.Insert(document);
            var newDoc = new DocumentReference(document, this);
            Items.Add(newDoc);
            return newDoc;
        }

        public virtual void Refresh()
        {
            OnPropertyChanging(nameof(Items));
            
            if (_items == null)
            {
                _items = new ObservableCollection<DocumentReference>();
            }
            else
            {
                _items.Clear();
            }

            foreach (var item in LiteCollection.FindAll().Select(a => new DocumentReference(a, this)))
            {
                _items.Add(item);
            }

            OnPropertyChanged(nameof(Items));
        }

        public void InvalidateProperties()
        {
            if (Items != null)
            {
                foreach (var documentReference in Items)
                {
                    documentReference.InvalidateProperties();
                }
            }

            OnPropertyChanged(string.Empty);
        }
        
        public event PropertyChangingEventHandler PropertyChanging;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string name)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}