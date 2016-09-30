using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Autorex {
	public static class Utilities {
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EndOfFile(this BinaryReader binaryReader) {
			return binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddGrid(this Canvas canvas) {
			canvas.AddGrid(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
		}

		public static void AddGrid(this Canvas canvas, double virtualHeight, double virtualWidth) {
			Line line;
			for (int x = 50; x < virtualWidth; x += 100) {
				line = Utilities.CreateLine(x, 0, x, virtualHeight, 1, "#FFA0A0A0");
				line.StrokeDashArray = new DoubleCollection(new double[] { 7, 3 });
				line.StrokeDashOffset = 3;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int y = 50; y < virtualHeight; y += 100) {
				line = Utilities.CreateLine(0, y, virtualWidth, y, 1, "#FFA0A0A0");
				line.StrokeDashArray = new DoubleCollection(new double[] { 7, 3 });
				line.StrokeDashOffset = 3;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int x = 100; x < virtualWidth; x += 100) {
				line = Utilities.CreateLine(x, 0, x, virtualHeight, 1, "#FF404040");
				line.StrokeDashArray = new DoubleCollection(new double[] { 13, 12 });
				line.StrokeDashOffset = 6;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
			for (int y = 100; y < virtualHeight; y += 100) {
				line = Utilities.CreateLine(0, y, virtualWidth, y, 1, "#FF404040");
				line.StrokeDashArray = new DoubleCollection(new double[] { 13, 12 });
				line.StrokeDashOffset = 6;
				line.Tag = "grid";
				canvas.Children.Add(line);
			}
		}

		public static void ClearGrid(this UIElementCollection collection) {
			for (int index = collection.Count - 1; index >= 0; index--)
				if ((collection[index] as Shape).Tag as string == "grid")
					collection.RemoveAt(index);
		}

		public static void ClearShapes(this UIElementCollection collection) {
			for (int index = collection.Count - 1; index >= 0; index--)
				if ((collection[index] as Shape).Tag == null)
					collection.RemoveAt(index);
		}

		[Obsolete("used when canvasGrid is a sibling xml element of canvas.")]
		public static void UpdateGrid_obsolete(this Canvas canvasGrid, Canvas canvas, Grid canvasContainer) {
			double marginLeft;
			double marginRight;
			double marginTop;
			double marginBottom;
			if (canvas.Margin.Left >= -99) {
				marginLeft = canvas.Margin.Left;
				marginRight = canvasContainer.ActualWidth - marginLeft - canvasGrid.ActualWidth;
			}
			//else if (canvas.Margin.Right >= -99) {
			//	//marginLeft = canvasGrid.ActualWidth + canvas.Margin.Left - canvas.ActualWidth;
			//	marginRight = canvas.Margin.Right;
			//	marginLeft = canvasContainer.ActualWidth - marginRight - canvasGrid.ActualWidth;
			//}
			else {
				marginLeft = Utilities.Mod(canvas.Margin.Left, 100) - 100;
				marginRight = canvasContainer.ActualWidth - marginLeft - canvasGrid.ActualWidth;
			}

			if (canvas.Margin.Top >= -99) {
				marginTop = canvas.Margin.Top;
				marginBottom = canvasContainer.ActualHeight - marginTop - canvasGrid.ActualHeight;
			}
			//else if (canvas.Margin.Bottom >= -99) {
			//	marginBottom = canvas.Margin.Bottom;
			//	marginTop = canvasContainer.ActualHeight - marginBottom - canvasGrid.ActualHeight;
			//}
			else {
				marginTop = Utilities.Mod(canvas.Margin.Top, 100) - 100;
				marginBottom = canvasContainer.ActualHeight - marginTop - canvasGrid.ActualHeight;
			}
			System.Diagnostics.Debug.WriteLine("canv margins: " + canvas.Margin.Left + " " + canvas.Margin.Top + " " + canvas.Margin.Right + " " + canvas.Margin.Bottom);
			System.Diagnostics.Debug.WriteLine("grid margins: " + marginLeft + " " + marginTop + " " + marginRight + " " + marginBottom);

			canvasGrid.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
			//canvasGrid.Margin = new Thickness(marginLeft, marginTop, canvasContainer.ActualWidth - canvasGrid.ActualWidth - marginLeft, canvasContainer.ActualHeight - canvasGrid.ActualHeight - marginTop);
		}

		// canvasGrid is child of canvas
		public static void UpdateGrid(this Canvas canvasGrid, Canvas canvas) {
			double marginLeft;
			double marginTop;

			if (canvas.Margin.Left >= -99)
				marginLeft = 0;
			else
				marginLeft = Math.Ceiling(canvas.Margin.Left / 100) * -100;

			if (canvas.Margin.Top >= -99)
				marginTop = 0;
			else
				marginTop = Math.Ceiling(canvas.Margin.Top / 100) * -100;

			double marginRight = canvas.Width - canvasGrid.Width - marginLeft;
			double marginBottom = canvas.Height - canvasGrid.Height - marginTop;

			canvasGrid.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
		}

		public static void InitialiseGraduationScale(Canvas sideScale, Canvas bottomScale, Thickness draftCanvasMargin) {
			for (double x = 0; x < bottomScale.ActualWidth + 99; x += 10) {
				double LineHeight = (1 + Convert.ToInt32(x % 100 == 0) * 2 + Convert.ToInt32(x % 50 == 0)) * 3;
				Line line = Utilities.CreateLine(x, 0, x, LineHeight, 1, "#FF000000");
				bottomScale.Children.Add(line);
			}
			for (double y = 0; y < sideScale.ActualHeight + 99; y += 10) {
				double LineWidth = (1 + Convert.ToInt32(y % 100 == 0) * 2 + Convert.ToInt32(y % 50 == 0)) * 3;
				Line line = Utilities.CreateLine(15 - LineWidth, y, 15, y, 1, "#FF000000");
				sideScale.Children.Add(line);
			}
			UpdateGraduationScale(sideScale, bottomScale, draftCanvasMargin);
		}

		private static double prevBottomScaleLabelShift = ~0;
		private static double prevSideScaleLabelShift = ~0;
		public static void UpdateGraduationScale(Canvas sideScale, Canvas bottomScale, Thickness draftCanvasMargin) {
			bottomScale.Margin = new Thickness(Utilities.Mod(draftCanvasMargin.Left, 100) - 100, bottomScale.Margin.Top, bottomScale.Margin.Right, bottomScale.Margin.Bottom);
			sideScale.Margin = new Thickness(sideScale.Margin.Left, Utilities.Mod(draftCanvasMargin.Top, 100) - 100, sideScale.Margin.Right, sideScale.Margin.Bottom);
			double scaleBottomLabelShift = Math.Floor(draftCanvasMargin.Left / 100);
			if (scaleBottomLabelShift != prevBottomScaleLabelShift) {
				bottomScale.Children.OfType<TextBlock>().ToList().ForEach(c => bottomScale.Children.Remove(c));
				for (double x = 0; x < bottomScale.ActualWidth + 99; x += 100) {
					TextBlock textBlock = CreateTextBlock(x + 2, 0, (x - (scaleBottomLabelShift + 1) * 100).ToString());
					bottomScale.Children.Add(textBlock);
				}
				prevBottomScaleLabelShift = scaleBottomLabelShift;
			}
			double scaleSideLabelShift = Math.Floor(draftCanvasMargin.Top / 100);
			if (scaleSideLabelShift != prevSideScaleLabelShift) {
				sideScale.Children.OfType<TextBlock>().ToList().ForEach(c => sideScale.Children.Remove(c));
				for (double y = 0; y < sideScale.ActualHeight + 99; y += 100) {
					TextBlock textBlock = CreateTextBlock(-2, y + 2, (y - (scaleSideLabelShift + 1) * 100).ToString());
					textBlock.LayoutTransform = new RotateTransform(90);
					sideScale.Children.Add(textBlock);
				}
				prevSideScaleLabelShift = scaleSideLabelShift;
			}
		}

		private static TextBlock CreateTextBlock(double MarginX, double MarginY, string text) {
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			Canvas.SetLeft(textBlock, MarginX);
			Canvas.SetTop(textBlock, MarginY);
			return textBlock;
		}

		/// <summary>
		/// Get margins that a new draft with given sizes should have 
		/// </summary>
		/// <param name="horizontalDifference">Difference between canvas container width and canvas width</param>
		/// <param name="verticalDifference">Difference between canvas container height and canvas height</param>
		/// <returns>Margins for new draft</returns>
		public static Thickness GetInitialMargin(double horizontalDifference, double verticalDifference) {
			double leftMargin, topMargin, rightMargin, bottomMargin;

			if (horizontalDifference > 10) {
				leftMargin = horizontalDifference / 2;
				rightMargin = leftMargin;
			}
			else {
				leftMargin = 5;
				rightMargin = horizontalDifference - 5;
			}

			if (verticalDifference > 10) {
				topMargin = verticalDifference / 2;
				bottomMargin = topMargin;
			}
			else {
				topMargin = 5;
				bottomMargin = verticalDifference - 5;
			}

			return new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
		}

		public static Thickness CalculateMargin(Thickness prevViewMargin, Size NewSize, Size PreviousSize, Grid canvasContainer, Canvas canvas) {
			// have canvasContainer.ActualWidth - canvas.ActualWidth as tmp value
			if (prevViewMargin.Left < 5) {
				if (prevViewMargin.Right < 5)
					prevViewMargin.Left = Math.Min(5, canvasContainer.ActualWidth - Math.Min(5, canvasContainer.ActualWidth - prevViewMargin.Left - canvas.ActualWidth) - canvas.ActualWidth);
				else if (NewSize.Width > PreviousSize.Width)
					prevViewMargin.Left = Math.Min(5, canvasContainer.ActualWidth - prevViewMargin.Right - canvas.ActualWidth);
			}
			prevViewMargin.Right = canvasContainer.ActualWidth - prevViewMargin.Left - canvas.ActualWidth;

			if (prevViewMargin.Top < 5) {
				if (prevViewMargin.Bottom < 5)
					prevViewMargin.Top = Math.Min(5, canvasContainer.ActualHeight - Math.Min(5, canvasContainer.ActualHeight - prevViewMargin.Top - canvas.ActualHeight) - canvas.ActualHeight);
				else if (NewSize.Height > PreviousSize.Height)
					prevViewMargin.Top = Math.Min(5, canvasContainer.ActualHeight - prevViewMargin.Bottom - canvas.ActualHeight);
			}
			prevViewMargin.Bottom = canvasContainer.ActualHeight - prevViewMargin.Top - canvas.ActualHeight;

			if (prevViewMargin.Left > canvasContainer.ActualWidth / 2) {
				prevViewMargin.Left = canvasContainer.ActualWidth / 2;
				prevViewMargin.Right = -canvas.ActualWidth + canvasContainer.ActualWidth / 2;
			}
			else if (prevViewMargin.Right > canvasContainer.ActualWidth / 2) {
				prevViewMargin.Left = -canvas.ActualWidth + canvasContainer.ActualWidth / 2;
				prevViewMargin.Right = canvasContainer.ActualWidth / 2;
			}
			if (prevViewMargin.Top > canvasContainer.ActualHeight / 2) {
				prevViewMargin.Top = canvasContainer.ActualHeight / 2;
				prevViewMargin.Bottom = -canvas.ActualHeight + canvasContainer.ActualHeight / 2;
			}
			else if (prevViewMargin.Bottom > canvasContainer.ActualHeight / 2) {
				prevViewMargin.Top = -canvas.ActualHeight + canvasContainer.ActualHeight / 2;
				prevViewMargin.Bottom = canvasContainer.ActualHeight / 2;
			}

			return new Thickness(prevViewMargin.Left, prevViewMargin.Top, prevViewMargin.Right, prevViewMargin.Bottom);
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
			} while (Math.Abs(previous - current) > epsilon);   // loops 3 times in worst case
			return current;
		}

		public static double Mod(double a, double b) {
			return a - b * Math.Floor(a / b);
		}
	}
}
