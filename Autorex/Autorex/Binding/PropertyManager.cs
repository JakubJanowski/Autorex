using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Autorex.Binding {
	// Decimal values are not currently supported
	public class PropertyManager {
		private Shape selectedShape;
		public ValueProperty Width { get; } = new ValueProperty();
		public ValueProperty Height { get; } = new ValueProperty();
		public Segment LineSegment { get; } = new Segment();
		public Offset Center { get; } = new Offset();


		public PropertyManager() {
			LineSegment.StartOffset.X.PropertyChanged += OnLineSegmentStartXOffsetChanged;
			LineSegment.StartOffset.Y.PropertyChanged += OnLineSegmentStartYOffsetChanged;
			LineSegment.EndOffset.X.PropertyChanged += OnLineSegmentEndXOffsetChanged;
			LineSegment.EndOffset.Y.PropertyChanged += OnLineSegmentEndYOffsetChanged;
			Center.X.PropertyChanged += OnCenterXOffsetChanged;
			Center.Y.PropertyChanged += OnCenterYOffsetChanged;
			Width.PropertyChanged += OnWidthChanged;
			Height.PropertyChanged += OnHeightChanged;
		}

		public void Select(Line line) {
			Debug.Write("\nSelect line");
			selectedShape = line;
			Width.Visibility.TextValue = "Collapsed";
			Height.Visibility.TextValue = "Collapsed";
			Center.SetVisibility("Collapsed");
			LineSegment.SetVisibility("Visible");
		}

		public void Update(Line line) {
			Debug.Write("\nUpdate line");
			LineSegment.StartOffset.X.Value = (decimal)line.X1;
			LineSegment.StartOffset.Y.Value = (decimal)line.Y1;
			LineSegment.EndOffset.X.Value = (decimal)line.X2;
			LineSegment.EndOffset.Y.Value = (decimal)line.Y2;
		}

		public void Select(Ellipse ellipse) {
			Debug.Write("\nSelect ellipse");
			selectedShape = ellipse;
			Width.Visibility.TextValue = "Visible";
			Height.Visibility.TextValue = "Visible";
			Center.SetVisibility("Visible");
			LineSegment.SetVisibility("Collapsed");
		}

		public void Update(Ellipse ellipse) {
			Debug.Write("\nUpdate ellipse");
			Width.Value = (decimal)ellipse.Width;
			Height.Value = (decimal)ellipse.Height;
			Center.X.Value = (decimal)Canvas.GetLeft(ellipse) + Width.Value / 2m;
			Center.Y.Value = (decimal)Canvas.GetTop(ellipse) + Height.Value / 2m;
		}

		public void Select(Canvas canvas) {
			Debug.Write("\nSelect canvas");
			selectedShape = null;
			Width.Visibility.TextValue = "Visible";
			Height.Visibility.TextValue = "Visible";
			Center.SetVisibility("Collapsed");
			LineSegment.SetVisibility("Collapsed");
		}

		public void Update(Canvas canvas) {
			Debug.Write("\nUpdate canvas");
			//Width.Value = 950m;
			//Height.Value = 950m;
		}

		private void OnLineSegmentStartXOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape is Line)
				((Line)selectedShape).X1 = (double)LineSegment.StartOffset.X.Value;
		}
		private void OnLineSegmentStartYOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape is Line)
				((Line)selectedShape).Y1 = (double)LineSegment.StartOffset.Y.Value;
		}
		private void OnLineSegmentEndXOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape is Line)
				((Line)selectedShape).X2 = (double)LineSegment.EndOffset.X.Value;
		}
		private void OnLineSegmentEndYOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape is Line)
				((Line)selectedShape).Y2 = (double)LineSegment.EndOffset.Y.Value;
		}
		private void OnWidthChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape != null)
				selectedShape.Width = (double)Width.Value;
		}
		private void OnHeightChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape != null)
				selectedShape.Height = (double)Height.Value;
		}
		private void OnCenterXOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape != null)
				Canvas.SetLeft(selectedShape, (double)(Center.X.Value - Width.Value / 2m));
		}
		private void OnCenterYOffsetChanged(object sender, PropertyChangedEventArgs e) {
			if (selectedShape != null)
				Canvas.SetTop(selectedShape, (double)(Center.Y.Value - Height.Value / 2m));
		}
	}
}
