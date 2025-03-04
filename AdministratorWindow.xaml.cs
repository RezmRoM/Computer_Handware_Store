using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Computer_Hardware_Strore
{
    /// <summary>
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        private readonly string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user122_db;password=user122;MultipleActiveResultSets=True;App=EntityFramework";
        private readonly int _userId;
        private ObservableCollection<ProductCard> Products { get; set; }
        private ObservableCollection<Category> Categories { get; set; }
        private ObservableCollection<Order> Orders { get; set; }
        private DispatcherTimer _timer;
        private Button _activeButton;
        private const int TimerInterval = 100; // миллисекунды между изменениями при удержании
        private DispatcherTimer incrementTimer;
        private DispatcherTimer decrementTimer;
        private bool isIncrementingPrice;
        private bool isIncrementingQuantity;
        private ProductCard currentProduct;

        public AdministratorWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            Products = new ObservableCollection<ProductCard>();
            Categories = new ObservableCollection<Category>();
            Orders = new ObservableCollection<Order>();

            InitializeTimers();
            LoadCategories();
            LoadProducts();
            LoadOrders();
        }

        private void InitializeTimers()
        {
            incrementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            incrementTimer.Tick += Timer_Tick;

            decrementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            decrementTimer.Tick += Timer_Tick;

            isIncrementingPrice = false;
            isIncrementingQuantity = false;
            currentProduct = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_activeButton != null)
            {
                var tag = _activeButton.Tag as string;
                switch (tag)
                {
                    case "IncrementPrice":
                        IncrementPrice(_activeButton);
                        break;
                    case "DecrementPrice":
                        DecrementPrice(_activeButton);
                        break;
                    case "IncrementQuantity":
                        IncrementQuantity(_activeButton);
                        break;
                    case "DecrementQuantity":
                        DecrementQuantity(_activeButton);
                        break;
                }
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _activeButton = null;
            _timer.Stop();
        }

        private void IncrementPrice_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.Tag = "IncrementPrice";
            _activeButton = button;
            IncrementPrice(button);
            _timer.Start();
        }

        private void DecrementPrice_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.Tag = "DecrementPrice";
            _activeButton = button;
            DecrementPrice(button);
            _timer.Start();
        }

        private void IncrementQuantity_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.Tag = "IncrementQuantity";
            _activeButton = button;
            IncrementQuantity(button);
            _timer.Start();
        }

        private void DecrementQuantity_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.Tag = "DecrementQuantity";
            _activeButton = button;
            DecrementQuantity(button);
            _timer.Start();
        }

        private void IncrementPrice(Button button)
        {
            var product = (ProductCard)button.DataContext;
            product.Tsena += 100;
            UpdateProduct(product);
        }

        private void DecrementPrice(Button button)
        {
            var product = (ProductCard)button.DataContext;
            if (product.Tsena >= 100)
            {
                product.Tsena -= 100;
                UpdateProduct(product);
            }
        }

        private void IncrementQuantity(Button button)
        {
            var product = (ProductCard)button.DataContext;
            product.KolichestvoTovarov++;
            UpdateProduct(product);
        }

        private void DecrementQuantity(Button button)
        {
            var product = (ProductCard)button.DataContext;
            if (product.KolichestvoTovarov > 0)
            {
                product.KolichestvoTovarov--;
                UpdateProduct(product);
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT kategorii_id, nazvanie FROM CT_Kategorii ORDER BY nazvanie";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Categories.Clear();
                            while (reader.Read())
                            {
                                Categories.Add(new Category
                                {
                                    KategoriiId = reader.GetInt32(0),
                                    Nazvanie = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
                CategoriesListView.ItemsSource = Categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            z.zakaz_id,
                            z.data_zakaza,
                            z.summa,
                            CONCAT('Заказ №', z.zakaz_id) as status
                        FROM CT_Zakazi z
                        ORDER BY z.data_zakaza DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Orders.Clear();
                            while (reader.Read())
                            {
                                Orders.Add(new Order
                                {
                                    ZakazId = reader.GetInt32(0),
                                    DataZakaza = reader.GetDateTime(1),
                                    Summa = reader.GetDecimal(2),
                                    Status = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
                OrdersListView.ItemsSource = Orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать добавление категории
            MessageBox.Show("Функция добавления категории будет доступна в следующем обновлении", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            p.produkt_id,
                            p.nazvanie,
                            p.opisanie,
                            p.tsena,
                            p.kolichestvo_tovarov,
                            k.nazvanie as kategoria,
                            p.url_izobrazheniya
                        FROM CT_Produkti p
                        INNER JOIN CT_Kategorii k ON p.id_kategorii = k.kategorii_id
                        ORDER BY p.nazvanie";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Products.Clear();
                            var categoryNames = new ObservableCollection<string>(Categories.Select(c => c.Nazvanie));
                            while (reader.Read())
                            {
                                var product = new ProductCard
                                {
                                    ProduktId = reader.GetInt32(0),
                                    Nazvanie = reader.GetString(1),
                                    Opisanie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Tsena = reader.GetDecimal(3),
                                    KolichestvoTovarov = reader.GetInt32(4),
                                    Kategoria = reader.GetString(5),
                                    UrlIzobrazheniya = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Categories = categoryNames
                                };
                                Products.Add(product);
                            }
                        }
                    }
                }

                ProductsItemsControl.ItemsSource = Products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProduct(ProductCard product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE CT_Produkti SET
                            nazvanie = @name,
                            opisanie = @description,
                            tsena = @price,
                            kolichestvo_tovarov = @quantity,
                            id_kategorii = (SELECT kategorii_id FROM CT_Kategorii WHERE nazvanie = @category)
                        WHERE produkt_id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", product.ProduktId);
                        command.Parameters.AddWithValue("@name", product.Nazvanie);
                        command.Parameters.AddWithValue("@description", (object)product.Opisanie ?? DBNull.Value);
                        command.Parameters.AddWithValue("@price", product.Tsena);
                        command.Parameters.AddWithValue("@quantity", product.KolichestvoTovarov);
                        command.Parameters.AddWithValue("@category", product.Kategoria);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот товар?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM CT_Produkti WHERE produkt_id = @id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", product.ProduktId);
                            command.ExecuteNonQuery();
                            Products.Remove(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = button.DataContext;
            // TODO: Реализовать редактирование товара
            MessageBox.Show("Редактирование товара будет добавлено позже", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var category = button.DataContext;
            // TODO: Реализовать редактирование категории
            MessageBox.Show("Редактирование категории будет добавлено позже", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var category = button.DataContext;
            var result = MessageBox.Show("Вы уверены, что хотите удалить эту категорию?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM CT_Kategorii WHERE kategorii_id = @id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", ((dynamic)category).kategorii_id);
                            command.ExecuteNonQuery();
                        }
                    }
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var order = button.DataContext;
            // TODO: Реализовать просмотр деталей заказа
            MessageBox.Show("Просмотр деталей заказа будет добавлен позже", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authWindow = new AuthorizationWindow();
            authWindow.Show();
            this.Close();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDialog("Добавление товара/категории", true);
            dialog.Owner = this;
            dialog.SetCategories(new ObservableCollection<string>(Categories.Select(c => c.Nazvanie)));

            if (dialog.ShowDialog() == true)
            {
                if (dialog.IsProduct)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = @"
                                INSERT INTO CT_Produkti (
                                    nazvanie, 
                                    opisanie, 
                                    tsena, 
                                    kolichestvo_tovarov, 
                                    id_kategorii
                                ) VALUES (
                                    @name,
                                    @description,
                                    @price,
                                    @quantity,
                                    (SELECT kategorii_id FROM CT_Kategorii WHERE nazvanie = @category)
                                )";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@name", dialog.ProductName);
                                command.Parameters.AddWithValue("@description", string.IsNullOrEmpty(dialog.Description) ? DBNull.Value : (object)dialog.Description);
                                command.Parameters.AddWithValue("@price", dialog.Price);
                                command.Parameters.AddWithValue("@quantity", dialog.Quantity);
                                command.Parameters.AddWithValue("@category", dialog.Category);

                                command.ExecuteNonQuery();
                            }
                        }
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO CT_Kategorii (nazvanie) VALUES (@name)";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@name", dialog.CategoryName);
                                command.ExecuteNonQuery();
                            }
                        }
                        LoadCategories();
                        LoadProducts(); // Обновляем список товаров, так как изменились категории
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ProductCard product)
            {
                string propertyName = ((Binding)textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding).Path.Path;
                switch (propertyName)
                {
                    case "Nazvanie":
                        UpdateProductInDatabase(product, "nazvanie", textBox.Text);
                        break;
                    case "Opisanie":
                        UpdateProductInDatabase(product, "opisanie", textBox.Text);
                        break;
                    case "Tsena":
                        if (decimal.TryParse(textBox.Text, out decimal price))
                        {
                            UpdateProductInDatabase(product, "tsena", price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        break;
                    case "KolichestvoTovarov":
                        if (int.TryParse(textBox.Text, out int quantity))
                        {
                            UpdateProductInDatabase(product, "kolichestvo_tovarov", quantity.ToString());
                        }
                        break;
                }
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is ProductCard product)
            {
                string newCategory = comboBox.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(newCategory))
                {
                    product.Kategoria = newCategory;
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = @"
                                UPDATE CT_Produkti 
                                SET id_kategorii = (SELECT kategorii_id FROM CT_Kategorii WHERE nazvanie = @category)
                                WHERE produkt_id = @id";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@category", newCategory);
                                command.Parameters.AddWithValue("@id", product.ProduktId);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoadProducts();
                    }
                }
            }
        }

        private void CategoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Category category)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE CT_Kategorii SET nazvanie = @name WHERE kategorii_id = @id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", textBox.Text);
                            command.Parameters.AddWithValue("@id", category.KategoriiId);
                            command.ExecuteNonQuery();
                        }
                    }
                    // Обновляем списки после изменения названия категории
                    LoadCategories();
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении названия категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadCategories();
                }
            }
        }

        private void UpdateProductInDatabase(ProductCard product, string fieldName, string value)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE CT_Produkti SET {fieldName} = @value WHERE produkt_id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (fieldName == "tsena")
                        {
                            command.Parameters.AddWithValue("@value", decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (fieldName == "kolichestvo_tovarov")
                        {
                            command.Parameters.AddWithValue("@value", int.Parse(value));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@value", value);
                        }
                        command.Parameters.AddWithValue("@id", product.ProduktId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadProducts(); // Перезагружаем список товаров в случае ошибки
            }
        }
    }

    public class Category
    {
        public int KategoriiId { get; set; }
        public string Nazvanie { get; set; }
    }

    public class Order
    {
        public int ZakazId { get; set; }
        public DateTime DataZakaza { get; set; }
        public string Status { get; set; }
        public decimal Summa { get; set; }
    }

    public class AddDialog : Window
    {
        private TextBox nameTextBox;
        private TextBox descriptionTextBox;
        private TextBox priceTextBox;
        private TextBox quantityTextBox;
        private ComboBox categoryComboBox;
        private CheckBox isProductCheckBox;
        private StackPanel productFields;

        public string ProductName => nameTextBox.Text;
        public string CategoryName => nameTextBox.Text;
        public string Description => descriptionTextBox.Text;
        public decimal Price => decimal.Parse(priceTextBox.Text);
        public int Quantity => int.Parse(quantityTextBox.Text);
        public string Category => categoryComboBox.SelectedItem?.ToString();
        public bool IsProduct => isProductCheckBox.IsChecked ?? true;

        public AddDialog(string title, bool showTypeSelector = false)
        {
            Title = title;
            Width = 400;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = System.Windows.Media.Brushes.Transparent;

            // Создаем стиль для CheckBox
            var checkBoxStyle = new Style(typeof(CheckBox));

            // Устанавливаем свойства
            checkBoxStyle.Setters.Add(new Setter(ForegroundProperty, Application.Current.Resources["TextColor"]));
            checkBoxStyle.Setters.Add(new Setter(BackgroundProperty, System.Windows.Media.Brushes.Transparent));
            checkBoxStyle.Setters.Add(new Setter(BorderBrushProperty, Application.Current.Resources["PrimaryColor"]));

            // Создаем шаблон
            var template = new ControlTemplate(typeof(CheckBox));
            var templateGrid = new FrameworkElementFactory(typeof(Grid));

            // Определяем колонки
            var colDef1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            colDef1.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
            var colDef2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            colDef2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));

            templateGrid.AppendChild(colDef1);
            templateGrid.AppendChild(colDef2);

            // Создаем Border для чекбокса
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.WidthProperty, 20.0);
            border.SetValue(Border.HeightProperty, 20.0);
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(BorderBrushProperty));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));
            border.Name = "Border";

            // Создаем Path для галочки
            var checkmark = new FrameworkElementFactory(typeof(Path));
            checkmark.SetValue(Path.WidthProperty, 10.0);
            checkmark.SetValue(Path.HeightProperty, 10.0);
            checkmark.SetValue(Path.DataProperty, Geometry.Parse("M 0,4 L 3,8 L 8,0"));
            checkmark.SetValue(Path.StrokeProperty, Application.Current.Resources["TextColor"]);
            checkmark.SetValue(Path.StrokeThicknessProperty, 2.0);
            checkmark.SetValue(Path.VisibilityProperty, Visibility.Collapsed);
            checkmark.Name = "Checkmark";

            border.AppendChild(checkmark);
            templateGrid.AppendChild(border);

            // Добавляем ContentPresenter
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(Grid.ColumnProperty, 1);
            contentPresenter.SetValue(MarginProperty, new Thickness(10, 0, 0, 0));
            contentPresenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);

            templateGrid.AppendChild(contentPresenter);

            template.VisualTree = templateGrid;

            // Добавляем триггер для IsChecked
            var checkedTrigger = new Trigger { Property = ToggleButton.IsCheckedProperty, Value = true };
            checkedTrigger.Setters.Add(new Setter(Border.BackgroundProperty, Application.Current.Resources["PrimaryColor"], "Border"));
            checkedTrigger.Setters.Add(new Setter(Path.VisibilityProperty, Visibility.Visible, "Checkmark"));

            template.Triggers.Add(checkedTrigger);

            checkBoxStyle.Setters.Add(new Setter(TemplateProperty, template));

            // Сохраняем стиль в ресурсах окна
            this.Resources.Add("ModernCheckBox", checkBoxStyle);

            var mainBorder = new Border
            {
                Background = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["SurfaceColor"],
                CornerRadius = new CornerRadius(15),
                Margin = new Thickness(10),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Color = System.Windows.Media.Color.FromRgb(37, 99, 235)
                }
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var titleTextBlock = new TextBlock
            {
                Text = title,
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"],
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };
            Grid.SetRow(titleTextBlock, 0);

            var inputStackPanel = new StackPanel { Margin = new Thickness(30, 10, 30, 10) };

            if (showTypeSelector)
            {
                isProductCheckBox = new CheckBox
                {
                    Content = "Добавить товар",
                    Style = (Style)FindResource("ModernCheckBox"),
                    IsChecked = true,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                isProductCheckBox.Checked += (s, e) => productFields.Visibility = Visibility.Visible;
                isProductCheckBox.Unchecked += (s, e) => productFields.Visibility = Visibility.Collapsed;
                inputStackPanel.Children.Add(isProductCheckBox);
            }

            // Название
            inputStackPanel.Children.Add(new TextBlock { Text = "Название:", Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"], Margin = new Thickness(0, 0, 0, 5) });
            nameTextBox = new TextBox { Height = 40, Style = (Style)FindResource("ModernTextBox") };
            inputStackPanel.Children.Add(nameTextBox);

            productFields = new StackPanel();

            // Описание
            productFields.Children.Add(new TextBlock { Text = "Описание:", Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"], Margin = new Thickness(0, 15, 0, 5) });
            descriptionTextBox = new TextBox { Height = 80, Style = (Style)FindResource("ModernTextBox"), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
            productFields.Children.Add(descriptionTextBox);

            // Цена
            productFields.Children.Add(new TextBlock { Text = "Цена:", Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"], Margin = new Thickness(0, 15, 0, 5) });
            priceTextBox = new TextBox { Height = 40, Style = (Style)FindResource("ModernTextBox") };
            productFields.Children.Add(priceTextBox);

            // Количество
            productFields.Children.Add(new TextBlock { Text = "Количество:", Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"], Margin = new Thickness(0, 15, 0, 5) });
            quantityTextBox = new TextBox { Height = 40, Style = (Style)FindResource("ModernTextBox") };
            productFields.Children.Add(quantityTextBox);

            // Категория
            productFields.Children.Add(new TextBlock { Text = "Категория:", Foreground = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["TextColor"], Margin = new Thickness(0, 15, 0, 5) });
            categoryComboBox = new ComboBox { Height = 40, Style = (Style)FindResource("ModernComboBox") };
            productFields.Children.Add(categoryComboBox);

            if (showTypeSelector)
            {
                inputStackPanel.Children.Add(productFields);
            }
            else
            {
                foreach (var child in productFields.Children.Cast<UIElement>().ToList())
                {
                    inputStackPanel.Children.Add(child);
                }
            }

            Grid.SetRow(inputStackPanel, 1);

            var buttonsStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };

            var addButton = new Button
            {
                Content = "Добавить",
                Style = (Style)FindResource("ModernButton"),
                Width = 120,
                Height = 40,
                Margin = new Thickness(0, 0, 10, 0)
            };
            addButton.Click += (s, e) =>
            {
                if (ValidateInput())
                    DialogResult = true;
            };

            var cancelButton = new Button
            {
                Content = "Отмена",
                Style = (Style)FindResource("ModernButton"),
                Width = 120,
                Height = 40
            };
            cancelButton.Click += (s, e) => DialogResult = false;

            buttonsStackPanel.Children.Add(addButton);
            buttonsStackPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonsStackPanel, 2);

            mainGrid.Children.Add(titleTextBlock);
            mainGrid.Children.Add(inputStackPanel);
            mainGrid.Children.Add(buttonsStackPanel);

            mainBorder.Child = mainGrid;
            Content = mainBorder;

            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название.");
                return false;
            }

            if (IsProduct)
            {
                if (!decimal.TryParse(priceTextBox.Text, out _))
                {
                    MessageBox.Show("Пожалуйста, введите корректную цену.");
                    return false;
                }

                if (!int.TryParse(quantityTextBox.Text, out _))
                {
                    MessageBox.Show("Пожалуйста, введите корректное количество.");
                    return false;
                }

                if (Category == null)
                {
                    MessageBox.Show("Пожалуйста, выберите категорию.");
                    return false;
                }
            }

            return true;
        }

        public void SetCategories(ObservableCollection<string> categories)
        {
            categoryComboBox.ItemsSource = categories;
            if (categories.Count > 0)
                categoryComboBox.SelectedIndex = 0;
        }
    }
}
