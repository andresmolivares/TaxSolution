using System.ComponentModel;

namespace TaxSolution.Desktop
{
    public abstract class Notifiable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void NotifyChanges(params string[] propertyNames)
        {
            if (null != propertyNames || propertyNames.Length > 0)
                for (var i = 0; i < propertyNames.Length; i++)
                    NotifyChange(propertyNames[i]); // Notify for each property
        }
    }
}
