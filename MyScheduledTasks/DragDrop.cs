using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32.TaskScheduler;

namespace MyScheduledTasks
{
    public class DragDrop : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            ScheduledTask sourceItem = dropInfo.Data as ScheduledTask;
            ScheduledTask targetItem = dropInfo.TargetItem as ScheduledTask;
            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }
        //public void Drop(IDropInfo dropInfo)
        //{
        //    ScheduledTask sourceItem = dropInfo.Data as ScheduledTask;
        //    ScheduledTask targetItem = dropInfo.TargetItem as ScheduledTask;
        //    ScheduledTask.TaskList.Remove(sourceItem);
        //    Debug.WriteLine($"{dropInfo.UnfilteredInsertIndex}");
        //    Debug.WriteLine($">>> in Drop src:{sourceItem.TaskName}  tgt:{targetItem.TaskName}");
        //    Debug.WriteLine($">>> in Drop pos:{dropInfo.InsertPosition} idx:{dropInfo.InsertIndex}");
        //    int tot = ScheduledTask.TaskList.Count;
        //    if (dropInfo.InsertIndex < tot)
        //    {
        //        Debug.WriteLine($"(insert at index) Dropping at {dropInfo.InsertIndex}");
        //        ScheduledTask.TaskList.Insert(dropInfo.InsertIndex, sourceItem);

        //    }
        //    else
        //    {
        //        Debug.WriteLine($"(Add) Dropping at {dropInfo.InsertIndex}");
        //        ScheduledTask.TaskList.Add(sourceItem);
        //    }
        //}
        public override void Drop(IDropInfo dropInfo)
        {
            base.Drop(dropInfo);
            Debug.WriteLine($">>> in Drop pos:{dropInfo.InsertPosition} idx:{dropInfo.InsertIndex}");
        }
    }
}
