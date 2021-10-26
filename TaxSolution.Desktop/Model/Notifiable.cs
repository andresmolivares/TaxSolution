using System.ComponentModel;

namespace TaxSolution.Desktop.Model
{
    public abstract class Notifiable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyChange(string propertyName)
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void NotifyChanges(params string[] propertyNames)
        {
            if (propertyNames is not null || propertyNames?.Length > 0)
                for (var i = 0; i < propertyNames.Length; i++)
                    NotifyChange(propertyNames[i]); // Notify for each property
        }
    }
}
