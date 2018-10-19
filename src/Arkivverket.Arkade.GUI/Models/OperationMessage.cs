using System;
using System.Windows;
using System.Windows.Media;
using Arkivverket.Arkade.Core.Logging;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class OperationMessage : BindableBase, IComparable
    {
        private static readonly SolidColorBrush ColorError = new SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54));
        private static readonly SolidColorBrush ColorWarning = new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 150, 243));
        private static readonly SolidColorBrush ColorSuccess = new SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
        private const string IconError = "Alert";
        private const string IconWarning = "InformationOutline";
        private const string IconSuccess = "Check";

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
            if (status == OperationMessageStatus.Started)
                ShowProgressBar();
            else
            {
                ShowMessages();
                if (status == OperationMessageStatus.Error)
                {
                    ShowOperationAsError();
                }
                else if (status == OperationMessageStatus.Warning)
                {
                    ShowOperationAsWarning();
                }
                else
                {
                    ShowOperationAsSuccess();
                }
            }
        }

        private void ShowOperationAsSuccess()
        {
            Color = ColorSuccess;
            Label = Resources.GUI.TestrunnerMessageLabelSuccess;
            Icon = IconSuccess;
        }

        private void ShowOperationAsError()
        {
            Color = ColorError;
            Label = Resources.GUI.TestrunnerMessageLabelError;
            Icon = IconError;
        }

        private void ShowOperationAsWarning()
        {
            Color = ColorWarning;
            Label = Resources.GUI.TestrunnerMessageLabelWarning;
            Icon = IconWarning;
        }

        private void ShowProgressBar()
        {
            ProgressBarVisibility = Visibility.Visible;
            StatusVisibility = Visibility.Collapsed;
        }

        private void ShowMessages()
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