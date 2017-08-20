using HashCalculator.Calculator.Concrete;
using HashCalculator.Models;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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


namespace HashCalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FilesCalculatorViewModel _event;
        public MainWindow()
        {
            InitializeComponent();

           // bind the Date to the UI
            this.DataContext = _event;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new CommonOpenFileDialog();

            ConfigureFileDialog(openFileDialog);

            _event = new FilesCalculatorViewModel();

            _event.ConfigureFileInfo(openFileDialog.FileName);

            dataGrid.ItemsSource = _event.FilesInfo;
           
        }

        private void ConfigureFileDialog(CommonOpenFileDialog openFileDialog)
        {
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog.IsFolderPicker = true;

            openFileDialog.ShowDialog();
        }
    }
}
