using System.Windows;
using System.Windows.Input;

namespace ValheimModManager.UI.Views
{
    public partial class OnlineModActions
    {
        public static readonly DependencyProperty DownloadCommandProperty;
        public static readonly DependencyProperty DownloadWithoutDependenciesCommandProperty;

        static OnlineModActions()
        {
            DownloadCommandProperty =
                DependencyProperty.Register
                (
                    nameof(DownloadCommand),
                    typeof(ICommand),
                    typeof(OnlineModActions),
                    new PropertyMetadata(default(ICommand))
                );

            DownloadWithoutDependenciesCommandProperty =
                DependencyProperty.Register
                (
                    nameof(DownloadWithoutDependenciesCommand),
                    typeof(ICommand),
                    typeof(OnlineModActions),
                    new PropertyMetadata(default(ICommand))
                );
        }

        public OnlineModActions()
        {
            InitializeComponent();
        }

        public ICommand DownloadCommand
        {
            get { return (ICommand)GetValue(DownloadCommandProperty); }
            set { SetValue(DownloadCommandProperty, value); }
        }

        public ICommand DownloadWithoutDependenciesCommand
        {
            get { return (ICommand)GetValue(DownloadWithoutDependenciesCommandProperty); }
            set { SetValue(DownloadWithoutDependenciesCommandProperty, value); }
        }
    }
}
