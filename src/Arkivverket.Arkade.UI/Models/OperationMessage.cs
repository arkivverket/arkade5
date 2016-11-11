using System;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Arkivverket.Arkade.Logging;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.Models
{
    public class OperationMessage : BindableBase, IComparable
    {
        private readonly SolidColorBrush _colorFailed = new SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54));

        private readonly SolidColorBrush _colorSuccess = new SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));

        private string _message;

        private Visibility _progressBarVisibility = Visibility.Visible;

        private SolidColorBrush _color;

        private string _icon;

        private string _label;

        public string Id { get; set; }

        private Visibility _statusVisibility = Visibility.Collapsed;

        public DateTime Updated { get; set; }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set { SetProperty(ref _progressBarVisibility, value); }
        }

        public Visibility StatusVisibility
        {
            get { return _statusVisibility; }
            set { SetProperty(ref _statusVisibility, value); }
        }


        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public SolidColorBrush Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        public OperationMessage(OperationMessageEventArgs operationMessageEventArgs)
        {
            Id = operationMessageEventArgs.Id;
            Updated = DateTime.Now;
            Update(operationMessageEventArgs);
        }

        public void Update(OperationMessageEventArgs operationMessageEventArgs)
        {
            Message = operationMessageEventArgs.Message;
            UpdateStatus(operationMessageEventArgs.Status);
        }

        public void UpdateStatus(OperationMessageStatus status)
        {
            if (status == OperationMessageStatus.Ok || status == OperationMessageStatus.Error)
            {
                ShowTestResults();
                if (status == OperationMessageStatus.Error)
                {
                    ShowOperationAsError();
                }
                else
                {
                    ShowOperationAsSuccess();
                }
            }
            else
            {
                ShowProgressBar();
            }
        }

        private void ShowOperationAsSuccess()
        {
            Color = _colorSuccess;
            Label = "OK";
            Icon = "Check";
        }

        private void ShowOperationAsError()
        {
            Color = _colorFailed;
            Label = "Feil";
            Icon = "Alert";
        }

        private void ShowProgressBar()
        {
            ProgressBarVisibility = Visibility.Visible;
            StatusVisibility = Visibility.Collapsed;
        }

        private void ShowTestResults()
        {
            StatusVisibility = Visibility.Visible;
            ProgressBarVisibility = Visibility.Collapsed;
        }

        public int CompareTo(object other)
        {
            OperationMessage otherOperationMessage = (OperationMessage) other;
            return otherOperationMessage.Updated.CompareTo(Updated);
        }
    }
}