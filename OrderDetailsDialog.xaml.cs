using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace Computer_Hardware_Strore
{
    public partial class OrderDetailsDialog : Window
    {
        private readonly string connectionString;
        private readonly int orderId;

        public OrderDetailsDialog(string connectionString, int orderId)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.orderId = orderId;

            // Добавляем обработчик для перетаскивания окна
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };

            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Загрузка основной информации о заказе
                    string orderQuery = @"
                        SELECT 
                            z.zakaz_id,
                            z.data_zakaza,
                            z.summa,
                            p.familiya_imya_otchestvo
                        FROM CT_Zakazi z
                        INNER JOIN CT_Polzovateli p ON z.id_polzovatelya = p.polzovatel_id
                        WHERE z.zakaz_id = @orderId";

                    using (SqlCommand command = new SqlCommand(orderQuery, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OrderNumberText.Text = $"Заказ №{reader.GetInt32(0)}";
                                OrderDateText.Text = $"Дата заказа: {reader.GetDateTime(1):dd.MM.yyyy HH:mm}";
                                TotalAmountText.Text = $"Сумма заказа: {reader.GetDecimal(2):C}";
                                CustomerText.Text = $"Покупатель: {reader.GetString(3)}";
                            }
                        }
                    }

                    // Загрузка деталей заказа
                    string detailsQuery = @"
                        SELECT 
                            p.nazvanie,
                            d.kolichestvo,
                            d.tsena,
                            d.kolichestvo * d.tsena as summa
                        FROM CT_DetaliZakaza d
                        INNER JOIN CT_Produkti p ON d.id_produkta = p.produkt_id
                        WHERE d.id_zakaza = @orderId";

                    using (SqlCommand command = new SqlCommand(detailsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var orderItems = new ObservableCollection<OrderItem>();
                            while (reader.Read())
                            {
                                orderItems.Add(new OrderItem
                                {
                                    Nazvanie = reader.GetString(0),
                                    Kolichestvo = reader.GetInt32(1),
                                    Tsena = reader.GetDecimal(2),
                                    Summa = reader.GetDecimal(3)
                                });
                            }
                            OrderItemsListView.ItemsSource = orderItems;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке деталей заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class OrderItem
    {
        public string Nazvanie { get; set; }
        public int Kolichestvo { get; set; }
        public decimal Tsena { get; set; }
        public decimal Summa { get; set; }
    }
}