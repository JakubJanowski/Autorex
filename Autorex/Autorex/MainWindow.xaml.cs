using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Autorex.Binding;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.ComponentModel;

namespace Autorex {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		DrawingTool tool = DrawingTool.Select;
		bool draw = false;
		bool draftSaved = true;
		bool windowInitialised = false;
		string filename;
		Point? prevPoint;
		Shape outline;
		Shape selectedShape;
		double initialWorkareaHeight = 1000;
		double initialWorkareaWidth = 1000;
		UIElement temporaryElement;
		public PropertyManager PropertyManager { get; set; } = new PropertyManager();

		double virtualScreenHeight;
		double virtualScreenWidth;


		public MainWindow() {
			InitializeComponent();
			propertiesPanel.DataContext = PropertyManager;
			col1Image.Source = new BitmapImage(new Uri("Resources/CursorPosition.gif", UriKind.Relative));
			virtualScreenHeight = SystemParameters.VirtualScreenHeight;
			virtualScreenWidth = SystemParameters.VirtualScreenWidth;
			SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
			if (Environment.GetCommandLineArgs().Length == 2) {
				filename = Environment.GetCommandLineArgs()[1];
				LoadFile();
			}
			else {
				canvas.Width = initialWorkareaWidth;
				canvas.Height = initialWorkareaHeight;
			}
		}

		private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e) {
			Debug.WriteLine("Resolution Changed");
			// get new rectangle bounding all screens
			if (SystemParameters.VirtualScreenWidth > virtualScreenWidth) {
				virtualScreenWidth = SystemParameters.VirtualScreenWidth;
				if (SystemParameters.VirtualScreenHeight > virtualScreenHeight)
					virtualScreenHeight = SystemParameters.VirtualScreenHeight;
				canvasGrid.Children.ClearGrid();
				canvasGrid.AddGrid(virtualScreenHeight, virtualScreenWidth);
				bottomScale.Children.Clear();
				sideScale.Children.Clear();
				Utilities.InitialiseGraduationScale(sideScale, bottomScale, canvasBorder.Margin, virtualScreenHeight, virtualScreenWidth);
			}
			else if (SystemParameters.VirtualScreenHeight > virtualScreenHeight) {
				virtualScreenHeight = SystemParameters.VirtualScreenHeight;
				canvasGrid.Children.ClearGrid();
				canvasGrid.AddGrid(virtualScreenHeight, virtualScreenWidth);
				bottomScale.Children.Clear();
				sideScale.Children.Clear();
				Utilities.InitialiseGraduationScale(sideScale, bottomScale, canvasBorder.Margin, virtualScreenHeight, virtualScreenWidth);
			}
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
			canvasBorder.Margin = Utilities.GetInitialMargin(canvasContainer.ActualWidth - canvas.Width, canvasContainer.ActualHeight - canvas.Height);

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

			LoadFile();
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
			if (tool == DrawingTool.Select) {
				PropertyManager.Select(canvas);
				PropertyManager.Update(canvas);
				prevPoint = e.GetPosition(canvasContainer);
				Debug.WriteLine("canvas_MouseLeftButtonDown " + canvasBorder.Margin.Top + " " + canvasBorder.Margin.Right + " " + canvasBorder.Margin.Bottom + " " + canvasBorder.Margin.Left);
			}
			else
				prevPoint = e.GetPosition(canvas);
			canvas.CaptureMouse();
		}

		private void canvas_MouseMove(object sender, MouseEventArgs e) {
			Point mousePositionOnCanvas = e.GetPosition(canvas);
			col1Label.Content = mousePositionOnCanvas.X + ", " + mousePositionOnCanvas.Y;
			if (!draw)
				return;

			switch (tool) {
				case DrawingTool.Ellipse:
					DrawEllipse(mousePositionOnCanvas);
					break;
				case DrawingTool.Line:
					DrawLine(mousePositionOnCanvas);
					break;
				case DrawingTool.Pen:
					DrawPen(mousePositionOnCanvas);
					break;
				case DrawingTool.Select:
					Select(e.GetPosition(canvasContainer));
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
			outline = Utilities.Outline(dynamicSender);
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
			if (selectedShape == null)
				return;
			/*dynamic dynamicSender = sender;
			dynamicSender.CaptureMouse();
			PropertyManager.Select(dynamicSender);
			PropertyManager.Update(dynamicSender);*/
			dynamic dynamicShape = selectedShape;
			selectedShape.CaptureMouse();
			PropertyManager.Select(dynamicShape);
			PropertyManager.Update(dynamicShape);
			e.Handled = true;
		}

		private void ShapeMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Debug.Write("\nShapeMouseLeftButtonUp");
			if (selectedShape == null)
				return;
			//((Shape)sender).ReleaseMouseCapture();
			selectedShape.ReleaseMouseCapture();
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
			canvasGrid.AddGrid(virtualScreenHeight, virtualScreenWidth);
			Utilities.InitialiseGraduationScale(sideScale, bottomScale, canvasBorder.Margin, virtualScreenHeight, virtualScreenWidth);
			PropertyManager.Select(canvas);
			PropertyManager.Update(canvas);
			windowInitialised = true;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (draftSaved)
				return;
			MessageBoxResult result = MessageBox.Show("Unsaved changes will be lost. Do you want to save this draft?", "Warning", MessageBoxButton.YesNoCancel);
			if (result == MessageBoxResult.Yes)
				Save(null, null);
			if (result == MessageBoxResult.Cancel)
				e.Cancel = true;
		}

		private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (!windowInitialised)
				return;

			canvasGrid.Width = e.NewSize.Width > canvasContainer.ActualWidth + 99 ? canvasContainer.ActualWidth + 99 : e.NewSize.Width;
			canvasGrid.Height = e.NewSize.Height > canvasContainer.ActualHeight + 99 ? canvasContainer.ActualHeight + 99 : e.NewSize.Height;
			canvasBorder.Margin = Utilities.CalculateMargin(canvasBorder.Margin, e.NewSize, e.PreviousSize, canvasContainer, canvas);
			canvasGrid.UpdateGrid(canvas, canvasBorder.Margin);
			Utilities.UpdateGraduationScale(sideScale, bottomScale, canvasBorder.Margin);
			PropertyManager.Update(canvas);
		}

		private void canvasContainer_SizeChanged(object sender, SizeChangedEventArgs e) {
			canvasGrid.Width = canvas.ActualWidth > e.NewSize.Width + 99 ? e.NewSize.Width + 99 : canvas.ActualWidth;
			canvasGrid.Height = canvas.ActualHeight > e.NewSize.Height + 99 ? e.NewSize.Height + 99 : canvas.ActualHeight;

			//// boundaries
			if (!windowInitialised)
				canvasBorder.Margin = Utilities.GetInitialMargin(canvasContainer.ActualWidth - canvas.Width, canvasContainer.ActualHeight - canvas.Height);
			else
				canvasBorder.Margin = Utilities.CalculateMargin(canvasBorder.Margin, e.NewSize, e.PreviousSize, canvasContainer, canvas);
			canvasGrid.UpdateGrid(canvas, canvasBorder.Margin);
			Utilities.UpdateGraduationScale(sideScale, bottomScale, canvasBorder.Margin);
		}

		/* method variables
		bool reverted = false;
		bool warn = true;
		[Obsolete("function is obsolete", true)]
		private void canvas_SizeChanged_obsolete(object sender, SizeChangedEventArgs e) {
			if (!windowInitialised)
				return;

			Dispatcher.BeginInvoke(new Action(() => {
				if (warn) {
					if (e.NewSize.Width * e.NewSize.Height > 100000000 && (e.NewSize.Width > e.PreviousSize.Width || e.NewSize.Height > e.PreviousSize.Height)) {
						MessageBoxResult result = MessageBox.Show("Specified size is very large and Autorex might slow down your computer. Do you want to continue?", "Warning", MessageBoxButton.YesNo);
						if (result == MessageBoxResult.No) {
							canvas.Width = canvasWorkareaSize.Width;
							canvas.Height = canvasWorkareaSize.Height;
							reverted = true;
							return;
						}
						warn = false;
					}
				}
				else if (e.PreviousSize.Width * e.PreviousSize.Height > 100000000 && e.NewSize.Width * e.NewSize.Height < 100000000)
					warn = true;

				if (!reverted) {
					canvas.Children.ClearGrid();
					try {
						canvas.AddGrid(e.NewSize);
						canvasWorkareaSize = e.NewSize;
						canvasBorder.Margin = Utilities.CalculateMargin(canvasBorder.Margin, e.NewSize, e.PreviousSize, canvasContainer, canvas);
					} catch (OutOfMemoryException) {
						canvas.Children.ClearGrid();
						canvas.Width = canvasWorkareaSize.Width;
						canvas.Height = canvasWorkareaSize.Height;
						MessageBox.Show("Your computer doesn't have enough memory for a draft of this size.", "Error", MessageBoxButton.OK);
						canvas.AddGrid(canvasWorkareaSize);
					}
				}
				reverted = false;
				PropertyManager.Update(canvas);
			}));
		}*/

		private void propertiesPanel_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e) {
			PropertyManager.UserOperation((e.OriginalSource as FrameworkElement).GetBindingExpression(TextBox.TextProperty).ParentBinding.Path.Path);
			draftSaved = false;
		}
		#endregion



		/////////////////////////
		// Helper functions
		#region helper functions
		/// <summary>
		/// Load file specified by class filename field
		/// </summary>
		private void LoadFile() {
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
					switch ((DrawingTool)binaryReader.ReadByte()) {
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

		private void DrawEllipse(Point mousePosition) {
			Ellipse temporaryEllipse;
			canvas.Children.Remove(temporaryElement);
			if (temporaryElement == null) {
				draftSaved = false;
				temporaryElement = temporaryEllipse = new Ellipse();
				temporaryEllipse.MouseEnter += ShapeMouseEnter;
				temporaryEllipse.MouseLeave += ShapeMouseLeave;
				temporaryEllipse.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
				temporaryEllipse.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
				temporaryEllipse.StrokeThickness = 2;
				PropertyManager.Select(temporaryEllipse);
			}
			else
				temporaryEllipse = (Ellipse)temporaryElement;
			temporaryEllipse.Stroke = System.Windows.Media.Brushes.Blue;
			temporaryEllipse.Width = Math.Abs(prevPoint.Value.X - mousePosition.X);
			temporaryEllipse.Height = Math.Abs(prevPoint.Value.Y - mousePosition.Y);
			Canvas.SetLeft(temporaryElement, Math.Min(prevPoint.Value.X, mousePosition.X));
			Canvas.SetTop(temporaryElement, Math.Min(prevPoint.Value.Y, mousePosition.Y));
			canvas.Children.Add(temporaryElement);
			PropertyManager.Update(temporaryEllipse);
		}

		private void DrawLine(Point mousePosition) {
			Line temporaryLine;
			canvas.Children.Remove(temporaryElement);
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
				PropertyManager.Select(temporaryLine);
			}
			else
				temporaryLine = (Line)temporaryElement;

			temporaryLine.X1 = prevPoint.Value.X;
			temporaryLine.Y1 = prevPoint.Value.Y;
			temporaryLine.X2 = mousePosition.X;
			temporaryLine.Y2 = mousePosition.Y;
			canvas.Children.Add(temporaryElement);
			PropertyManager.Update(temporaryLine);
		}

		private void DrawPen(Point mousePosition) {
			draftSaved = false;
			Line myLine = new Line();
			myLine.Stroke = System.Windows.Media.Brushes.Blue;
			myLine.X1 = prevPoint.Value.X;
			myLine.Y1 = prevPoint.Value.Y;
			myLine.X2 = mousePosition.X;
			myLine.Y2 = mousePosition.Y;
			canvas.Children.Add(myLine);
			prevPoint = mousePosition;
		}

		private void Select(Point mousePosition) {
			// boundaries
			Thickness margin = canvasBorder.Margin;
			if (margin.Left + mousePosition.X - prevPoint.Value.X > canvasContainer.ActualWidth / 2) {
				margin.Left = Math.Floor(canvasContainer.ActualWidth / 2 - mousePosition.X + prevPoint.Value.X);
				margin.Right = canvasContainer.ActualWidth - canvas.ActualWidth - 2 - margin.Left;
			}
			else if (margin.Right + prevPoint.Value.X - mousePosition.X > canvasContainer.ActualWidth / 2) {
				margin.Left = Math.Floor(-canvas.ActualWidth - 2 + canvasContainer.ActualWidth / 2 - mousePosition.X + prevPoint.Value.X);
				margin.Right = canvasContainer.ActualWidth - canvas.ActualWidth - 2 - margin.Left;
			}
			if (margin.Top + mousePosition.Y - prevPoint.Value.Y > canvasContainer.ActualHeight / 2) {
				margin.Top = Math.Floor(canvasContainer.ActualHeight / 2 - mousePosition.Y + prevPoint.Value.Y);
				margin.Bottom = canvasContainer.ActualHeight - canvas.ActualHeight - 2 - margin.Top;
			}
			else if (margin.Bottom + prevPoint.Value.Y - mousePosition.Y > canvasContainer.ActualHeight / 2) {
				margin.Top = Math.Floor(-canvas.ActualHeight - 2 + canvasContainer.ActualHeight / 2 - mousePosition.Y + prevPoint.Value.Y);
				margin.Bottom = canvasContainer.ActualHeight - canvas.ActualHeight - 2 - margin.Top;
			}

			canvasBorder.Margin = new Thickness(margin.Left + mousePosition.X - prevPoint.Value.X, margin.Top + mousePosition.Y - prevPoint.Value.Y,
				margin.Right + prevPoint.Value.X - mousePosition.X, margin.Bottom + prevPoint.Value.Y - mousePosition.Y);
			canvasGrid.UpdateGrid(canvas, canvasBorder.Margin);
			Utilities.UpdateGraduationScale(sideScale, bottomScale, canvasBorder.Margin);
			prevPoint = mousePosition;
		}
		#endregion
	}
}
