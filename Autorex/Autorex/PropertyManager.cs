using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex {
	public class PropertyManager: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public Width Width { get; } = new Width();
	}

	public class Width: INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		private static HashSet<double> ratios;
		static {
			ratios = new HashSet<double>;
			ratios.Add("um", 0.001);
			ratios.Add("mm", 1);
			ratios.Add("cm", 10);
			ratios.Add("dm", 100);
			ratios.Add("m", 1000);
			ratios.Add("in", 25.4);//dokladniej
		}
		private string visibility = "Visible";
		public string Visibility {
			get { return visibility; }
			set {
				if (value != visibility && (value == "Visible" || value == "Collapsed")) {
					visibility = value;
					//OnPropertyChanged("Visibility");
				}
			}
		}
		private decimal value = 95m;
		public string Value {
			get { return value + " mm"; }
			set {
				int result;
				int index = value.IndexOf(' ');
				if (int.TryParse(value.Substring(0, index), out result)) {
					double ratio = ratios[value.Substring(index + 1)];
					if (ratios != null)
						this.value = result * ratio;
				}
			}
		}
	}
}
