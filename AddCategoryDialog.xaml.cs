using System.Windows;

namespace Computer_Hardware_Strore
{
    public partial class AddCategoryDialog : Window
    {
        public string CategoryName { get { return CategoryNameTextBox.Text; } }

        public AddCategoryDialog()
        {
            InitializeComponent();

            // Добавляем обработчик для перетаскивания окна
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                MessageBox.Show("Пожалуйста, введите название категории.");
                return;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}