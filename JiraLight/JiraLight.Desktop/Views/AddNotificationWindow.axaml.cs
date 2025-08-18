using Avalonia.Controls;
using System.Threading.Tasks;

namespace JiraLight.Desktop;

public partial class AddNotificationWindow : Window
{
    public AddNotificationWindow()
    {
        InitializeComponent();
        this.Opened += async (_, _) => await ShowAndCloseAsync();
    }

    private async Task ShowAndCloseAsync()
    {
        // Плавное появление
        for (double i = 0; i <= 1; i += 0.05)
        {
            this.Opacity = i;
            await Task.Delay(15);
        }

        // Ждём 1.5 секунды
        await Task.Delay(1500);

        // Плавное исчезновение
        for (double i = 1; i >= 0; i -= 0.05)
        {
            this.Opacity = i;
            await Task.Delay(15);
        }

        Close();
    }
}