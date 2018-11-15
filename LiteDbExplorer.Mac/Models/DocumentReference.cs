using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LiteDB;

namespace LiteDbExplorer.Mac.Models
{
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
}