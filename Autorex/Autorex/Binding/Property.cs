using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Autorex.Binding {
	public class Property: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public List<Tuple<object, string>> updateProperties = new List<Tuple<object, string>>();
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
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
				foreach(var property in updateProperties)
					handler(property.Item1, new PropertyChangedEventArgs(property.Item2));
			}
		}
	}
}
