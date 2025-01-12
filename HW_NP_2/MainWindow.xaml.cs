using Client.Services;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HW_NP_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedImagePath;
        private readonly ServerService _serverService;

        public MainWindow()
        {
            InitializeComponent();
            _selectedImagePath = String.Empty;
            _serverService = new ServerService();
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Виберіть зображення"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                FileNameTextBox.Text = $"{_selectedImagePath}";
                UploadButton.IsEnabled = true;
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_selectedImagePath))
            {
                MessageBox.Show("Виберіть файл перед завантаженням!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UploadButton.IsEnabled = false;

            try
            {
                var uploadResult = await _serverService.UploadImageAsync(_selectedImagePath);

                if (uploadResult.IsSuccess)
                {
                    var searchResult = await _serverService.SearchImageAsync(uploadResult.ImageUrl);

                    if (searchResult.IsSuccess)
                        DisplayImage(searchResult.ImageUrl);
                    else
                        MessageBox.Show($"Помилка під час пошуку: {searchResult.ErrorMessage}");
                }
                else
                    MessageBox.Show($"Помилка під час завантаження: {uploadResult.ErrorMessage}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                UploadButton.IsEnabled = true;
            }
        }

        private void DisplayImage(string imageUrl)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            UploadedImage.Source = bitmap;
        }
    }
}