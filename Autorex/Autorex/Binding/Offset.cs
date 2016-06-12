using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class Offset: Property {
		public ValueProperty X { get; } = new ValueProperty();
		public ValueProperty Y { get; } = new ValueProperty();
		public VisibleProperty Visibility { get; } = new VisibleProperty();
		public void SetVisibility(string value) {
			if (value != Visibility.TextValue && (value == "Visible" || value == "Collapsed")) {
				Visibility.TextValue = value;
				X.Visibility.TextValue = value;
				Y.Visibility.TextValue = value;
			}
		}
	}
}
