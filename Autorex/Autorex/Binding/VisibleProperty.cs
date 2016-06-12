using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class VisibleProperty: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected string visibility = "Collapsed";
		public virtual string TextValue {
			get { return visibility; }
			set {
				if (value != visibility && (value == "Visible" || value == "Collapsed")) {
					visibility = value;
					OnPropertyChanged("TextValue");
				}
			}
		}

		protected void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
