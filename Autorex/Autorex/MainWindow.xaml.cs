using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Autorex.Binding;

namespace Autorex {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
        // initially use
        DrawingTool tool = DrawingTool.Select;
        bool draw = false;
        Point? prevPoint;
		UIElement temporaryElement;
		Shape outline;
		Shape selectedShape;
		public PropertyManager PropertyManager { get; set; } = new PropertyManager();
		public string Test { get; set; } = "xdxd";
		public MainWindow() {
			InitializeComponent();
			PropertyManager.Update(canvas);//
			propertiesPanel.DataContext = PropertyManager;
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
		private void selectBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Select;
		}
		#endregion
		private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (tool != DrawingTool.Select) {
				draw = true;
				prevPoint = e.GetPosition(canvas);
				// add a dot if no mousemove event happened before mouseup
				canvas.CaptureMouse();
			}
			else
				PropertyManager.Update(canvas);
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
						temporaryElement.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
						temporaryElement.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
					}
					((Ellipse)temporaryElement).Stroke = System.Windows.Media.Brushes.Blue;
					((Ellipse)temporaryElement).Width = Math.Abs(prevPoint.Value.X - mousePosition.X);
					((Ellipse)temporaryElement).Height = Math.Abs(prevPoint.Value.Y - mousePosition.Y);
					Canvas.SetLeft(temporaryElement, Math.Min(prevPoint.Value.X, mousePosition.X));
					Canvas.SetTop(temporaryElement, Math.Min(prevPoint.Value.Y, mousePosition.Y));
					((Ellipse)temporaryElement).StrokeThickness = 2;
					canvas.Children.Add(temporaryElement);
					PropertyManager.Update((Ellipse)temporaryElement);
					break;
				case DrawingTool.Line:
					canvas.Children.Remove(temporaryElement);
					if (temporaryElement == null) {
						temporaryElement = new Line();
						temporaryElement.MouseEnter += ShapeMouseEnter;
						temporaryElement.MouseLeave += ShapeMouseLeave;
						temporaryElement.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
						temporaryElement.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
					}
					Line temporaryLine = (Line)temporaryElement;
					temporaryLine.Stroke = System.Windows.Media.Brushes.Blue;
					temporaryLine.X1 = prevPoint.Value.X;
					temporaryLine.Y1 = prevPoint.Value.Y;
					temporaryLine.X2 = mousePosition.X;
					temporaryLine.Y2 = mousePosition.Y;
					temporaryLine.StrokeThickness = 2;
					temporaryLine.StrokeStartLineCap = PenLineCap.Round;
					temporaryLine.StrokeEndLineCap = PenLineCap.Round;
					canvas.Children.Add(temporaryElement);
					PropertyManager.Update(temporaryLine);
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
			if (selectedShape != null || tool != DrawingTool.Select)
				return;
			dynamic dynamicSender = sender;
			outline = Extensions.Outline(dynamicSender);
			outline.Stroke = System.Windows.Media.Brushes.White;
			outline.MouseLeave += OutlineMouseLeave;
			outline.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
			outline.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
			outline.StrokeStartLineCap = PenLineCap.Round;
			outline.StrokeEndLineCap = PenLineCap.Round;
			Canvas.SetZIndex((UIElement)sender, 1);
			canvas.Children.Add(outline);
			selectedShape = (Shape)sender;
		}
		private void ShapeMouseLeave(object sender, RoutedEventArgs e) {
			if (tool == DrawingTool.Select && !outline.IsMouseOver) {
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

		private void ShapeMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			dynamic dynamicSender = sender;
			dynamicSender.CaptureMouse();
			PropertyManager.Update(dynamicSender);
			e.Handled = true;
		}

		private void ShapeMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			((Shape)sender).ReleaseMouseCapture();
			e.Handled = true;
		}
	}
}
