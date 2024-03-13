The My Scheduled Tasks ReadMe File


Introduction
============
My Scheduled Tasks is an application that lets you keep track of the tasks in Windows Task Scheduler that
you care about. My Scheduled Tasks can be automated to let you know if any of the tasks completed with
a non-zero exit code. It's important to note that while My Scheduled Tasks allows you an simple way to
check on scheduled tasks, it doesn't allow you to change existing tasks or create new tasks. For those
activities you will need to use Windows Task Scheduler.


Changes from Previous Versions
==============================
Starting in version 0.6.0, My Scheduled Tasks can import tasks from an XML file and delete tasks from
Windows Task Scheduler. The Import and Delete options are located under the "Advanced" menu which is
hidden by default. You should only use these option if you understand the implications of doing so. I
will not be held responsible if you use these options. Please use them wisely. The Advanced menu can
be enabled in Settings -> Application Settings.

Also starting in version 0.6.0, My Scheduled Tasks will always run as Administrator. Depending on your
User Account Control (UAC) settings, you may get a UAC dialog. If a UAC prompt is shown, you will need
to click "Yes" in order to use My Scheduled Tasks. This change was made because My Scheduled Tasks needs
to run elevated to be able to see all of the tasks in Windows Task Scheduler and to perform functions
such as enabling, disabling, running, and deleting tasks.


Navigation
==========
Use the menu on the left for page navigation or to exit the application.


Getting Started
===============
When you first start My Scheduled Tasks, you will see a pop up asking if you would like to add tasks now.
If you select "Yes" you will be taken to the Add Tasks page. From there you can select the desired tasks
and then click the Add Tasks button. More on adding tasks later.


Using My Scheduled Tasks
========================
After adding tasks, you will notice that there are several columns in the grid. The first column,
the one with the ⚠ (warning emoji), has a checkbox. Check the checkbox if you are going to automate
My Scheduled Tasks and you want to be alerted if the task in this row had a non-zero result from it's
last scheduled run. If you don't plan to automate My Scheduled Tasks, you can hide this column.

The next column is the task name as seen in Windows Task Scheduler.

The next column shows the last result. It shows "OK" is the last result was zero or "NZ" in red for a
non-zero result. "No Info" is shown if no information for this task was found in Windows Task Scheduler.
The actual result code (in hexadecimal) will be shown when the mouse cursor is over a value in this column.

Other possible results are:

    AR  - An instance of this task is already running
    DIS - The task has been disabled
    FNF - File not found
    NMR - There are no more runs scheduled for this task
    NYR - The task has not yet run
    RDY - Task is ready to run at its next scheduled time
    RUN - The task is currently running
    TRM - The last run of the task was terminated by the user

The next column shows the status of the task. If the task is currently running, the text will be green.
If the task is disabled, the text will be gray.

The next column shows the date and time of the last run.

The next column shows the date and time of the next run.

The next column shows the folder in Windows Task Scheduler where the task is located.

The last column is the Notes column. You may enter a note directly into this field.

With the exception of the Task Name and Result columns, any of the columns may be hidden by unchecking
them by going to Settings -> Select Grid Columns.


Details Pane
==================
For additional details on any task select a task in the grid. If the details pane is not visible,
open the View menu and select Toggle Details Pane or press CTRL+D to toggle the details pane. Details
are only displayed when exactly one task is selected in the grid at the top.


Adding Tasks
============
Adding tasks can be done when My Scheduled tasks is started for the first time as discussed in the
Getting Started section. Tasks can be added at any time by selecting Add Tasks from the navigation menu.
At the top of the page you will see a box that can filter the list of tasks. This filter will display
only the tasks whose task name matches the text typed into the filter box. To the right of the filter
box is a button that will hide all tasks that are in the \Microsoft folder and its sub-folders.

Select a task to add to your list by clicking on it. Standard Windows selection rules apply, hold the
Ctrl key down to select more than one task, hold the shift key down to select a range. When you have
selected the tasks that you want, click the Add Tasks button. When you are finished adding tasks,
navigate back to the View Tasks page by clicking View Tasks in the navigation menu on the left side.

Tasks are added to your list at the bottom. To rearrange the list, navigate to the View Tasks page
and drag the task and drop it in the desired location in the grid.


Removing Tasks
==============
You can remove tasks by right-clicking the appropriate row and selecting Remove Selected or by using
the Remove Selected item on the My Tasks menu. Multiple rows can be deleted if needed. Note that this
does not remove any tasks from Windows Task Scheduler, only from your list of tasks.


Task Notes
==========
You can add a note to any task. For example, you can use a note to associate a vaguely named task with
its application. Or you could have a note reminding not to disable an essential task. Notes can be edited
by selecting the note field in the grid or by selecting Edit Task Note from the My Tasks menu or the
context menu.


The Advanced Menu
=================
The Advanced menu has selections that may potentially break things. As a result, the Advanced menu is by
default hidden. Go to Settings > Application Settings, select Show Advanced Menu, then tick the option to
activate the menu. In addition to the Advanced menu appearing, the context (right-click) menu gains new
options when the Advanced Menu is enabled. Please, ONLY ENABLE THE ADVANCED MENU IF YOU KNOW WHAT YOU ARE DOING!

The following sections describe the items on the Advanced Menu.


Importing Tasks
===============
You can import a task by selecting Import Task from the Advanced menu. Enter the path to the task XML
file in the textbox at the top. The icon to the right of this box opens a file picker.

Enter the task name in the next textbox. This name should start with the folder (if any) and the task
name must be unique within that folder. This name will override the URI in the XML file.

Next is an option to run the task only when the user is logged on. If you uncheck this box, you should
verify the Security Options for the task via Windows Task Scheduler.

Next is an option to reset the task creation date and time to the current date and time. This will override
the Date in the XML file.

Lastly, there is an option to add the task to your My Tasks list after the import is successful.

Click the Import button to import the task. When you are finished importing tasks, click the Cancel button
to dismiss the dialog.


Exporting Tasks
===============
You can export a task by right-clicking the appropriate row and selecting Export Selected or by using
the Export Selected item on the Advanced menu. Tasks are exported in XML format. Multiple tasks may be
exported by selecting more than one before selecting Export Selected.


Enable, Disable or Run Tasks
============================
Tasks can be enabled, disabled, or executed via the context menu or the Advanced menu. These options
should be self-explanatory. The task status and last run will be updated as appropriate. In some
cases, it may be necessary to refresh the list by pressing F5 or clicking the refresh button.


Deleting Tasks
==============
Tasks may be deleted via the context menu or the Advanced menu. In this context it is important not to
confuse Removing, as mentioned earlier, with Deleting. Removing a task simply removes it from your list.
Deleting a task deletes it from the Windows Task Scheduler. This action is permanent and irreversible.
Consequently, it is recommended that tasks be exported before being deleted. Better yet, disable the
task instead of deleting it. In any case, approach with caution and consider the hazards involved with
disabling or deleting a task.


Saving your Task List
=====================
The tasks that you have selected are saved in a file named MyTasks.json in the application's folder.
This file is automatically saved when a task is added to or removed from the list, when an alert
checkbox is changed, when a task note is changed, or when the order of tasks is changed. You can also
save the file by selecting the Save Task List File option in the File menu, or by pressing Ctrl+S.


Copying Text
============
Text in the Details pane can be copied to the clipboard by right-clicking on it. No need to highlight
the text, simply right-click. A confirmation message will be shown in the upper right.


Automating My Scheduled Tasks
=============================
You can run My Scheduled Tasks on a regular schedule by adding a task to Windows Task Scheduler,
specifying the path to the My Scheduled Tasks executable in the Program field and "-h" or "--hide"
in the Arguments field.

When set up this way, My Scheduled Tasks will only show its window if there is a task that has a
check in the Alert column and the last result was non-zero or file not found. Otherwise, it will
shut down without showing any windows.

If you choose to add My Scheduled Tasks to Windows Task Scheduler, don't check the Alert checkbox
next to My Scheduled Tasks. If you do, it will always show an alert window because tasks always have
a non-zero result while they are running.


Command Line Options
====================
There are command line options that determine how My Scheduled Tasks starts. Specifying "-h" or "--hide" was
discussed in the previous paragraph.

Don't use the quote characters. They are used in the previous paragraphs for clarity. These options
work regardless of case, however the dash "-" character must be used. The slash "/" character will
not work.


Keyboard Shortcuts
==================
These keyboard shortcuts are available in the main window:

	Ctrl + Comma = Shows the Settings page.
	Ctrl + D = Toggles the details pane.
    Ctrl + R = Resets column sort.
	Ctrl + S = Saves the Task List file.
    Ctrl + T = Shows the View Tasks page.
	Ctrl + Minus = Decreases size.
	Ctrl + Plus = Increases size.
	Ctrl + Shift + C = Changes the primary accent color.
	Ctrl + Shift + F = Opens the application's folder.
	Ctrl + Shift + T = Changes the theme.
	Escape = Clears the selection.
	F1 = Shows the About page.
	F5 = Refreshes the displayed items on the View Tasks page.


Three-Dot Menu
==============
The three-dot menu at the right end of the page header that has options to view the log file, open the
tasks file, open the application folder, open the results code reference, and open readme file (this file).


Known Issues
============
Depending on the capabilities of your computer, performance may decline if hundreds of tasks are added
to the My Scheduled Tasks list.


Uninstalling My Scheduled Tasks
===============================
To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================
My Scheduled Tasks was written by Tim Kennedy.

My Scheduled Tasks uses the following packages:

    * TaskScheduler https://github.com/dahall/taskscheduler

    * Material Design in XAML Toolkit https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit

    * Command Line Parser https://github.com/commandlineparser/commandline

    * GongSolutions.WPF.DragDrop https://github.com/punker76/gong-wpf-dragdrop

    * Community Toolkit MVVM https://github.com/CommunityToolkit/dotnet

    * NLog https://nlog-project.org/

    * GitVersion https://github.com/GitTools/GitVersion

    * OctoKit https://github.com/octokit/octokit.net

    * Vanara https://github.com/dahall/vanara

    * GitKraken was used for everything Git related. https://www.gitkraken.com/

    * Inno Setup was used to create the installer. https://jrsoftware.org/isinfo.php

    * Visual Studio Community was used throughout the development of Get My IP. https://visualstudio.microsoft.com/vs/community/

    * XAML Styler is indispensable when working with XAML. https://github.com/Xavalon/XamlStyler

    * And the indispensable PowerToys https://github.com/microsoft/PowerToys



MIT License
Copyright (c) 2020 - 2024 Tim Kennedy

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject
to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
