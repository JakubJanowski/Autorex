using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class Segment: Property {
		public Offset StartOffset { get; set; } = new Offset();
		public Offset EndOffset { get; set; } = new Offset();
		public string LengthString {
			get {
				return Extensions.Sqrt((StartOffset.X.Value - EndOffset.X.Value) * (StartOffset.X.Value - EndOffset.X.Value)
					+ (StartOffset.Y.Value - EndOffset.Y.Value) * (StartOffset.Y.Value - EndOffset.Y.Value)) + " mm";
			}
		}
		public override string Visibility { 
			set {
				StartOffset.Visibility = value;
				EndOffset.Visibility = value;
			}
		}
		/*public Segment() {
			StartOffset.X.updateProperties.Add("LengthString");
			StartOffset.Y.updateProperties.Add("LengthString");
			EndOffset.X.updateProperties.Add("LengthString");
			EndOffset.Y.updateProperties.Add("LengthString");
		}*/
	}
}
