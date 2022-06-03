using System;
using System.Windows;
using System.Windows.Media;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.GUI.Languages;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class ArchiveFormatValidationStatusDisplay : BindableBase
    {
        public ArchiveFormatValidationStatusDisplay()
        {
            Reset();
        }

        private string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private Visibility _progressBarVisibility;

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetProperty(ref _progressBarVisibility, value);
        }

        private Visibility _resultIconVisibility;

        public Visibility ResultIconVisibility
        {
            get => _resultIconVisibility;
            set => SetProperty(ref _resultIconVisibility, value);
        }

        private string _resultIconKind;

        public string ResultIconKind
        {
            get => _resultIconKind;
            set => SetProperty(ref _resultIconKind, value);
        }

        private SolidColorBrush _resultIconColor;

        public SolidColorBrush ResultIconColor
        {
            get => _resultIconColor;
            set => SetProperty(ref _resultIconColor, value);
        }

        public void DisplayRunning()
        {
            Reset();
            ProgressBarVisibility = Visibility.Visible;
            StatusMessage = ToolsGUI.ArchiveFormatValidationRunningStatusMessage;
        }

        public void DisplayFinished(ArchiveFormatValidationReport validationReport)
        {
            Reset();
            ConfigureIconByValidationResult(validationReport);
            ResultIconVisibility = Visibility.Visible;
            StatusMessage = validationReport.ValidationSummary();
        }

        public void Reset()
        {
            StatusMessage = string.Empty;
            ResultIconVisibility = Visibility.Collapsed;
            ProgressBarVisibility = Visibility.Collapsed;
        }

        private void ConfigureIconByValidationResult(ArchiveFormatValidationReport result)
        {
            (ResultIconKind, ResultIconColor) = result.ValidationResult switch
            {
                ArchiveFormatValidationResult.Valid =>
                    ("CheckBold", new SolidColorBrush(Colors.Teal)),

                ArchiveFormatValidationResult.Invalid when result.IsAcceptable =>
                    ("Information", new SolidColorBrush(Colors.RoyalBlue)),

                ArchiveFormatValidationResult.Invalid =>
                    ("MinusCircleOutline", new SolidColorBrush(Colors.DarkRed)),

                ArchiveFormatValidationResult.Error =>
                    ("CloseBold", new SolidColorBrush(Colors.DimGray)),

                _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };
        }
    }
}
