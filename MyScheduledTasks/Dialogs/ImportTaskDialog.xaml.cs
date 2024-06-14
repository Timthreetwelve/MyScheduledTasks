// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

public partial class ImportTaskDialog : UserControl
{
    public ImportTaskDialog()
    {
        InitializeComponent();
    }

    internal static void FilePicker()
    {
        OpenFileDialog dlgOpen = new()
        {
            Title = GetStringResource("ImportTask_FilePickerTitle"),
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "XML files (*.xml)|*.xml"
        };
        bool? result = dlgOpen.ShowDialog();
        if (result == true)
        {
            TempSettings.Setting!.ImportXMLFile = dlgOpen.FileName;
        }
    }
}
