using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using HashCalculator.ViewModels;
using Microsoft.Practices.Unity;

namespace HashCalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FilesCalculatorViewModel _model;

        public MainWindow()
        {
            InitializeComponent();

            _model = new FilesCalculatorViewModel();

            DataContext = _model;
        }
    }
}
