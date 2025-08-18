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
        // ������� ���������
        for (double i = 0; i <= 1; i += 0.05)
        {
            this.Opacity = i;
            await Task.Delay(15);
        }

        // ��� 1.5 �������
        await Task.Delay(1500);

        // ������� ������������
        for (double i = 1; i >= 0; i -= 0.05)
        {
            this.Opacity = i;
            await Task.Delay(15);
        }

        Close();
    }
}