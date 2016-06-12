using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class TextProperty: Property {
		protected string text;
		public string Text {
			get { return text; }
			internal set {
				text = value;
				OnPropertyChanged("Text");
			}
		}
	}
}
