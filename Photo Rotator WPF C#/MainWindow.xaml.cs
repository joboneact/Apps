using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Timers;

namespace PhotoRotatorWPF
{
    /// <summary>
    /// Main window for the Photo Rotator WPF application.
    /// Loads images from a local folder and displays them in random order,
    /// automatically rotating every few seconds, with selectable transitions.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Array to hold full file paths of all images in the target folder
        private string[]? _imageFiles;

        // Random number generator for selecting images
        private readonly Random _random = new();

        // Timer to trigger automatic photo rotation
        private System.Timers.Timer? _timer;

        // Path to the folder containing images. Update this to your desired folder.
        //private readonly string _folderPath = @"C:\Path\To\Your\Photos"; // <-- Set your folder here


        // Enum for available transitions
        private enum TransitionType { None, Fade, SlideLeft, SlideRight, Zoom }
        private TransitionType _selectedTransition = TransitionType.Fade;

        /// <summary>
        /// Initializes the main window, loads images, shows the first image, and starts the timer.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent(); // WPF method to initialize UI components from XAML
            LoadImages();          // Populate _imageFiles with image paths from the folder
            ShowRandomImage();     // Display a random image at startup
            StartTimer();          // Begin automatic rotation

            // Populate transition selection UI (if present)
            if (TransitionComboBox != null)
            {
                TransitionComboBox.ItemsSource = Enum.GetValues(typeof(TransitionType));
                TransitionComboBox.SelectedItem = _selectedTransition;
                TransitionComboBox.SelectionChanged += TransitionComboBox_SelectionChanged;
            }

            // Settings button event handler
            SettingsButton.Click += SettingsButton_Click;
        }

        // Add a Settings button to the UI and handle its click event for folder selection
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select the folder containing your photos",
                SelectedPath = _folderPath,
                ShowNewFolderButton = false
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _folderPath = dialog.SelectedPath;
                LoadImages();
                ShowRandomImage();
            }
        }

        private void TransitionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TransitionComboBox.SelectedItem is TransitionType t)
                _selectedTransition = t;
        }

        /// <summary>
        /// Loads all supported image files from the configured folder into the _imageFiles array.
        /// </summary>
        private void LoadImages()
        {
            // Supported image file extensions
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            // Get all files in the folder with supported extensions (case-insensitive)
            _imageFiles = Directory.Exists(_folderPath)
                ? Directory.GetFiles(_folderPath)
                    .Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                    .ToArray()
                : Array.Empty<string>();
        }

        /// <summary>
        /// Selects a random image from _imageFiles and displays it in the PhotoImage control.
        /// If no images are found, clears the image display.
        /// </summary>
        private void ShowRandomImage()
        {
            // If no images are loaded, clear the display and exit
            if (_imageFiles == null || _imageFiles.Length == 0)
            {
                PhotoImage.Source = null;
                return;
            }

            // Pick a random index and load the corresponding image
            var idx = _random.Next(_imageFiles.Length);

            // Create a BitmapImage and load the file
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_imageFiles[idx]);
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensures file is not locked
            bitmap.EndInit();

            // Apply transition
            ApplyTransition(() => PhotoImage.Source = bitmap);
        }

        private void ApplyTransition(Action setImage)
        {
            switch (_selectedTransition)
            {
                case TransitionType.Fade:
                    var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                    fadeOut.Completed += (s, e) =>
                    {
                        setImage();
                        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                        PhotoImage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    };
                    PhotoImage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                    break;
                case TransitionType.SlideLeft:
                    SlideTransition(setImage, from: 0, to: -PhotoImage.ActualWidth);
                    break;
                case TransitionType.SlideRight:
                    SlideTransition(setImage, from: 0, to: PhotoImage.ActualWidth);
                    break;
                case TransitionType.Zoom:
                    var zoomOut = new DoubleAnimation(1, 0.7, TimeSpan.FromMilliseconds(200));
                    zoomOut.Completed += (s, e) =>
                    {
                        setImage();
                        var zoomIn = new DoubleAnimation(0.7, 1, TimeSpan.FromMilliseconds(200));
                        PhotoImage.BeginAnimation(UIElement.RenderTransformProperty, null);
                        PhotoImage.RenderTransform = new System.Windows.Media.ScaleTransform(1, 1);
                        PhotoImage.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                        PhotoImage.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, zoomIn);
                        PhotoImage.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, zoomIn);
                    };
                    PhotoImage.RenderTransform = new System.Windows.Media.ScaleTransform(1, 1);
                    PhotoImage.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                    PhotoImage.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, zoomOut);
                    PhotoImage.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, zoomOut);
                    break;
                default:
                    setImage();
                    break;
            }
        }

        private void SlideTransition(Action setImage, double from, double to)
        {
            var trans = new System.Windows.Media.TranslateTransform();
            PhotoImage.RenderTransform = trans;
            var slideOut = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(250));
            slideOut.Completed += (s, e) =>
            {
                setImage();
                var slideIn = new DoubleAnimation(-to, 0, TimeSpan.FromMilliseconds(250));
                trans.X = -to;
                trans.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slideIn);
            };
            trans.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slideOut);
        }

        /// <summary>
        /// Starts the timer that triggers automatic random image rotation every 3 seconds.
        /// </summary>
        private void StartTimer()
        {
            _timer = new System.Timers.Timer(3000); // Interval in milliseconds
            // On each timer tick, update the image on the UI thread
            _timer.Elapsed += (s, e) => Dispatcher.Invoke(ShowRandomImage);
            _timer.Start();
        }

        /// <summary>
        /// Handles the "Next" button click event to manually show a new random image.
        /// </summary>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRandomImage();
        }
    }
}