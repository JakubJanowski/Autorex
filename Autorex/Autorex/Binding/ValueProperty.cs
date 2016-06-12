using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class ValueProperty: Property {
		protected static Dictionary<string, decimal> ratios;
		static ValueProperty() {
			ratios = new Dictionary<string, decimal>();
			ratios["um"] = 0.001m;
			ratios["mm"] = 1m;
			ratios["cm"] = 10m;
			ratios["dm"] = 100m;
			ratios["m"] = 1000m;
			ratios["in"] = 25.4m;
		}

		protected decimal value;
		public decimal Value {
			get { return value; ; }
			set {
				this.value = value;
				valueString = value + " mm";
				OnPropertyChanged("Value");
				OnPropertyChanged("ValueString");
			}
		}
		protected string valueString;
		public virtual string ValueString {
			get { return valueString; }
			set {
				valueString = value;
				ParseValue(value, ref this.value);
				OnPropertyChanged("ValueString");
			}
		}

		public VisibleProperty Visibility { get; } = new VisibleProperty();

		public void Refresh() {
			valueString = value + " mm";
			OnPropertyChanged("ValueString");
		}
		
		/// <summary>
		/// Parse given string to exact value in milimeters e.g. "95 in"
		/// </summary>
		/// <param name="text">string to parse</param>
		/// <param name="value">value in milimeters as reference</param>
		protected static void ParseValue(string text, ref decimal value) {
			decimal result;
			int startIndex;
			int endIndex;
			for (startIndex = 0; startIndex < text.Length && text[startIndex] > '9' && text[startIndex] < '0'; startIndex++)
				;
			for (endIndex = startIndex + 1; endIndex < text.Length && ((text[endIndex] <= '9' && text[endIndex] >= '0') ||
				text[endIndex] == ',' || text[endIndex] == '.' || text[endIndex] == ' '); endIndex++)
				;

			if (!decimal.TryParse(text.Substring(startIndex, endIndex - startIndex), out result))
				return;

			try {
				value = result * ratios[new Regex("[^a-z]").Replace(text.Substring(endIndex).ToLower(), "")];
			} catch { }
		}
	}
}
