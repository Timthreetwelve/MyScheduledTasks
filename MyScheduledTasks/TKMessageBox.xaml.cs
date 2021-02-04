// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TKUtils
{
    // Based on https://www.codeproject.com/tips/1081930/custom-message-box-in-wpf-xaml
    //
    // Remember to copy the 8 png images to the Images folder

    public sealed partial class TKMessageBox : Window
    {
        private TKMessageBox()
        {
            InitializeComponent();
            if (Application.Current.MainWindow.IsVisible)
            {
                Owner = GetWindow(Application.Current.MainWindow);
            }
        }

        #region Private backing fields
        private static TKMessageBox _messageBox;
        private static MessageBoxResult _result = MessageBoxResult.No;
        #endregion

        #region MessageBoxResult
        public static MessageBoxResult Show(string msg, string caption, MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(msg, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                case MessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(msg, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                case MessageBoxType.Information:
                    return Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Information);

                case MessageBoxType.Error:
                    return Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                case MessageBoxType.Warning:
                    return Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Warning);

                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageBoxType type)
        {
            return Show(msg, string.Empty, type);
        }

        public static MessageBoxResult Show(string msg)
        {
            return Show(msg, string.Empty, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string text, string caption, MessageBoxButton button)
        {
            return Show(text, caption, button, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            _messageBox = new TKMessageBox
            { txtMsg = { Text = text }, MessageTitle = { Text = caption } };
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            _messageBox.ShowDialog();
            return _result;
        }
        #endregion

        #region Button Visibility
        private static void SetVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnOk.Focus();
                    break;

                case MessageBoxButton.OKCancel:
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;

                case MessageBoxButton.YesNo:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Focus();
                    break;

                case MessageBoxButton.YesNoCancel:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region MessageBox Image
        private static void SetImageOfMessageBox(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    _messageBox.SetImage("Warning.png");
                    break;

                case MessageBoxImage.Question:
                    _messageBox.SetImage("Question.png");
                    break;

                case MessageBoxImage.Information:
                    _messageBox.SetImage("Information.png");
                    break;

                case MessageBoxImage.Error:
                    _messageBox.SetImage("Error.png");
                    break;

                case MessageBoxImage.ArrowCircle:
                    _messageBox.SetImage("ArrowCircle.png");
                    break;

                case MessageBoxImage.Check:
                    _messageBox.SetImage("Check.png");
                    break;

                case MessageBoxImage.FileSave:
                    _messageBox.SetImage("FileSave.png");
                    break;

                default:
                    _messageBox.msgBoxImage.Visibility = Visibility.Collapsed;
                    break;
            }
            if (_messageBox.msgBoxImage.Visibility == Visibility.Collapsed)
            {
                _messageBox.ImageGrid.Margin = new Thickness(40, 0, 0, 0);
            }
        }
        #endregion

        #region Button Click Event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                _result = MessageBoxResult.OK;
            else if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            else if (sender == btnCancel)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;
            (_messageBox ?? (_messageBox = this)).Close();
            _messageBox = null;
        }
        #endregion

        #region Pick Image (when needed)
        private void SetImage(string imageName)
        {
            string uri = string.Format("pack://application:,,,/Images/{0}", imageName);
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            msgBoxImage.Source = new BitmapImage(uriSource);
        }
        #endregion

        #region Move the MessageBox window
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }

    #region Enums
    public enum MessageBoxType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum MessageBoxImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        Check,
        ArrowCircle,
        FileSave,
        None
    }
    #endregion
}
