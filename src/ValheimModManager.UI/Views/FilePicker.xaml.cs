using System.Windows;

using Microsoft.Win32;

namespace ValheimModManager.UI.Views
{
    public partial class FilePicker
    {
        public static readonly DependencyProperty FileNameProperty;
        public static readonly DependencyProperty FilterProperty;

        static FilePicker()
        {
            FileNameProperty =
                DependencyProperty.Register
                (
                    nameof(FileName),
                    typeof(string),
                    typeof(FilePicker),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

            FilterProperty =
                DependencyProperty.Register
                (
                    nameof(Filter),
                    typeof(string),
                    typeof(FilePicker),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        }

        public FilePicker()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            var dialog = 
                new OpenFileDialog
                {
                    Filter = Filter
                };

            if (dialog.ShowDialog() ?? false)
            {
                FileName = dialog.FileName;
            }
        }
    }
}
