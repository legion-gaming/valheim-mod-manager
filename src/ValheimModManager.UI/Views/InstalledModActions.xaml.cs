using System.Windows;
using System.Windows.Input;

namespace ValheimModManager.UI.Views
{
    public partial class InstalledModActions
    {
        public static readonly DependencyProperty UpdateCommandProperty;
        public static readonly DependencyProperty UninstallCommandProperty;
        public static readonly DependencyProperty UninstallCommandParameterProperty;
        public static readonly DependencyProperty UninstallWithoutDependenciesCommandProperty;
        public static readonly DependencyProperty UninstallWithoutDependenciesCommandParameterProperty;

        static InstalledModActions()
        {
            UpdateCommandProperty =
                DependencyProperty.Register
                (
                    nameof(UpdateCommand),
                    typeof(ICommand),
                    typeof(InstalledModActions),
                    new PropertyMetadata(default(ICommand))
                );

            UninstallCommandProperty =
                DependencyProperty.Register
                (
                    nameof(UninstallCommand),
                    typeof(ICommand),
                    typeof(InstalledModActions),
                    new PropertyMetadata(default(ICommand))
                );

            UninstallCommandParameterProperty =
                DependencyProperty.Register
                (
                    nameof(UninstallCommandParameter),
                    typeof(object),
                    typeof(InstalledModActions),
                    new PropertyMetadata(null)
                );

            UninstallWithoutDependenciesCommandProperty =
                DependencyProperty.Register
                (
                    nameof(UninstallWithoutDependenciesCommand),
                    typeof(ICommand),
                    typeof(InstalledModActions),
                    new PropertyMetadata(default(ICommand))
                );

            UninstallWithoutDependenciesCommandParameterProperty =
                DependencyProperty.Register
                (
                    nameof(UninstallWithoutDependenciesCommandParameter),
                    typeof(object),
                    typeof(InstalledModActions),
                    new PropertyMetadata(default)
                );
        }

        public InstalledModActions()
        {
            InitializeComponent();
        }

        public ICommand UpdateCommand
        {
            get { return (ICommand)GetValue(UpdateCommandProperty); }
            set { SetValue(UpdateCommandProperty, value); }
        }

        public ICommand UninstallCommand
        {
            get { return (ICommand)GetValue(UninstallCommandProperty); }
            set { SetValue(UninstallCommandProperty, value); }
        }

        public object UninstallCommandParameter
        {
            get { return GetValue(UninstallCommandParameterProperty); }
            set { SetValue(UninstallCommandParameterProperty, value); }
        }

        public ICommand UninstallWithoutDependenciesCommand
        {
            get { return (ICommand)GetValue(UninstallWithoutDependenciesCommandProperty); }
            set { SetValue(UninstallWithoutDependenciesCommandProperty, value); }
        }

        public object UninstallWithoutDependenciesCommandParameter
        {
            get { return GetValue(UninstallWithoutDependenciesCommandParameterProperty); }
            set { SetValue(UninstallWithoutDependenciesCommandParameterProperty, value); }
        }
    }
}
