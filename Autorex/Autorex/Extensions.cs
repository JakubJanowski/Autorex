using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Autorex {
	public static class Extensions {
		public static T NewInstance<T>(this T s) where T : Shape, new() {
			return new T();
		}

		public static Line CreateLine(double X1, double Y1, double X2, double Y2, double thickness, string colorHex) {
			Line line = new Line();
			line.Stroke = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(colorHex);
			line.StrokeThickness = thickness;
			line.X1 = X1;
			line.Y1 = Y1;
			line.X2 = X2;
			line.Y2 = Y2;
			return line;
		}

		public static Line Outline(this Line line) {
			Line copy = new Line();
			copy.X1 = line.X1;
			copy.X2 = line.X2;
			copy.Y1 = line.Y1;
			copy.Y2 = line.Y2;
			copy.StrokeThickness = line.StrokeThickness + 4;
			return copy;
		}

		public static Ellipse Outline(this Ellipse ellipse) {
			Ellipse copy = new Ellipse();
			copy.Height = ellipse.Height + 4;
			copy.Width = ellipse.Width + 4;
			Canvas.SetLeft(copy, Canvas.GetLeft(ellipse) - 2);
			Canvas.SetTop(copy, Canvas.GetTop(ellipse) - 2);
			copy.StrokeThickness = ellipse.StrokeThickness + 4;
			return copy;
		}
		
		public static bool EndOfFile(this BinaryReader binaryReader) {
			return binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
		}

		public static void ClearShapes(this UIElementCollection collection) {
			for (int index = collection.Count - 1; index >= 0; index--)
				if ((collection[index] as Shape).Tag == null)
					collection.RemoveAt(index);
		}

		public static void ClearGrid(this UIElementCollection collection) {
			for (int index = collection.Count - 1; index >= 0; index--)
				if ((collection[index] as Shape).Tag as string == "grid")
					collection.RemoveAt(index);
		}
		public static void AddGrid(this Canvas canvas, Size? size = null) {
			if (size == null)
				size = new Size(canvas.ActualWidth, canvas.ActualHeight);
			Line line;
			for (int x = 50; x < size.Value.Width; x += 100) {
				line = Extensions.CreateLine(x, 0, x, size.Value.Height, 0.5, "#FF808080");
				line.StrokeDashArray = new DoubleCollection(new double[] { 10 });
				line.StrokeDashOffset = 5;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int y = 50; y < size.Value.Height; y += 100) {
				line = Extensions.CreateLine(0, y, size.Value.Width, y, 0.5, "#FF808080");
				line.StrokeDashArray = new DoubleCollection(new double[] { 10 });
				line.StrokeDashOffset = 5;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int x = 100; x < size.Value.Width; x += 100) {
				line = Extensions.CreateLine(x, 0, x, size.Value.Height, 1, "#FF404040");
				line.StrokeDashArray = new DoubleCollection(new double[] { 10 });
				line.StrokeDashOffset = 5;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int y = 100; y < size.Value.Height; y += 100) {
				line = Extensions.CreateLine(0, y, size.Value.Width, y, 1, "#FF404040");
				line.StrokeDashArray = new DoubleCollection(new double[] { 10 });
				line.StrokeDashOffset = 5;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			// Border
			line = Extensions.CreateLine(0, 0, size.Value.Width, 0, 2, "#FF000000");
			line.Tag = "grid";
			canvas.Children.Add(line);
			line = Extensions.CreateLine(0, 0, 0, size.Value.Height, 2, "#FF000000");
			line.Tag = "grid";
			canvas.Children.Add(line);
			line = Extensions.CreateLine(0, size.Value.Height, size.Value.Width, size.Value.Height, 2, "#FF000000");
			line.Tag = "grid";
			canvas.Children.Add(line);
			line = Extensions.CreateLine(size.Value.Width, 0, size.Value.Width, size.Value.Height, 2, "#FF000000");
			line.Tag = "grid";
			canvas.Children.Add(line);
		}
		
		[Obsolete("please use AddGrid with precedent call to ClearGrid instead.")]
		public static void UpdateGrid(this Canvas canvas, double startX, double startY) {
			for (double x = startX - 50 + ((100 - (startX - 50) % 100) % 100); x < canvas.ActualWidth; x += 100) {
				Line line = Extensions.CreateLine(x, 0, x, canvas.ActualHeight, 0.5, "#FF808080");
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (double y = startY - 50 + ((100 - (startY - 50) % 100) % 100); y < canvas.ActualHeight; y += 100) {
				Line line = Extensions.CreateLine(0, y, canvas.ActualWidth, y, 0.5, "#FF808080");
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (double x = startX + ((100 - startX % 100) % 100); x < canvas.ActualWidth; x += 100) {
				Line line = Extensions.CreateLine(x, 0, x, canvas.ActualHeight, 1, "#FF404040");
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (double y = startY + ((100 - startY % 100) % 100); y < canvas.ActualHeight; y += 100) {
				Line line = Extensions.CreateLine(0, y, canvas.ActualWidth, y, 1, "#FF404040");
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
		}

		public static decimal Sqrt(decimal x, decimal epsilon = 0.0m) {
			if (x < 0m) throw new OverflowException("Cannot calculate square root from a negative number");
			if (epsilon < 0m) throw new ArgumentException("Cannot calculate square root with negative accuracy");

			decimal current = (decimal)Math.Sqrt((double)x);
			decimal previous;
			do {
				previous = current;
				if (previous == 0.0m) return 0m;
				current = (previous + x / previous) / 2m;
			} while (Math.Abs(previous - current) > epsilon);	// loops 3 times in worst case
			return current;
		}
	}
}
