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
