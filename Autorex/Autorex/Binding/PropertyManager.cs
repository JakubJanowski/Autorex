using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Autorex.Binding {
	// Decimal values are not currently supported
	public class PropertyManager {
		private FrameworkElement selectedShape;
		public ValueProperty Width { get; } = new ValueProperty();
		public ValueProperty Height { get; } = new ValueProperty();
		public Segment LineSegment { get; } = new Segment();
		public Offset Center { get; } = new Offset();
		public TextProperty ShapeName { get; } = new TextProperty();

		public void Select(Line line) {
			selectedShape = line;
			ShapeName.Text = "Line";
			Width.Visibility.TextValue = "Collapsed";
			Height.Visibility.TextValue = "Collapsed";
			Center.SetVisibility("Collapsed");
			LineSegment.SetVisibility("Visible");
		}

		public void Update(Line line) {
			LineSegment.StartOffset.X.Value = (decimal)line.X1;
			LineSegment.StartOffset.Y.Value = (decimal)line.Y1;
			LineSegment.EndOffset.X.Value = (decimal)line.X2;
			LineSegment.EndOffset.Y.Value = (decimal)line.Y2;
		}

		public void Select(Ellipse ellipse) {
			selectedShape = ellipse;
			ShapeName.Text = "Ellipse";
			Width.Visibility.TextValue = "Visible";
			Height.Visibility.TextValue = "Visible";
			Center.SetVisibility("Visible");
			LineSegment.SetVisibility("Collapsed");
		}

		public void Update(Ellipse ellipse) {
			Width.Value = (decimal)ellipse.Width;
			Height.Value = (decimal)ellipse.Height;
			Center.X.Value = (decimal)Canvas.GetLeft(ellipse) + Width.Value / 2m;
			Center.Y.Value = (decimal)Canvas.GetTop(ellipse) + Height.Value / 2m;
		}

		public void Select(Canvas canvas) {
			selectedShape = canvas;
			Width.Visibility.TextValue = "Visible";
			Height.Visibility.TextValue = "Visible";
			Center.SetVisibility("Collapsed");
			LineSegment.SetVisibility("Collapsed");
		}

		public void Update(Canvas canvas) {
			ShapeName.Text = "Draft";
			Width.Value = (decimal)canvas.ActualWidth;
			Height.Value = (decimal)canvas.ActualHeight;
			// not implemented yet
		}

		public void UserOperation(string propertyPath) {
			if (selectedShape == null) return;
			string[] properties = propertyPath.Split('.');
			switch(properties[0]) {
				case "Width":
					Canvas.SetLeft(selectedShape, (double)(Center.X.Value - Width.Value / 2m));
					selectedShape.Width = (double)Width.Value;
					break;
				case "Height":
					Canvas.SetTop(selectedShape, (double)(Center.Y.Value - Height.Value / 2m));
					selectedShape.Height = (double)Height.Value;
					break;
				case "LineSegment":
					switch (properties[1]) {
						case "StartOffset":
							switch (properties[2]) {
								case "X":
									((Line)selectedShape).X1 = (double)LineSegment.StartOffset.X.Value;
									break;
								case "Y":
									((Line)selectedShape).Y1 = (double)LineSegment.StartOffset.Y.Value;
									break;
							}
							break;
						case "EndOffset":
							switch (properties[2]) {
								case "X":
									((Line)selectedShape).X2 = (double)LineSegment.EndOffset.X.Value;
									break;
								case "Y":
									((Line)selectedShape).Y2 = (double)LineSegment.EndOffset.Y.Value;
									break;
							}
							break;
					}
					break;
				case "Center":
					switch (properties[1]) {
						case "X":
							Canvas.SetLeft(selectedShape, (double)(Center.X.Value - Width.Value / 2m));
							break;
						case "Y":
							Canvas.SetTop(selectedShape, (double)(Center.Y.Value - Height.Value / 2m));
							break;
					}
					break;
			}
		}
	}
}
