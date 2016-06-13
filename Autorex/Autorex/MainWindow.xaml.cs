using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Autorex.Binding;
using System.Diagnostics;
using System.IO;

namespace Autorex {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
        DrawingTool tool = DrawingTool.Line;
        bool draw = false;
		bool draftSaved = true;
		string filename;
        Point? prevPoint;
		UIElement temporaryElement;
		Shape outline;
		Shape selectedShape;
		Size canvasWorkareaSize = new Size(1000, 1000);
		Thickness prevViewMargin;
		public PropertyManager PropertyManager { get; set; } = new PropertyManager();

		public MainWindow() {
			InitializeComponent();
			propertiesPanel.DataContext = PropertyManager;
			canvas.Width = canvasWorkareaSize.Width;
			canvas.Height = canvasWorkareaSize.Height;
		}

		/////////////////////////
		// Main menu functions
		#region menu_functions
		private void New(object sender, RoutedEventArgs e) {
			if (!draftSaved) {
				MessageBoxResult result = MessageBox.Show("Unsaved changes will be lost. Do you want to save this draft?", "Warning", MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Yes) {
					Save(null, null);
					return;
				}
				if (result != MessageBoxResult.No)
					return;
			}
			canvas.Children.ClearShapes();
			draftSaved = true;
			canvas.Width = 1000;
			canvas.Height = 1000;
			canvas.Margin = new Thickness(0, 0, 0, 0);
			PropertyManager.Select(canvas);
			PropertyManager.Update(canvas);
		}

		private void Open(object sender, RoutedEventArgs e) {
			if (!draftSaved) {
				MessageBoxResult result = MessageBox.Show("Unsaved changes will be lost. Do you want to save this draft?", "Warning", MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Yes) {
					Save(null, null);
					return;
				}
				if (result != MessageBoxResult.No)
					return;
			}

			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Filter = "Autorex Files (*.atrx)|*.atrx|All files (*.*)|*.*";

			if (dialog.ShowDialog() == true)
				filename = dialog.FileName;
			else return;

			if (filename == null) {
				MessageBox.Show("Could not open specified file.", "Error");
				return;
			}

			BinaryReader binaryReader;
			try {
				binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
			} catch (IOException) {
				MessageBox.Show("Could not open file.", "Error");
				return;
			}

			canvas.Children.ClearShapes();
			try {
				canvas.Width = binaryReader.ReadDouble();
				canvas.Height = binaryReader.ReadDouble();
				while (!binaryReader.EndOfFile()) {
					switch((DrawingTool)binaryReader.ReadByte()) {
						case DrawingTool.Line:
							Line line = new Line();

							line.MouseEnter += ShapeMouseEnter;
							line.MouseLeave += ShapeMouseLeave;
							line.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
							line.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
							line.Stroke = System.Windows.Media.Brushes.Blue;
							line.StrokeThickness = 2;
							line.StrokeStartLineCap = PenLineCap.Round;
							line.StrokeEndLineCap = PenLineCap.Round;

							line.X1 = binaryReader.ReadDouble();
							line.Y1 = binaryReader.ReadDouble();
							line.X2 = binaryReader.ReadDouble();
							line.Y2 = binaryReader.ReadDouble();
							canvas.Children.Add(line);
							break;
						case DrawingTool.Ellipse:
							Ellipse ellipse = new Ellipse();

							ellipse.MouseEnter += ShapeMouseEnter;
							ellipse.MouseLeave += ShapeMouseLeave;
							ellipse.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
							ellipse.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
							ellipse.Stroke = System.Windows.Media.Brushes.Blue;
							ellipse.StrokeThickness = 2;

							Canvas.SetLeft(ellipse, binaryReader.ReadDouble());
							Canvas.SetTop(ellipse, binaryReader.ReadDouble());
							ellipse.Width = binaryReader.ReadDouble();
							ellipse.Height = binaryReader.ReadDouble();
							canvas.Children.Add(ellipse);
							break;
					}
				}
				draftSaved = true;
				PropertyManager.Select(canvas);
				PropertyManager.Update(canvas);
			} catch (IOException) {
				MessageBox.Show("An error occured while reading the file. It might be corrupted.", "Error");
			} finally {
				binaryReader.Close();
			}
		}

		private void Save(object sender, RoutedEventArgs e) {
			if (filename == null) {
				SaveAs(sender, e);
				return;
			}

			BinaryWriter binaryWriter;
			try {
				binaryWriter = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
			} catch (IOException) {
				MessageBox.Show("Could not create file.", "Error");
				return;
			}

			try {
				binaryWriter.Write(canvas.ActualWidth);
				binaryWriter.Write(canvas.ActualHeight);
				foreach (Shape shape in canvas.Children) {
					if (shape.Tag != null)
						continue;
					if (shape is Line) {
						binaryWriter.Write((byte)DrawingTool.Line);
						binaryWriter.Write((shape as Line).X1);
						binaryWriter.Write((shape as Line).Y1);
						binaryWriter.Write((shape as Line).X2);
						binaryWriter.Write((shape as Line).Y2);
					}
					else if (shape is Ellipse) {
						binaryWriter.Write((byte)DrawingTool.Ellipse);
						binaryWriter.Write(Canvas.GetLeft(shape as Ellipse));
						binaryWriter.Write(Canvas.GetTop(shape as Ellipse));
						binaryWriter.Write((shape as Ellipse).Width);
						binaryWriter.Write((shape as Ellipse).Height);
					}
				}
				draftSaved = true;
			} catch (IOException) {
				MessageBox.Show("An error occured while writing to file.", "Error");
			} finally {
				binaryWriter.Close();
			}
		}

		private void SaveAs(object sender, RoutedEventArgs e) {
			Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
			dialog.Filter = "Autorex Files (*.atrx)|*.atrx|All files (*.*)|*.*";

			if (dialog.ShowDialog() == true)
				filename = dialog.FileName;
			else return;

			if (filename != null)
				Save(sender, e);
		}

		private void Exit(object sender, RoutedEventArgs e) {
			if (!draftSaved) {
				MessageBoxResult result = MessageBox.Show("Unsaved changes will be lost. Do you want to save this draft?", "Warning", MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Yes)
					Save(null, null);
				if (result != MessageBoxResult.No) {
					return;
				}
			}
			draftSaved = true;
			Close();
		}
		#endregion

		/////////////////////////
		// Button actions
		#region buttons
		private void penBtn_Click(object sender, RoutedEventArgs e) {
            tool = DrawingTool.Pen;
			canvas.Cursor = Cursors.Pen;
		}
		private void lineBtn_Click(object sender, RoutedEventArgs e) {
            tool = DrawingTool.Line;
			canvas.Cursor = Cursors.Cross;
		}
		private void circleBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Circle;
		}
		private void curveBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Curve;
		}
		private void ellipseBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Ellipse;
			canvas.Cursor = Cursors.Cross;
		}
		private void selectBtn_Click(object sender, RoutedEventArgs e) {
			tool = DrawingTool.Select;
			canvas.Cursor = Cursors.SizeAll;
		}
		#endregion

		/////////////////////////
		// Mouse events handling
		#region mouse_events
		private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			draw = true;
			canvas.CaptureMouse();
			if (tool == DrawingTool.Select) {
				PropertyManager.Select(canvas);
				PropertyManager.Update(canvas);
				prevPoint = e.GetPosition(canvasContainer);
				prevViewMargin = canvas.Margin;
			}
			else
				prevPoint = e.GetPosition(canvas);
		}

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
			if (!draw)
				return;

			Point mousePosition;
			switch (tool) {
				case DrawingTool.Ellipse:
					mousePosition = e.GetPosition(canvas);
					canvas.Children.Remove(temporaryElement);
					if (temporaryElement == null) {
						draftSaved = false;
						temporaryElement = new Ellipse();
						temporaryElement.MouseEnter += ShapeMouseEnter;
						temporaryElement.MouseLeave += ShapeMouseLeave;
						temporaryElement.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
						temporaryElement.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
						((Ellipse)temporaryElement).StrokeThickness = 2;
						PropertyManager.Select((Ellipse)temporaryElement);
					}
					((Ellipse)temporaryElement).Stroke = System.Windows.Media.Brushes.Blue;
					((Ellipse)temporaryElement).Width = Math.Abs(prevPoint.Value.X - mousePosition.X);
					((Ellipse)temporaryElement).Height = Math.Abs(prevPoint.Value.Y - mousePosition.Y);
					Canvas.SetLeft(temporaryElement, Math.Min(prevPoint.Value.X, mousePosition.X));
					Canvas.SetTop(temporaryElement, Math.Min(prevPoint.Value.Y, mousePosition.Y));
					canvas.Children.Add(temporaryElement);
					PropertyManager.Update((Ellipse)temporaryElement);
					break;
				case DrawingTool.Line:
					mousePosition = e.GetPosition(canvas);
					canvas.Children.Remove(temporaryElement);
					Line temporaryLine;
					if (temporaryElement == null) {
						draftSaved = false;
						temporaryElement = temporaryLine = new Line();
						temporaryLine.MouseEnter += ShapeMouseEnter;
						temporaryLine.MouseLeave += ShapeMouseLeave;
						temporaryLine.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
						temporaryLine.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
						temporaryLine.Stroke = System.Windows.Media.Brushes.Blue;
						temporaryLine.StrokeThickness = 2;
						temporaryLine.StrokeStartLineCap = PenLineCap.Round;
						temporaryLine.StrokeEndLineCap = PenLineCap.Round;
						PropertyManager.Select((Line)temporaryElement);
					}
					else
						temporaryLine = (Line)temporaryElement;

					temporaryLine.X1 = prevPoint.Value.X;
					temporaryLine.Y1 = prevPoint.Value.Y;
					temporaryLine.X2 = mousePosition.X;
					temporaryLine.Y2 = mousePosition.Y;
					canvas.Children.Add(temporaryElement);
					PropertyManager.Update(temporaryLine);
					break;
				case DrawingTool.Pen:
					mousePosition = e.GetPosition(canvas);
					draftSaved = false;
					Line myLine = new Line();
					myLine.Stroke = System.Windows.Media.Brushes.Blue;
					myLine.X1 = prevPoint.Value.X;
					myLine.Y1 = prevPoint.Value.Y;
					myLine.X2 = mousePosition.X;
					myLine.Y2 = mousePosition.Y;
					canvas.Children.Add(myLine);
					prevPoint = mousePosition;
					break;
				case DrawingTool.Select:
					mousePosition = e.GetPosition(canvasContainer);
					// boundaries
					if (prevViewMargin.Left + mousePosition.X - prevPoint.Value.X > canvasContainer.ActualWidth / 2) {
						prevViewMargin.Left = canvasContainer.ActualWidth / 2 - mousePosition.X + prevPoint.Value.X;
						prevViewMargin.Right = -canvas.ActualWidth + canvasContainer.ActualWidth / 2 - prevPoint.Value.X + mousePosition.X;
					}
					else if (prevViewMargin.Right + prevPoint.Value.X - mousePosition.X > canvasContainer.ActualWidth / 2) {
						prevViewMargin.Left = -canvas.ActualWidth + canvasContainer.ActualWidth / 2 - mousePosition.X + prevPoint.Value.X;
						prevViewMargin.Right = canvasContainer.ActualWidth / 2 - prevPoint.Value.X + mousePosition.X;
					}
					if (prevViewMargin.Top + mousePosition.Y - prevPoint.Value.Y > canvasContainer.ActualHeight / 2) {
						prevViewMargin.Top = canvasContainer.ActualHeight / 2 - mousePosition.Y + prevPoint.Value.Y;
						prevViewMargin.Bottom = -canvas.ActualHeight + canvasContainer.ActualHeight / 2 - prevPoint.Value.Y + mousePosition.Y;
					}
					else if (prevViewMargin.Bottom + prevPoint.Value.Y - mousePosition.Y > canvasContainer.ActualHeight / 2) {
						prevViewMargin.Top = -canvas.ActualHeight + canvasContainer.ActualHeight / 2 - mousePosition.Y + prevPoint.Value.Y;
						prevViewMargin.Bottom = canvasContainer.ActualHeight / 2 - prevPoint.Value.Y + mousePosition.Y;
					}

					canvas.Margin = new Thickness(prevViewMargin.Left + mousePosition.X - prevPoint.Value.X, prevViewMargin.Top + mousePosition.Y - prevPoint.Value.Y,
						prevViewMargin.Right + prevPoint.Value.X - mousePosition.X, prevViewMargin.Bottom + prevPoint.Value.Y - mousePosition.Y);
					Debug.Write("\n margin " + canvas.Margin);
					break;
			}
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			draw = false;
			temporaryElement = null;
			canvas.ReleaseMouseCapture();
		}

		private void ShapeMouseEnter(object sender, RoutedEventArgs e) {
			Debug.Write("\nShapeMouseEnter");
			if (selectedShape != null || tool != DrawingTool.Select)
				return;
			Debug.Write(" added outline");
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
			canvas.Cursor = Cursors.Hand;
		}

		private void ShapeMouseLeave(object sender, RoutedEventArgs e) {
			Debug.Write("\nShapeMouseLeave");
			if (tool != DrawingTool.Select || outline.IsMouseOver)
				return;
			Debug.Write(" removed outline");
			canvas.Children.Remove(outline);
			Canvas.SetZIndex(selectedShape, 0);
			selectedShape = null;
			canvas.Cursor = Cursors.SizeAll;
		}

		private void OutlineMouseLeave(object sender, RoutedEventArgs e) {
			Debug.Write("\nOutlineMouseLeave");
			if (selectedShape.IsMouseOver)
				return;
			Debug.Write(" removed outline");
			canvas.Children.Remove(outline);
			Canvas.SetZIndex(selectedShape, 0);
			selectedShape = null;
			canvas.Cursor = Cursors.SizeAll;
		}

		private void ShapeMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Debug.Write("\nShapeMouseLeftButtonDown");
			dynamic dynamicSender = sender;
			dynamicSender.CaptureMouse();
			PropertyManager.Select(dynamicSender);
			PropertyManager.Update(dynamicSender);
			e.Handled = true;
		}

		private void ShapeMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Debug.Write("\nShapeMouseLeftButtonUp");
			((Shape)sender).ReleaseMouseCapture();
			e.Handled = true;
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
			((sender as FrameworkElement).GetBindingExpression(TextBox.TextProperty).ResolvedSource as ValueProperty).Refresh();
		}
		#endregion

		/////////////////////////
		// Miscellaneous events
		#region events
		private void Window_ContentRendered(object sender, EventArgs e) {
			PropertyManager.Select(canvas);
			PropertyManager.Update(canvas);
			canvas.AddGrid(canvasWorkareaSize);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (draftSaved)
				return;
			MessageBoxResult result = MessageBox.Show("Unsaved changes will be lost. Do you want to save this draft?", "Warning", MessageBoxButton.YesNoCancel);
			if (result == MessageBoxResult.Yes)
				Save(null, null);
			if (result != MessageBoxResult.No)
				e.Cancel = true;
		}
		
		private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (e.NewSize.Width * e.NewSize.Height > 100000000 && (e.NewSize.Width > e.PreviousSize.Width || e.NewSize.Height > e.PreviousSize.Height)) {
				MessageBoxResult result = MessageBox.Show("Specified size is very large and Autorex might slow down your computer. Do you want to continue?", "Warning", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.No) {
					canvas.Width = canvasWorkareaSize.Width;
					canvas.Height = canvasWorkareaSize.Height;
					Dispatcher.BeginInvoke(new Action(() =>  PropertyManager.Update(canvas)));	// clear last input - not the best way
					return;
				}
			}
			canvasWorkareaSize = e.NewSize;
			canvas.Children.ClearGrid();
			canvas.AddGrid(canvasWorkareaSize);
			canvas.Margin = new Thickness(0, 0, 0, 0);
		}

		private void propertiesPanel_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e) {
			PropertyManager.UserOperation((e.OriginalSource as FrameworkElement).GetBindingExpression(TextBox.TextProperty).ParentBinding.Path.Path);
			draftSaved = false;
		}
		#endregion
	}
}
