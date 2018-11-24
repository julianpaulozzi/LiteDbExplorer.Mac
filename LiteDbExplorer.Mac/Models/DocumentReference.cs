using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foundation;
using JetBrains.Annotations;
using LiteDB;

namespace LiteDbExplorer.Mac.Models
{
    [Register(nameof(DocumentReferenceVM))]
    public class DocumentReferenceVM : NSObject
    {
        private BsonDocumentVM _liteDocument;
        private CollectionReferenceVM _collection;

        private string _instanceId;
        [Export(nameof(InstanceId))]
        public string InstanceId
        {
            get => _instanceId;
        }

        [Export(nameof(LiteDocument))]
        public BsonDocumentVM LiteDocument
        {
            get => _liteDocument;
            set
            {
                WillChangeValue(nameof(LiteDocument));
                _liteDocument = value;
                DidChangeValue(nameof(LiteDocument));
            }
        }

        [Export(nameof(Collection))]
        public CollectionReferenceVM Collection
        {
            get => _collection;
            set
            {
                WillChangeValue(nameof(Collection));
                _collection = value;
                DidChangeValue(nameof(Collection));
            }
        }
    }

    [Register(nameof(BsonDocumentVM))]
    public class BsonDocumentVM : NSObject
    {

    }

    public class DocumentReference : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private BsonDocument _liteDocument;
        private CollectionReference _collection;

        public DocumentReference()
        {
            InstanceId = Guid.NewGuid().ToString();
        }

        public DocumentReference(BsonDocument document, CollectionReference collection) : this()
        {
            LiteDocument = document;
            Collection = collection;
        }

        public string InstanceId { get; }

        public BsonDocument LiteDocument
        {
            get => _liteDocument;
            set
            {
                OnPropertyChanging(nameof(LiteDocument));
                _liteDocument = value;
                OnPropertyChanged(nameof(LiteDocument));
            }
        }

        public CollectionReference Collection
        {
            get => _collection;
            set
            {
                OnPropertyChanging(nameof(Collection));
                _collection = value;
                OnPropertyChanged(nameof(Collection));
            }
        }

        public void InvalidateProperties()
        {
            OnPropertyChanged(nameof(LiteDocument));
            OnPropertyChanged(nameof(Collection));
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