using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Computer_Hardware_Strore
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private readonly string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user122_db;password=user122;MultipleActiveResultSets=True;App=EntityFramework";
        private readonly Random random = new Random();
        private readonly List<Line> gridLines = new List<Line>();
        private readonly DispatcherTimer animationTimer;

        public AuthorizationWindow()
        {
            InitializeComponent();
            CreateAnimatedGrid();

            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Добавляем эффект при наведении на кнопки
            MinimizeButton.MouseEnter += Button_MouseEnter;
            MaximizeButton.MouseEnter += Button_MouseEnter;
            CloseButton.MouseEnter += Button_MouseEnter;
        }

        private void CreateAnimatedGrid()
        {
            // Создаем сетку линий
            for (int i = 0; i < 20; i++)
            {
                var line = new Line
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(40, 0, 255, 157)),
                    StrokeThickness = 1,
                    X1 = random.Next(0, (int)AnimatedGrid.ActualWidth),
                    Y1 = random.Next(0, (int)AnimatedGrid.ActualHeight),
                    X2 = random.Next(0, (int)AnimatedGrid.ActualWidth),
                    Y2 = random.Next(0, (int)AnimatedGrid.ActualHeight)
                };

                AnimatedGrid.Children.Add(line);
                gridLines.Add(line);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            foreach (var line in gridLines)
            {
                // Анимируем каждую линию
                var animation = new DoubleAnimation
                {
                    To = random.Next(0, (int)AnimatedGrid.ActualWidth),
                    Duration = TimeSpan.FromSeconds(2),
                    EasingFunction = new QuadraticEase()
                };

                line.BeginAnimation(Line.X2Property, animation);

                animation = new DoubleAnimation
                {
                    To = random.Next(0, (int)AnimatedGrid.ActualHeight),
                    Duration = TimeSpan.FromSeconds(2),
                    EasingFunction = new QuadraticEase()
                };

                line.BeginAnimation(Line.Y2Property, animation);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = (Button)sender;
            var animation = new ColorAnimation
            {
                To = Colors.Red,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            if (button.Background is SolidColorBrush brush)
            {
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT polzovatel_id, rol FROM CT_Polzovateli WHERE pochta = @Email AND parol = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string role = reader.GetString(1);

                                Window nextWindow;
                                if (role == "Администратор")
                                {
                                    nextWindow = new AdministratorWindow(userId);
                                }
                                else
                                {
                                    nextWindow = new BuyerWindow(userId);
                                }

                                nextWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Неверный email или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }
    }
}
