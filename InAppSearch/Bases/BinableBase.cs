using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InAppSearch.Bases
{
    public class BinableBase : INotifyPropertyChanged
    {
        protected void SetValue<T>(ref T oldVa, T newVa, [CallerMemberName]string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(oldVa, newVa))
            {
                oldVa = newVa;
                NotifyChange(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChange([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
