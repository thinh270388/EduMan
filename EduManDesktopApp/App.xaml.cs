using EduManDesktopApp.View;
using EduManDesktopApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EduManDesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IHost? AppHost {  get; private set; }
        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContent, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddTransient<LoginWindow>();
            }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.MouseDown += MainWindow_MouseDown;
            mainWindow.Show();

            var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();
            loginWindow.Owner = mainWindow;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.MouseDown += LoginWindow_MouseDown;
            loginWindow.ShowDialog();

            MainViewModel mainViewModel = new(AdUse.LoginUser);
            mainWindow.DataContext = mainViewModel;

            base.OnStartup(e);
        }

        private void LoginWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window? window = sender as Window;
            if (e.ButtonState == MouseButtonState.Pressed && e.ButtonState == e.LeftButton)
                window?.DragMove();
        }


        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window? window = sender as Window;
            if (e.ButtonState == MouseButtonState.Pressed && e.ButtonState == e.LeftButton)
                window?.DragMove();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
