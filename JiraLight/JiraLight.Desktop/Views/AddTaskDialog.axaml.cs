using Avalonia.Controls;
using System;

namespace JiraLight.Desktop.Views;


public partial class AddTaskDialog : UserControl
{
    private IDisposable? _saveSubscription;

    public AddTaskDialog()
    {
        InitializeComponent();
    }
}