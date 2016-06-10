using System.Windows.Controls;
using System.Windows.Shapes;

namespace Autorex {
	public static class Extensions {
		public static T NewInstance<T>(this T s) where T : Shape, new() {
			return new T();
		}
		public static Line Outline(this Line line) {
			Line copy = new Line();
			copy.X1 = line.X1;
			copy.X2 = line.X2;
			copy.Y1 = line.Y1;
			copy.Y2 = line.Y2;
			copy.StrokeThickness = 6;
			return copy;
		}
		public static Ellipse Outline(this Ellipse ellipse) {
			Ellipse copy = new Ellipse();
			copy.Height = ellipse.Height + 4;
			copy.Width = ellipse.Width + 4;
			Canvas.SetLeft(copy, Canvas.GetLeft(ellipse) - 2);
			Canvas.SetTop(copy, Canvas.GetTop(ellipse) - 2);
			copy.StrokeThickness = 6;
			return copy;
		}
	}
}
