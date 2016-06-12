using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class Segment: Property {
		public Offset StartOffset { get; set; } = new Offset();
		public Offset EndOffset { get; set; } = new Offset();
		public string LengthString {
			get {
				return Extensions.Sqrt((StartOffset.X.Value - EndOffset.X.Value) * (StartOffset.X.Value - EndOffset.X.Value) + 
					(StartOffset.Y.Value - EndOffset.Y.Value) * (StartOffset.Y.Value - EndOffset.Y.Value)).ToString("#.###") + " mm";
			}
		}
		public override string Visibility {
			get {
				return visibility;
			}
			set {
				if (value != visibility && (value == "Visible" || value == "Collapsed")) {
					visibility = value;
					StartOffset.Visibility = value;
					EndOffset.Visibility = value;
					OnPropertyChanged("Visibility");
				}
			}
		}
		public Segment() {
			StartOffset.X.updateProperties.Add(Tuple.Create((object)this, "LengthString"));
			StartOffset.Y.updateProperties.Add(Tuple.Create((object)this, "LengthString"));
			EndOffset.X.updateProperties.Add(Tuple.Create((object)this, "LengthString"));
			EndOffset.Y.updateProperties.Add(Tuple.Create((object)this, "LengthString"));
		}
	}
}
