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
				try {
					return Utilities.Sqrt((StartOffset.X.Value - EndOffset.X.Value) * (StartOffset.X.Value - EndOffset.X.Value) +
						(StartOffset.Y.Value - EndOffset.Y.Value) * (StartOffset.Y.Value - EndOffset.Y.Value)).ToString("#.###") + " mm";
				} catch (ArgumentOutOfRangeException) {
					return "Inf";
				} catch (Exception) {
					return "NaN";
				}
			}
		}

		public VisibleProperty Visibility { get; } = new VisibleProperty();
		public void SetVisibility(string value) {
			if (value != Visibility.TextValue && (value == "Visible" || value == "Collapsed")) {
				Visibility.TextValue = value;
				StartOffset.SetVisibility(value);
				EndOffset.SetVisibility(value);
			}
		}

		public Segment() {
			StartOffset.X.PropertyChanged += OnPropertyChanged;
			StartOffset.Y.PropertyChanged += OnPropertyChanged;
			EndOffset.X.PropertyChanged += OnPropertyChanged;
			EndOffset.Y.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			base.OnPropertyChanged("LengthString");
		}
	}
}
