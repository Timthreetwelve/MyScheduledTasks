The MyScheduledTasks ReadMe File


Introduction
============
MyScheduledTasks is an application that lets you keep track of the tasks in Windows Task Scheduler that
you care about. MyScheduledTasks can be automated to let you know if any of the tasks completed with
a non-zero exit code. It's important to note that MyScheduledTasks allows you an simple way to check
on scheduled tasks, it doesn't allow you to add, delete, or change them. For those activities you
will need to use Windows Task Scheduler.


Getting Started
===============
When you first start MyScheduledTasks you will see a window that has a menu and some column headers.
The first thing to do is to add some tasks from Windows Task Scheduler.  Click on the Tasks menu and
then on Add Tasks, or press Ctrl and N. A window will pop up with a list of tasks. Select desired
tasks and then click the Add button. Standard Windows selection rules apply, hold the Ctrl key down
to select more than one, hold the shift key down to select a range. When you have selected the tasks
that you want, click the Add button. When you are finished adding, click the Done button.


Using MyScheduledTasks
======================
After adding tasks, you will notice that there are several columns in the grid. The first column,
the one with the ⚠ (warning emoji), has a checkbox. Check the checkbox if you are going to automate
MyScheduledTasks and you want to be alerted if the task in this row had a non-zero result from it's
last scheduled run.

The next column is the task name as seen in Windows Task Scheduler.

The next column shows the last result. It shows "OK" is the last result was zero or "NZ" in red for a
non-zero result. Hovering over the result will show a tooltip with the actual last result in hex.

The next column shows the status of the task. If the task is currently running, the row will be shown
in green text. If the task is disabled, the row will be shown in gray text.

The next column shows the date and time of the last run.

The next column shows the folder in Windows Task Scheduler where the task is located.

The last column is the Notes column. You may enter a note directly into this field.

With the exception of the Task Name column, any of the columns may be hidden by un-checking them in
the View menu.


Additional Details
==================
For additional details on any task, simply double-click the corresponding row to show an expanded
view. To collapse the details, double-click again. If you have expanded several rows, you can collapse
all of them by selecting Collapse All from the View menu.


Removing Tasks
==============
You can remove tasks by right-clicking the appropriate row and selecting Remove Selected or by using
the Remove Selected item on the Tasks menu. Multiple rows can be deleted if needed. Note that this
does not remove any tasks from Windows Task Scheduler.


Exporting Tasks
===============

You can export a task by right-clicking the appropriate row and selecting Export Selected or by using
the Export Selected item on the Tasks menu. Tasks are exported in XML format. Only one task at a time
may be exported.


Saving your Task List
=====================
After adding and/or removing tasks, you will notice "Unsaved changes" in the right corner of the
status bar. You can save your changes by selecting Save Task List File from the File menu. You can
also save your changes by pressing Ctrl and S. Your selections are saved in a file named MyTasks.json
in the application's folder.

You can opt to have your changes automatically saved when exiting by checking the Save Task List on
Exit item on the Options Menu. You can also choose to suppress some of the file saved notifications
if those dialog boxes get to be too much.


Sorting the Grid
================
You can sort the grid by clicking any of the column headers. Click again to sort in the opposite
order. Your sort will be saved and will be applied next time you start MyScheduledTasks. To reset
the column sort, use Reset Column Sort on the Options menu.


Run as Administrator
====================
If MyScheduledTasks is not running as an administrator, you will only see a subset of all the tasks
in Windows Task Scheduler. If it is running as Administrator, you can see more of the tasks. If all
of the tasks that you care about can be seen without running as an administrator, then by all means
run it that way.

There is an item on the File menu that will restart MyScheduledTasks as Administrator. Depending on
your system settings, you may see a UAC dialog asking for permission to run.

To always run MyScheduledTasks as an administrator, locate MyScheduledTasks.exe in File Explorer,
right-click on the file and select Properties. In the Properties dialog, select the Compatibility tab.
Check the option that says Run the program as an administrator.

Remember, you only need to run MyScheduledTasks as an administrator if you do not see the tasks that
you desire when running it normally. Whichever way you choose to run it, do so consistently.


Enable, Disable or Run Tasks
============================

If MyScheduledTasks is running as administrator, the options to enable, disable and run the selected
task on the context (right-click) menu.


Automating MyScheduledTasks
===========================
You can run MyScheduledTasks on a regular schedule by adding a task to Windows Task Scheduler,
specifying the path to the MyScheduledTasks executable in the Program field and /hide in the
Arguments field.

For your convenience, there is a menu selection to start Windows Task Scheduler on the Tasks menu
and an option on the Help/Debug menu to copy the executable path to the clipboard.

When set up this way, MyScheduledTasks will only show its window if there is a task that has a
check in the Alert column and the last result was non-zero. Otherwise, it will shut down without
showing its window.

If you choose to add MyScheduledTasks to Windows Task Scheduler, don't check the Alert checkbox
next to MyScheduledTasks. If you do, it will always show its window because tasks always have a
non-zero result while they are running.


Notices and License
===================

MyScheduledTasks was written in C# by Tim Kennedy.

MyScheduledTasks uses the following icons & packages:

Fugue Icons set https://p.yusukekamiyamane.com/

Json.net v12.0.3 from Newtonsoft https://www.newtonsoft.com/json

TaskScheduler v2.8.20 https://github.com/dahall/taskscheduler

NLog v4.7.3 https://nlog-project.org/


MIT License
Copyright (c) 2020 Tim Kennedy

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
