using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Computer_Hardware_Strore
{
    public partial class ProfileWindow : Window, INotifyPropertyChanged
    {
        private readonly string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user122_db;password=user122;MultipleActiveResultSets=True;App=EntityFramework";
        private readonly int _userId;
        private string _fullName;
        private string _email;
        private string _phone;
        private string _address;
        private string _avatarUrl;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string AvatarUrl
        {
            get => _avatarUrl;
            set
            {
                _avatarUrl = value;
                OnPropertyChanged(nameof(AvatarUrl));
            }
        }

        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            DataContext = this;
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT familiya_imya_otchestvo, pochta, telefon, adres, url_avatara
                        FROM CT_Polzovateli
                        WHERE polzovatel_id = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                FullName = reader.GetString(0);
                                Email = reader.GetString(1);
                                Phone = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                AvatarUrl = reader.IsDBNull(4) ? "/Images/default_avatar.png" : reader.GetString(4);
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                            UPDATE CT_Polzovateli 
                            SET familiya_imya_otchestvo = @fullName,
                                pochta = @email,
                                telefon = @phone,
                                adres = @address,
                                url_avatara = @avatarUrl";

                        if (!string.IsNullOrEmpty(NewPasswordBox.Password))
                        {
                            query += ", parol = @password";
                        }

                        query += " WHERE polzovatel_id = @userId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@fullName", FullName);
                            command.Parameters.AddWithValue("@email", Email);
                            command.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(Phone) ? DBNull.Value : (object)Phone);
                            command.Parameters.AddWithValue("@address", string.IsNullOrEmpty(Address) ? DBNull.Value : (object)Address);
                            command.Parameters.AddWithValue("@avatarUrl", string.IsNullOrEmpty(AvatarUrl) ? DBNull.Value : (object)AvatarUrl);
                            command.Parameters.AddWithValue("@userId", _userId);

                            if (!string.IsNullOrEmpty(NewPasswordBox.Password))
                            {
                                command.Parameters.AddWithValue("@password", NewPasswordBox.Password);
                            }

                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Пожалуйста, введите ФИО", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Пожалуйста, введите email", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                if (string.IsNullOrEmpty(CurrentPasswordBox.Password))
                {
                    MessageBox.Show("Пожалуйста, введите текущий пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Новые пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Проверка текущего пароля
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT COUNT(*) FROM CT_Polzovateli WHERE polzovatel_id = @userId AND parol = @password";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@userId", _userId);
                            command.Parameters.AddWithValue("@password", CurrentPasswordBox.Password);

                            int count = (int)command.ExecuteScalar();
                            if (count == 0)
                            {
                                MessageBox.Show("Неверный текущий пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при проверке пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
}