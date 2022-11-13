using System;
using System.Windows;
using System.Windows.Input;

namespace ValheimModManager.UI.Views
{
    public partial class Pagination
    {
        public static readonly DependencyProperty PageProperty;
        public static readonly DependencyProperty PageCountProperty;
        public static readonly DependencyProperty PageSizeProperty;
        public static readonly DependencyProperty SortProperty;
        public static readonly DependencyProperty ItemCountProperty;
        public static readonly DependencyProperty PreviousCommandProperty;
        public static readonly DependencyProperty NextCommandProperty;

        static Pagination()
        {
            PageProperty =
                DependencyProperty.Register
                (
                    nameof(Page),
                    typeof(int),
                    typeof(Pagination),
                    new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

            PageCountProperty =
                DependencyProperty.Register
                (
                    nameof(PageCount),
                    typeof(int),
                    typeof(Pagination),
                    new PropertyMetadata(default(int))
                );

            PageSizeProperty =
                DependencyProperty.Register
                (
                    nameof(PageSize),
                    typeof(int),
                    typeof(Pagination),
                    new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

            SortProperty =
                DependencyProperty.Register
                (
                    nameof(Sort),
                    typeof(string),
                    typeof(Pagination),
                    new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

            ItemCountProperty =
                DependencyProperty.Register
                (
                    nameof(ItemCount),
                    typeof(int),
                    typeof(Pagination),
                    new PropertyMetadata(default(int))
                );

            PreviousCommandProperty =
                DependencyProperty.Register
                (
                    nameof(PreviousCommand),
                    typeof(ICommand),
                    typeof(Pagination),
                    new PropertyMetadata(null)
                );

            NextCommandProperty =
                DependencyProperty.Register
                (
                    nameof(NextCommand),
                    typeof(ICommand),
                    typeof(Pagination),
                    new PropertyMetadata(null)
                );
        }

        public Pagination()
        {
            InitializeComponent();
        }

        public int Page
        {
            get { return (int)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        public string Sort
        {
            get { return (string)GetValue(SortProperty); }
            set { SetValue(SortProperty, value); }
        }

        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }

        public ICommand PreviousCommand
        {
            get { return (ICommand)GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }

        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }
    }
}
