using System;
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
