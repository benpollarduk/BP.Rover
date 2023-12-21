using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BP.Rover.UI.ViewModels;
using Microsoft.Win32;

namespace BP.Rover.UI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Fields

        private IMainViewModel viewModel;

        #endregion

        #region Properties

        public IMainViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                DataContext = value;
            }
        }

        #endregion

        #region Constructors

        public MainView()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private string GetMapPath()
        {
            var oFDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                Filter = $"Map files (*{Map.FileExtension})|*{Map.FileExtension}",
                Title = "Import map"
            };

            oFDialog.ShowDialog();
            return oFDialog.FileName;
        }

        #endregion

        #region EventHandlers

        private void LoadCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var path = GetMapPath();

                if (string.IsNullOrEmpty(path))
                    return;

                ViewModel.LoadMap(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception caught loading map: {ex.Message}");
            }
        }

        private void ExploreCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ViewModel.Explore(e.Parameter as IRouter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception caught exploring the map: {ex.Message}");
            }
        }

        private void ResetCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Reset();
        }

        private void CancelCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Cancel();
        }

        private void GenerateRandomCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.GenerateRandomMap();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion
    }
}
