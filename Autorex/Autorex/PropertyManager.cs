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
		private static Dictionary<string, decimal> ratios;
		static Width() {
			ratios = new Dictionary<string, decimal>();
			ratios["um"] = 0.001m;
			ratios["mm"] = 1m;
			ratios["cm"] = 10m;
			ratios["dm"] = 100m;
			ratios["m"] =  1000m;
			ratios["in"] = 25.4m;
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
					try {
						this.value = result * ratios[value.Substring(index + 1)];
					} catch {}
				}
			}
		}
	}
}
