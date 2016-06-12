using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Autorex.Binding {
	public class Property: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected string visibility = "Collapsed";
		public virtual string Visibility {
			get { return visibility; }
			set {
				if (value != visibility && (value == "Visible" || value == "Collapsed")) {
					visibility = value;
					OnPropertyChanged("Visibility");
				}
			}
		}

		protected void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
