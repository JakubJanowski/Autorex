using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Autorex.Binding {
	public class PropertyManager {
		private Shape selectedShape;
		public ValueProperty Width { get; } = new ValueProperty();
		public ValueProperty Height { get; } = new ValueProperty();
		public Segment LineSegment { get; } = new Segment();
		public Offset Center { get; } = new Offset();

		public void Update(Line line) {
			selectedShape = line;
			Width.Visibility = "Collapsed";
			Height.Visibility = "Collapsed";
			Center.Visibility = "Collapsed";
			LineSegment.Visibility = "Visible";
			LineSegment.StartOffset.X.Value = (decimal)line.X1;
			LineSegment.StartOffset.X.PropertyChanged += OnLineSegmentStartXOffsetChanged;
			LineSegment.StartOffset.Y.Value = (decimal)line.Y1;
			LineSegment.StartOffset.Y.PropertyChanged += OnLineSegmentStartYOffsetChanged;
			LineSegment.EndOffset.X.Value = (decimal)line.X2;
			LineSegment.EndOffset.X.PropertyChanged += OnLineSegmentEndXOffsetChanged;
			LineSegment.EndOffset.Y.Value = (decimal)line.Y2;
			LineSegment.EndOffset.Y.PropertyChanged += OnLineSegmentEndYOffsetChanged;
		}

		public void Update(Ellipse ellipse) {
			selectedShape = ellipse;
			Width.Visibility = "Visible";
			Width.Value = (decimal)ellipse.Width;
			Height.Visibility = "Visible";
			Height.Value = (decimal)ellipse.Height;
			Center.Visibility = "Visible";
			Center.X.Value = (decimal)Canvas.GetLeft(ellipse) + Width.Value / 2m;
			Center.Y.Value = (decimal)Canvas.GetTop(ellipse) + Height.Value / 2m;
			LineSegment.Visibility = "Collapsed";
		}

		public void Update(Canvas canvas) {
			Width.Visibility = "Visible";
			Width.Value = 0.12m;
			Height.Visibility = "Visible";
			Height.Value = 0.122m;
			Center.Visibility = "Collapsed";
			LineSegment.Visibility = "Collapsed";
		}

		private void OnLineSegmentStartXOffsetChanged(object sender, PropertyChangedEventArgs e) {
			((Line)selectedShape).X1 = (double)LineSegment.StartOffset.X.Value;
		}
		private void OnLineSegmentStartYOffsetChanged(object sender, PropertyChangedEventArgs e) {
			((Line)selectedShape).Y1 = (double)LineSegment.StartOffset.Y.Value;
		}
		private void OnLineSegmentEndXOffsetChanged(object sender, PropertyChangedEventArgs e) {
			((Line)selectedShape).X2 = (double)LineSegment.EndOffset.X.Value;
		}
		private void OnLineSegmentEndYOffsetChanged(object sender, PropertyChangedEventArgs e) {
			((Line)selectedShape).Y2 = (double)LineSegment.EndOffset.Y.Value;
		}
	}
}
