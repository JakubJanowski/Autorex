using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autorex {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			// Create a StackPanel to contain the shape.
			StackPanel myStackPanel = new StackPanel();

			// Create a red Ellipse.
			Ellipse myEllipse = new Ellipse();

			// Create a SolidColorBrush with a red color to fill the 
			// Ellipse with.
			SolidColorBrush mySolidColorBrush = new SolidColorBrush();

			// Describes the brush's color using RGB values. 
			// Each value has a range of 0-255.
			mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
			myEllipse.Fill = mySolidColorBrush;
			myEllipse.StrokeThickness = 2;
			myEllipse.Stroke = Brushes.Black;

			// Set the width and height of the Ellipse.
			myEllipse.Width = 200;
			myEllipse.Height = 100;

			// Add the Ellipse to the StackPanel.
			myStackPanel.Children.Add(myEllipse);

			//this.Content = myStackPanel;
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
	}
}
