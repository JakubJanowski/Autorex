using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Autorex.Binding {
	public class Property: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		internal void ClearHandlers() {
			if (PropertyChanged != null)
				foreach (PropertyChangedEventHandler d in PropertyChanged.GetInvocationList())
					PropertyChanged -= d;
		}

		protected void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		internal void AddHandler(PropertyChangedEventHandler d) {
			if (PropertyChanged != null && Array.FindIndex(PropertyChanged.GetInvocationList(), x => x.Equals(d)) < 0)
				PropertyChanged += d;
		}
	}
}
