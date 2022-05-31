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

        public void DisplayFinished(ArchiveFormatValidationResult validationResult)
        {
            Reset();
            ConfigureIconByValidationResult(validationResult.ValidationResult);
            ResultIconVisibility = Visibility.Visible;
            StatusMessage = validationResult.ValidationSummary();
        }

        public void Reset()
        {
            StatusMessage = string.Empty;
            ResultIconVisibility = Visibility.Collapsed;
            ProgressBarVisibility = Visibility.Collapsed;
        }

        private void ConfigureIconByValidationResult(ArchiveFormatValidationResultType result)
        {
            switch (result)
            {
                case ArchiveFormatValidationResultType.Valid:
                {
                    ResultIconKind = "CheckBold";
                    ResultIconColor = new SolidColorBrush(Colors.Teal);
                    break;
                }
                case ArchiveFormatValidationResultType.Invalid:
                {
                    ResultIconKind = "MinusCircleOutline";
                    ResultIconColor = new SolidColorBrush(Colors.DarkRed);
                    break;
                }
                case ArchiveFormatValidationResultType.Error:
                {
                    ResultIconKind = "CloseBold";
                    ResultIconColor = new SolidColorBrush(Colors.DimGray);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}
