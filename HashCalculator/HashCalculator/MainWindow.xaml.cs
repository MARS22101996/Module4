using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using HashCalculator.ViewModels;

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

            _event = new FilesCalculatorViewModel();

            DataContext = _event;     
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {           
            var openFileDialog = new CommonOpenFileDialog();

            ConfigureFileDialog(openFileDialog);          

            _event.ConfigureFileInfo(openFileDialog.FileName);                        
        }

        private void ConfigureFileDialog(CommonOpenFileDialog openFileDialog)
        {
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog.IsFolderPicker = true;

            openFileDialog.ShowDialog();
        }

        private async void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _event.Cancel();          
        }
    }
}
