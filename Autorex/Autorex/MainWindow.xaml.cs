using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Autorex {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
        // initially use
        DrawingTool tool = DrawingTool.Move;
        bool draw = false;
        Point? prevPoint;
		UIElement temporaryElement;
		Shape outline;
		Shape selectedShape;
		public PropertyManager CurrentProperties { get; set; } = new PropertyManager();
		public string Test { get; set; } = "xdxd";
		public MainWindow() {
			InitializeComponent();
			tmp.DataContext = CurrentProperties;
		}

		/////////////////////////
		// Main menu functions
		#region menu_functions
		private void Open(object sender, RoutedEventArgs e) {
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			
			// Set filter for file extension and default file extension 
			//dialog.DefaultExt = ".atrx";
			dialog.Filter = "Autorex Files (*.atrx)|*.atrx";
			
			if (dialog.ShowDialog() == true) {
				string filename = dialog.FileName;
			}
		}

		private void Save(object sender, RoutedEventArgs e) {
			
		}

		private void SaveAs(object sender, RoutedEventArgs e) {
			
		}

		private void Exit(object sender, RoutedEventArgs e) {
			Close();
		}
		#endregion

		#region buttons
		private void penBtn_Click(object sender, RoutedEventArgs e) {
            tool = DrawingTool.Pen;
        }
		private void lineBtn_Click(object sender, RoutedEventArgs e) {
            tool = DrawingTool.Line;
        }
		private void circleBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Circle;
		}
		private void curveBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Curve;
		}
		private void ellipseBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Ellipse;
		}
		#endregion
		private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            draw = true;
            prevPoint = e.GetPosition(canvas);
			// add a dot if no mousemove event happened before mouseup
			canvas.CaptureMouse();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
			//Debug.WriteLine("mouseMove " + e.GetPosition(canvas).ToString());
			if (!draw)
				return;

			Point mousePosition = e.GetPosition(canvas);
			switch (tool) {
				case DrawingTool.Ellipse:
					canvas.Children.Remove(temporaryElement);
					if (temporaryElement == null) {
						temporaryElement = new Ellipse();
						temporaryElement.MouseEnter += ShapeMouseEnter;
						temporaryElement.MouseLeave += ShapeMouseLeave;
					}
					((Ellipse)temporaryElement).Stroke = System.Windows.Media.Brushes.Blue;
					((Ellipse)temporaryElement).Width = Math.Abs(prevPoint.Value.X - mousePosition.X);
					((Ellipse)temporaryElement).Height = Math.Abs(prevPoint.Value.Y - mousePosition.Y);
					Canvas.SetLeft(temporaryElement, Math.Min(prevPoint.Value.X, mousePosition.X));
					Canvas.SetTop(temporaryElement, Math.Min(prevPoint.Value.Y, mousePosition.Y));
					((Ellipse)temporaryElement).StrokeThickness = 2;
					canvas.Children.Add(temporaryElement);
					break;
				case DrawingTool.Line:
					canvas.Children.Remove(temporaryElement);
					if (temporaryElement == null) {
						temporaryElement = new Line();
						temporaryElement.MouseEnter += ShapeMouseEnter;
						temporaryElement.MouseLeave += ShapeMouseLeave;
					}
					((Line)temporaryElement).Stroke = System.Windows.Media.Brushes.Blue;
					((Line)temporaryElement).X1 = prevPoint.Value.X;
					((Line)temporaryElement).Y1 = prevPoint.Value.Y;
					((Line)temporaryElement).X2 = mousePosition.X;
					((Line)temporaryElement).Y2 = mousePosition.Y;
					((Line)temporaryElement).StrokeThickness = 2;
					((Line)temporaryElement).StrokeStartLineCap = PenLineCap.Round;
					((Line)temporaryElement).StrokeEndLineCap = PenLineCap.Round;
					canvas.Children.Add(temporaryElement);
					break;
				case DrawingTool.Pen:
					Line myLine = new Line();
					myLine.Stroke = System.Windows.Media.Brushes.Blue;
					myLine.X1 = prevPoint.Value.X;
					myLine.Y1 = prevPoint.Value.Y;
					myLine.X2 = mousePosition.X;
					myLine.Y2 = mousePosition.Y;
					canvas.Children.Add(myLine);
					prevPoint = mousePosition;
					break;
			}
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			draw = false;
			temporaryElement = null;
			canvas.ReleaseMouseCapture();
        }

		private void ShapeMouseEnter(object sender, RoutedEventArgs e) {
			if (selectedShape != null)
				return;
			dynamic dynamicSender = sender;
			outline = Extensions.Outline(dynamicSender);
			outline.Stroke = System.Windows.Media.Brushes.White;
			outline.MouseLeave += OutlineMouseLeave;
			outline.StrokeStartLineCap = PenLineCap.Round;
			outline.StrokeEndLineCap = PenLineCap.Round;
			Canvas.SetZIndex((UIElement)sender, 1);
			canvas.Children.Add(outline);
			selectedShape = (Shape)sender;
		}
		private void ShapeMouseLeave(object sender, RoutedEventArgs e) {
			if (!outline.IsMouseOver) {
				canvas.Children.Remove(outline);
				Canvas.SetZIndex(selectedShape, 0);
				selectedShape = null;
			}
		}
		private void OutlineMouseLeave(object sender, RoutedEventArgs e) {
			if (!selectedShape.IsMouseOver) {
				canvas.Children.Remove(outline);
				Canvas.SetZIndex(selectedShape, 0);
				selectedShape = null;
			}
		}
	}
}
