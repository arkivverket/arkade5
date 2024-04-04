using Arkivverket.Arkade.GUI.Models;
using System.Windows;
using System;
using static Arkivverket.Arkade.GUI.Languages.GUI;

namespace Arkivverket.Arkade.GUI.Util
{
    internal static class UserDialogs
    {
        public static bool UserConfirmsShutDown()
        {
            string warningMessage = ArkadeProcessingState.TestingIsStarted && !ArkadeProcessingState.PackingIsFinished
                ? TestResultsAndOtherAddedDataLostWarning + Environment.NewLine + ApplicationShutdownConfirmDialogText
                : ApplicationShutdownConfirmDialogText;

            bool userConfirmsShutdown = MessageBox.Show(warningMessage, ApplicationShutdownConfirmDialogCaption,
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;

            return userConfirmsShutdown;
        }
    }
}
