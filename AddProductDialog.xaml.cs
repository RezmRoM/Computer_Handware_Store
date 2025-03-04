using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Computer_Hardware_Strore
{
    public partial class AddProductDialog : Window
    {
        public string ProductName { get { return ProductNameTextBox.Text; } }
        public string Description { get { return DescriptionTextBox.Text; } }
        public string ImageUrl { get { return ImageUrlTextBox.Text; } }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public string Category { get { return CategoryComboBox.SelectedItem?.ToString(); } }

        public AddProductDialog(ObservableCollection<string> categories)
        {
            InitializeComponent();
            CategoryComboBox.ItemsSource = categories;

            if (categories.Count > 0)
                CategoryComboBox.SelectedIndex = 0;

            // Добавляем обработчик для перетаскивания окна
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Пожалуйста, введите название товара.");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (Category == null)
            {
                MessageBox.Show("Пожалуйста, выберите категорию.");
                return;
            }

            Price = price;
            Quantity = quantity;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}