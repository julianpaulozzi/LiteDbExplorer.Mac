using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AppKit;
using Foundation;
using LiteDB;

namespace LiteDbExplorer.Mac.Models
{
    [Register(nameof(DatabaseReference))]
    public class DatabaseReferenceVM : NSObject
    {
        private string _name;
        private string _location;
        private NSMutableArray _children = new NSMutableArray();

        [Export(nameof(InstanceId))]
        public string InstanceId { get; set; }

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

        [Export(nameof(Location))]
        public string Location
        {
            get => _location;
            set
            {
                WillChangeValue(nameof(Location));
                _location = value;
                DidChangeValue(nameof(Location));
            }
        }

        [Export(nameof(Collections))]
        public NSArray Collections
        {
            get => _children;
        }

    }

    public enum DbNavigationNodeType
    {
        Database,
        Collection,
        FileCollection
    }

    [Register(nameof(DbNavigationNode))]
    public class DbNavigationNode : NSObject
    {
        private string _name;
        private DbNavigationNodeType _nodeType;
        private NSMutableArray _children = new NSMutableArray();

        [Export(nameof(InstanceId))]
        public string InstanceId { get; set; }

        [Export(nameof(NodeType))]
        public DbNavigationNodeType NodeType
        {
            get => _nodeType;
            set
            {
                WillChangeValue(nameof(IsLeaf));
                WillChangeValue(nameof(Icon));
                _nodeType = value;
                DidChangeValue(nameof(IsLeaf));
                DidChangeValue(nameof(Icon));
            }
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

        [Export(nameof(IsLeaf))]
        public bool IsLeaf => NodeType != DbNavigationNodeType.Database;

        [Export(nameof(Icon))]
        public NSImage Icon
        {
            get 
            {
                switch (NodeType)
                {
                    case DbNavigationNodeType.Database:
                        return NSImage.ImageNamed ("database.png");
                    case DbNavigationNodeType.Collection:
                        return NSImage.ImageNamed ("table.png");
                    case DbNavigationNodeType.FileCollection:
                        return NSImage.ImageNamed ("file-table.png");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Export(nameof(Children))]
        public NSArray Children => _children;

        [Export(nameof(ChildrenCount))]
        public nint ChildrenCount
        {
            get { return (nint)_children.Count; }
        }

        [Export("addObject:")]
        public void AddChildren(DbNavigationNode node)
        {
            WillChangeValue(nameof(Children));
            _children.Add(node);
            DidChangeValue(nameof(Children));
        }
    }

    public class DatabaseReference : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
    {
        private string _name;
        private string _location;
        private ObservableCollection<CollectionReference> _collections;

        public string InstanceId => Guid.NewGuid().ToString();

        public LiteDatabase LiteDatabase
        {
            get;
        }

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

        public string Location
        {
            get => _location;
            set
            {
                OnPropertyChanging(nameof(Location));
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        [Export(nameof(Collections))]
        public ObservableCollection<CollectionReference> Collections
        {
            get => _collections;
            set
            {
                OnPropertyChanging(nameof(Collections));
                _collections = value;
                OnPropertyChanged(nameof(Collections));
            }
        }

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
                .Where(a => a != @"_chunks")
                .OrderBy(a => a)
                .Select(
                    a => a == @"_files" ? new FileCollectionReference(a, this) : new CollectionReference(a, this))
                );
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

        public event PropertyChangingEventHandler PropertyChanging;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string name)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}