using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Computer_Hardware_Strore
{
    public partial class CartWindow : Window, INotifyPropertyChanged
    {
        private readonly string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user122_db;password=user122;MultipleActiveResultSets=True;App=EntityFramework";
        private readonly int _userId;
        private decimal _totalAmount;
        private ObservableCollection<CartItem> CartItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public CartWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            CartItems = new ObservableCollection<CartItem>();
            DataContext = this;
            CartItemsControl.ItemsSource = CartItems;
            LoadCartItems();
        }

        private void LoadCartItems()
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
                            p.url_izobrazheniya,
                            kr.kolichestvo as quantity_in_cart
                        FROM CT_Korzina kr
                        JOIN CT_Produkti p ON kr.id_produkta = p.produkt_id
                        JOIN CT_Kategorii k ON p.id_kategorii = k.kategorii_id
                        WHERE kr.id_polzovatelya = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            CartItems.Clear();
                            while (reader.Read())
                            {
                                var item = new CartItem
                                {
                                    ProduktId = reader.GetInt32(0),
                                    Nazvanie = reader.GetString(1),
                                    Opisanie = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Tsena = reader.GetDecimal(3),
                                    KolichestvoTovarov = reader.GetInt32(4),
                                    Kategoria = reader.GetString(5),
                                    UrlIzobrazheniya = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Quantity = reader.GetInt32(7)
                                };
                                CartItems.Add(item);
                            }
                        }
                    }
                }

                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке корзины: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void UpdateTotalAmount()
        {
            TotalAmount = CartItems.Sum(item => item.Tsena * item.Quantity);
        }

        private void IncrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((FrameworkElement)sender).DataContext;
            if (item.Quantity < item.KolichestvoTovarov)
            {
                item.Quantity++;
                UpdateCartItemQuantity(item);
                UpdateTotalAmount();
            }
        }

        private void DecrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((FrameworkElement)sender).DataContext;
            if (item.Quantity > 1)
            {
                item.Quantity--;
                UpdateCartItemQuantity(item);
                UpdateTotalAmount();
            }
        }

        private void UpdateCartItemQuantity(CartItem item)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE CT_Korzina 
                        SET kolichestvo = @quantity
                        WHERE id_polzovatelya = @userId 
                        AND id_produkta = @productId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@quantity", item.Quantity);
                        command.Parameters.AddWithValue("@userId", _userId);
                        command.Parameters.AddWithValue("@productId", item.ProduktId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении количества: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                LoadCartItems();
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((FrameworkElement)sender).DataContext;
            var result = MessageBox.Show("Удалить товар из корзины?",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                            DELETE FROM CT_Korzina
                            WHERE id_polzovatelya = @userId 
                            AND id_produkta = @productId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@userId", _userId);
                            command.Parameters.AddWithValue("@productId", item.ProduktId);
                            command.ExecuteNonQuery();
                        }
                    }

                    CartItems.Remove(item);
                    UpdateTotalAmount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Оформить заказ на сумму {TotalAmount:N0} ₽?",
                                       "Подтверждение заказа",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Создаем новый заказ
                        string createOrderQuery = @"
                            INSERT INTO CT_Zakazi (id_polzovatelya, data_zakaza, summa)
                            VALUES (@userId, GETDATE(), @totalAmount);
                            SELECT SCOPE_IDENTITY();";

                        int orderId;
                        using (SqlCommand command = new SqlCommand(createOrderQuery, connection))
                        {
                            command.Parameters.AddWithValue("@userId", _userId);
                            command.Parameters.AddWithValue("@totalAmount", TotalAmount);
                            orderId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Добавляем детали заказа
                        foreach (var item in CartItems)
                        {
                            string addDetailsQuery = @"
                                INSERT INTO CT_DetaliZakaza (id_zakaza, id_produkta, kolichestvo, tsena)
                                VALUES (@orderId, @productId, @quantity, @price)";

                            using (SqlCommand command = new SqlCommand(addDetailsQuery, connection))
                            {
                                command.Parameters.AddWithValue("@orderId", orderId);
                                command.Parameters.AddWithValue("@productId", item.ProduktId);
                                command.Parameters.AddWithValue("@quantity", item.Quantity);
                                command.Parameters.AddWithValue("@price", item.Tsena);
                                command.ExecuteNonQuery();
                            }

                            // Обновляем количество товара на складе
                            string updateStockQuery = @"
                                UPDATE CT_Produkti
                                SET kolichestvo_tovarov = kolichestvo_tovarov - @quantity
                                WHERE produkt_id = @productId";

                            using (SqlCommand command = new SqlCommand(updateStockQuery, connection))
                            {
                                command.Parameters.AddWithValue("@quantity", item.Quantity);
                                command.Parameters.AddWithValue("@productId", item.ProduktId);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    CartItems.Clear();
                    UpdateTotalAmount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CartItem : INotifyPropertyChanged
    {
        public int ProduktId { get; set; }
        public string Nazvanie { get; set; }
        public string Opisanie { get; set; }
        public decimal Tsena { get; set; }
        public int KolichestvoTovarov { get; set; }
        public string Kategoria { get; set; }
        public string UrlIzobrazheniya { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}