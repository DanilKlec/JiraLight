using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using JiraLight.Desktop.Models;
using JiraLight.Desktop.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace JiraLight.Desktop.Views;

public partial class DashboardPage : UserControl
{
    private TaskModel _draggedTask;
    private ObservableCollection<TaskModel> _sourceCollection;
    private Border _dragVisual;
    private DateTime _lastClickTime;
    private const int DoubleClickThresholdMs = 5000;

    public DashboardPage()
    {
        InitializeComponent();
    }

    private void Task_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is TaskModel task)
        {
            var now = DateTime.Now;
            if ((now - _lastClickTime).TotalMilliseconds < DoubleClickThresholdMs)
            {
                // Двойной клик
                OpenTaskPage(task);
                _lastClickTime = DateTime.MinValue;
                return; // не начинаем Drag
            }
            _lastClickTime = now;

            // Старт Drag
            StartDrag(border, task, e);
        }
    }

    private void StartDrag(Border border, TaskModel task, PointerPressedEventArgs e)
    {
        if (DataContext is DashboardViewModel vm)
        {
            _draggedTask = task;
            _sourceCollection = vm.ToDoTasks.Contains(task) ? vm.ToDoTasks :
                                vm.InProgressTasks.Contains(task) ? vm.InProgressTasks :
                                vm.DoneTasks.Contains(task) ? vm.DoneTasks : null;

            _dragVisual = new Border
            {
                Width = border.Bounds.Width,
                Height = border.Bounds.Height,
                Background = new SolidColorBrush(Colors.LightGray, 0.7),
                CornerRadius = border.CornerRadius,
                Child = new TextBlock
                {
                    Text = task.Title,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize = 16,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            };
            DragCanvas.Children.Add(_dragVisual);
            UpdateDragVisualPosition(e.GetPosition(DragCanvas));
        }
    }

    private void Task_PointerMoved(object sender, PointerEventArgs e)
    {
        if (_draggedTask != null && _dragVisual != null)
            UpdateDragVisualPosition(e.GetPosition(DragCanvas));
    }

    private void Task_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (_draggedTask == null) return;

        var pos = e.GetPosition(this);
        Border targetColumn = null;

        if (IsPointerOver(ToDoColumn, pos)) targetColumn = ToDoColumn;
        else if (IsPointerOver(InProgressColumn, pos)) targetColumn = InProgressColumn;
        else if (IsPointerOver(DoneColumn, pos)) targetColumn = DoneColumn;

        if (targetColumn != null && _sourceCollection != null && this.DataContext is DashboardViewModel vm)
        {
            var targetCollection = targetColumn.Name switch
            {
                "ToDoColumn" => vm.ToDoTasks,
                "InProgressColumn" => vm.InProgressTasks,
                "DoneColumn" => vm.DoneTasks,
                _ => null
            };

            if (targetCollection != null)
            {
                TaskStatus newStatus = targetColumn.Name switch
                {
                    "ToDoColumn" => TaskStatus.ToDo,
                    "InProgressColumn" => TaskStatus.InProgress,
                    "DoneColumn" => TaskStatus.Done,
                    _ => TaskStatus.ToDo
                };

                vm.MoveTask(_draggedTask, _sourceCollection, targetCollection, newStatus);
            }
        }

        DragCanvas.Children.Remove(_dragVisual);
        _draggedTask = null;
        _sourceCollection = null;
        _dragVisual = null;
    }

    private void UpdateDragVisualPosition(Avalonia.Point p)
    {
        if (_dragVisual != null)
        {
            Canvas.SetLeft(_dragVisual, p.X - _dragVisual.Width / 2);
            Canvas.SetTop(_dragVisual, p.Y - _dragVisual.Height / 2);
        }
    }

    private bool IsPointerOver(Border column, Avalonia.Point pointer)
    {
        var bounds = column.Bounds;
        var topLeft = column.TranslatePoint(new Avalonia.Point(0, 0), this) ?? new Avalonia.Point();
        var rect = new Avalonia.Rect(topLeft, bounds.Size);
        return rect.Contains(pointer);
    }

    private void OpenTaskPage(TaskModel task)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
        && DataContext is DashboardViewModel dashboard)
        {
            var mainVm = (MainViewModel)desktop.MainWindow.DataContext;

            var detailVm = new TaskDetailViewModel(task, dashboard, () =>
            {
                // goBack: возвращаемся на Dashboard
                mainVm.CurrentPage = dashboard;
            });

            mainVm.CurrentPage = new TaskDetailPage { DataContext = detailVm };
        }
    }
}
