using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LiteDB;

namespace LiteDbExplorer.Mac.Models
{
    public class DatabaseReference : INotifyPropertyChanged, IDisposable
    {
        public LiteDatabase LiteDatabase
        {
            get;
        }

        public string Name { get; set; }

        public string Location { get; set; }

        public ObservableCollection<CollectionReference> Collections { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public DatabaseReference(string path, string password)
        {            
            Location = path;
            Name = Path.GetFileName(path);

            LiteDatabase = string.IsNullOrEmpty(password) ? 
                new LiteDatabase(path) : 
                new LiteDatabase($"Filename={path};Password={password}");

            UpdateCollections();
        }

        public void Dispose()
        {
            LiteDatabase.Dispose();
        }

        private void UpdateCollections()
        {
            Collections = new ObservableCollection<CollectionReference>(LiteDatabase.GetCollectionNames()
                .Where(a => a != @"_chunks").OrderBy(a => a).Select(a =>
                {
                    if (a == @"_files")
                    {
                        return new FileCollectionReference(a, this);
                    }

                    return new CollectionReference(a, this);
                }));

            OnPropertyChanged(nameof(Collections));
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public DocumentReference AddFile(string id, string path)
        {
            LiteDatabase.FileStorage.Upload(id, path);
            UpdateCollections();
            var collection = Collections.First(a => a is FileCollectionReference);
            return collection.Items.First(a => a.LiteDocument["_id"] == id);
        }

        public void AddCollection(string name)
        {
            if (LiteDatabase.GetCollectionNames().Contains(name))
            {
                throw new Exception($"Cannot add collection \"{name}\", collection with that name already exists.");
            }

            var coll = LiteDatabase.GetCollection(name);
            var newDoc = new BsonDocument
            {
                ["_id"] = ObjectId.NewObjectId()
            };

            coll.Insert(newDoc);
            coll.Delete(newDoc["_id"]);
            UpdateCollections();
        }

        public void RenameCollection(string oldName, string newName)
        {
            LiteDatabase.RenameCollection(oldName, newName);
            UpdateCollections();
        }

        public void DropCollection(string name)
        {
            LiteDatabase.DropCollection(name);
            UpdateCollections();
        }

        public static bool IsDbPasswordProtected(string path)
        {
            using (var db = new LiteDatabase(path))
            {
                try
                {
                    db.GetCollectionNames();
                    return false;
                }
                catch (LiteException e)
                {
                    if (e.Message.Contains("password"))
                    {
                        return true;
                    }

                    throw;
                }
            }
        }

        public void Refresh()
        {
            UpdateCollections();
        }
        
    }
    
    
    public class DocumentReference : INotifyPropertyChanged
    {
        public DocumentReference()
        {
        }

        public DocumentReference(BsonDocument document, CollectionReference collection)
        {
            LiteDocument = document;
            Collection = collection;
        }

        public BsonDocument LiteDocument { get; set; }

        public CollectionReference Collection { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvalidateProperties()
        {
            OnPropertyChanged(string.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class CollectionReference : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public DatabaseReference Database { get; set; }
        
        private ObservableCollection<DocumentReference> _items;
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
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public LiteCollection<BsonDocument> LiteCollection => Database.LiteDatabase.GetCollection(Name);

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
    }
}