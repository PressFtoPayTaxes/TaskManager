using System;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Models1;
using System.Windows.Forms;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            operationTypeComboBox.SelectedIndex = 0;

            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon("Oxygen-Icons.org-Oxygen-Actions-view-calendar-tasks.ico");
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
            _notifyIcon.Visible = false;
            _notifyIcon.Text = "Runing...";
            _notifyIcon.MouseClick += NotifyIconMouseClick;
        }

        delegate void MailDelegate(MailMessage mail);
        NotifyIcon _notifyIcon = new NotifyIcon();
        List<System.Threading.Timer> _timers = new List<System.Threading.Timer>();
        List<System.Threading.Tasks.Task> _tasksWithDelay = new List<System.Threading.Tasks.Task>();

        private void NotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            Show();
            _notifyIcon.Visible = false;
        }

        private void OperationTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (operationTypeComboBox.SelectedIndex)
            {
                case 0:
                    sendEmailGroupBox.Visibility = Visibility.Visible;
                    downloadFileGroupBox.Visibility = Visibility.Collapsed;
                    replaceDirectoryGroupBox.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    sendEmailGroupBox.Visibility = Visibility.Collapsed;
                    downloadFileGroupBox.Visibility = Visibility.Visible;
                    replaceDirectoryGroupBox.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    sendEmailGroupBox.Visibility = Visibility.Collapsed;
                    downloadFileGroupBox.Visibility = Visibility.Collapsed;
                    replaceDirectoryGroupBox.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private async void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate == null || datePicker.SelectedDate < DateTime.Now || taskNameTextBox.Text == null || taskNameTextBox.Text == string.Empty || periodComboBox.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Неправильно заполнены поля");
                return;
            }

            if (operationTypeComboBox.SelectedIndex == 0 && (receiverTextBox.Text == null || receiverTextBox.Text == string.Empty || themeTextBox.Text == null || themeTextBox.Text == string.Empty || mailTextBox.Text == null || mailTextBox.Text == string.Empty))
            {
                System.Windows.MessageBox.Show("Неправильно заполнены поля");
                return;
            }

            if (operationTypeComboBox.SelectedIndex == 1 && (fileUrlTextBox.Text == null || fileUrlTextBox.Text == string.Empty))
            {
                System.Windows.MessageBox.Show("Неправильно заполнены поля");
                return;
            }

            if (operationTypeComboBox.SelectedIndex == 2 && (primordialFolderTextBox.Text == null || primordialFolderTextBox.Text == string.Empty || destinationFolderTextBox.Text == null || destinationFolderTextBox.Text == string.Empty))
            {
                System.Windows.MessageBox.Show("Неправильно заполнены поля");
                return;
            }

            using (var context = new DataContext())
            {
                var task = new Models1.Task
                {
                    Name = taskNameTextBox.Text,
                    Date = (DateTime)datePicker.SelectedDate,
                    Period = (Period)periodComboBox.SelectedIndex
                };

                context.Tasks.Add(task);

                long periodInMilliseconds = 0;

                long millisecondsInDay = 24 * 60 * 60 * 100;
                long millisecondsInWeek = millisecondsInDay * 7;
                long millisecondsInMonth = millisecondsInWeek * 30;
                long millisecondsInYear = millisecondsInMonth * 12;

                switch (task.Period)
                {
                    case Period.Once:
                        break;
                    case Period.OnceADay:
                        periodInMilliseconds = millisecondsInDay;
                        break;
                    case Period.OnceAWeek:
                        periodInMilliseconds = millisecondsInWeek;
                        break;
                    case Period.OnceAMonth:
                        periodInMilliseconds = millisecondsInMonth;
                        break;
                    case Period.OnceAYear:
                        periodInMilliseconds = millisecondsInYear;
                        break;
                    default:
                        break;
                }

                switch (operationTypeComboBox.SelectedIndex)
                {
                    case 0:
                        var emailTask = new EmailSendingTask
                        {
                            MailReceiver = receiverTextBox.Text,
                            MailTheme = themeTextBox.Text,
                            MailText = mailTextBox.Text,
                            Task = task
                        };
                        context.EmailSendingTasks.Add(emailTask);

                        if (periodInMilliseconds == 0)
                        {
                            System.Threading.Tasks.Task taskWithDelay = new System.Threading.Tasks.Task(SendEmail, null);
                            await System.Threading.Tasks.Task.Delay(task.Date.TimeOfDay);
                        }
                        else
                        {
                            _timers.Add(new System.Threading.Timer(SendEmail, null, task.Date.Subtract(DateTime.Now).Milliseconds, (int)periodInMilliseconds));

                        }
                        break;
                    case 1:
                        var downloadTask = new FileDownloadingTask
                        {
                            FileUrl = fileUrlTextBox.Text,
                            Task = task
                        };
                        context.FileDownloadingTasks.Add(downloadTask);
                        break;
                    case 2:
                        var directoryTask = new DirectoryReplacingTask
                        {
                            PrimordialFolder = primordialFolderTextBox.Text,
                            DestinationFolder = destinationFolderTextBox.Text,
                            Task = task
                        };
                        context.DirectoryReplacingTasks.Add(directoryTask);
                        break;
                }

                await System.Threading.Tasks.Task.Run(() => context.SaveChanges());
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            _notifyIcon.Visible = true;
            e.Cancel = true;
        }

        private void SendEmail(object obj)
        {
            MailMessage mail = new MailMessage("igor_2002_25@mail.ru", receiverTextBox.Text);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.mail.ru";
            mail.Subject = themeTextBox.Text;
            mail.Body = mailTextBox.Text;

            MailDelegate mailDelegate = client.Send;
            try
            {
                mailDelegate.BeginInvoke(mail, null, null);
            }
            catch (SmtpFailedRecipientsException exception)
            {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = exception.Message;
                _notifyIcon.ShowBalloonTip(2000);
            }
            catch (SmtpException exception)
            {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = exception.Message;
                _notifyIcon.ShowBalloonTip(2000);
            }
            catch (Exception exception)
            {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = exception.Message;
                _notifyIcon.ShowBalloonTip(2000);
            }
        }

        public void DownloadFile(object obj)
        {
            var appDomain = AppDomain.CreateDomain("FileDownloadDomain");

            try
            {
                appDomain.ExecuteAssembly("FileDownloadApp.exe", new string[] { fileUrlTextBox.Text });
            }
            catch (FileNotFoundException exception)
            {
                _notifyIcon.BalloonTipTitle = "Ошибка";
                _notifyIcon.BalloonTipText = exception.Message;
                _notifyIcon.ShowBalloonTip(2000);
            }

            AppDomain.Unload(appDomain);
        }

        private object lockObject = new object();
        public void ReplaceDirectory(object obj)
        {
            var primordialDirectory = primordialFolderTextBox.Text;
            var destinationDirectory = destinationFolderTextBox.Text;

            lock (lockObject)
            {
                try
                {
                    Directory.Move(primordialDirectory, destinationDirectory);
                }
                catch (DirectoryNotFoundException exception)
                {
                    _notifyIcon.BalloonTipTitle = "Ошибка";
                    _notifyIcon.BalloonTipText = exception.Message;
                    _notifyIcon.ShowBalloonTip(2000);
                }
                catch (UnauthorizedAccessException exception)
                {
                    _notifyIcon.BalloonTipTitle = "Ошибка";
                    _notifyIcon.BalloonTipText = exception.Message;
                    _notifyIcon.ShowBalloonTip(2000);
                }
                catch (Exception exception)
                {
                    _notifyIcon.BalloonTipTitle = "Ошибка";
                    _notifyIcon.BalloonTipText = exception.Message;
                    _notifyIcon.ShowBalloonTip(2000);
                }
            }
        }

        private void TasksButtonClick(object sender, RoutedEventArgs e)
        {
            TasksWindow window = new TasksWindow();
            window.Show();
        }
    }
}
