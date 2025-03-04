using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Computer_Hardware_Strore
{
    /// <summary>
    /// Логика взаимодействия для BuyerWindow.xaml
    /// </summary>
    public partial class BuyerWindow : Window, INotifyPropertyChanged
    {
        private readonly string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user122_db;password=user122;MultipleActiveResultSets=True;App=EntityFramework";
        private ObservableCollection<ProductCard> Products { get; set; }
        private ObservableCollection<ProductCard> FilteredProducts { get; set; }
        private readonly int _userId;
        private string _userFullName;
        private string _userAvatarUrl;
        private int _cartItemsCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public string UserFullName
        {
            get => _userFullName;
            set
            {
                _userFullName = value;
                OnPropertyChanged(nameof(UserFullName));
            }
        }

        public string UserAvatarUrl
        {
            get => _userAvatarUrl;
            set
            {
                _userAvatarUrl = value;
                OnPropertyChanged(nameof(UserAvatarUrl));
            }
        }

        public int CartItemsCount
        {
            get => _cartItemsCount;
            set
            {
                _cartItemsCount = value;
                OnPropertyChanged(nameof(CartItemsCount));
            }
        }

        public BuyerWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            Products = new ObservableCollection<ProductCard>();
            FilteredProducts = new ObservableCollection<ProductCard>();
            DataContext = this;
            LoadData();
            LoadUserData();
            LoadCartItemsCount();
            SetupProductsControl();
        }

        private void LoadUserData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            familiya_imya_otchestvo,
                            url_avatara
                        FROM CT_Polzovateli 
                        WHERE polzovatel_id = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserFullName = reader.GetString(0);
                                UserAvatarUrl = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void LoadCartItemsCount()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT COUNT(*)
                        FROM CT_Korzina
                        WHERE id_polzovatelya = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);
                        CartItemsCount = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке количества товаров в корзине: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void SetupProductsControl()
        {
            ProductsItemsControl.ItemsSource = FilteredProducts;
        }

        private void LoadData()
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
                            p.id_kategorii,
                            p.kolichestvo_tovarov,
                            p.url_izobrazheniya,
                            k.nazvanie as kategoria
                        FROM CT_Produkti p
                        INNER JOIN CT_Kategorii k ON p.id_kategorii = k.kategorii_id
                        ORDER BY p.nazvanie";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Products.Clear();
                            FilteredProducts.Clear();
                            while (reader.Read())
                            {
                                var product = new ProductCard
                                {
                                    ProduktId = reader.GetInt32(0),
                                    Nazvanie = reader.GetString(1),
                                    Opisanie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Tsena = reader.GetDecimal(3),
                                    IdKategorii = reader.GetInt32(4),
                                    KolichestvoTovarov = reader.GetInt32(5),
                                    UrlIzobrazheniya = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Kategoria = reader.GetString(7)
                                };
                                Products.Add(product);
                                FilteredProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authWindow = new AuthorizationWindow();
            authWindow.Show();
            this.Close();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем наличие товара
                    string checkQuery = @"
                        SELECT kolichestvo_tovarov 
                        FROM CT_Produkti 
                        WHERE produkt_id = @productId";

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@productId", product.ProduktId);
                        int availableQuantity = (int)checkCommand.ExecuteScalar();

                        if (availableQuantity <= 0)
                        {
                            MessageBox.Show("К сожалению, товар закончился на складе",
                                          "Ошибка",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Проверяем, есть ли уже этот товар в корзине
                    string cartCheckQuery = @"
                        SELECT kolichestvo 
                        FROM CT_Korzina 
                        WHERE id_polzovatelya = @userId 
                        AND id_produkta = @productId";

                    using (SqlCommand cartCheckCommand = new SqlCommand(cartCheckQuery, connection))
                    {
                        cartCheckCommand.Parameters.AddWithValue("@userId", _userId);
                        cartCheckCommand.Parameters.AddWithValue("@productId", product.ProduktId);

                        object result = cartCheckCommand.ExecuteScalar();

                        if (result != null)
                        {
                            // Товар уже есть в корзине, увеличиваем количество
                            string updateQuery = @"
                                UPDATE CT_Korzina 
                                SET kolichestvo = kolichestvo + 1 
                                WHERE id_polzovatelya = @userId 
                                AND id_produkta = @productId";

                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@userId", _userId);
                                updateCommand.Parameters.AddWithValue("@productId", product.ProduktId);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Добавляем новый товар в корзину
                            string insertQuery = @"
                                INSERT INTO CT_Korzina (id_polzovatelya, id_produkta, kolichestvo)
                                VALUES (@userId, @productId, 1)";

                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@userId", _userId);
                                insertCommand.Parameters.AddWithValue("@productId", product.ProduktId);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    CartItemsCount++;
                    MessageBox.Show($"Товар \"{product.Nazvanie}\" добавлен в корзину",
                                  "Успешно",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара в корзину: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            FilteredProducts.Clear();

            foreach (var product in Products)
            {
                if (product.Nazvanie.ToLower().Contains(searchText) ||
                    product.Opisanie.ToLower().Contains(searchText) ||
                    product.Kategoria.ToLower().Contains(searchText))
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow(_userId);
            cartWindow.Show();
        }

        private void ProfileImage_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userId);
            profileWindow.Owner = this;
            profileWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            profileWindow.ShowDialog();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
