using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JIRA_Printer;

namespace JIRA_Printer.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand AddStatusCommand = new RoutedUICommand
        (
                "AddStatusCommand",
                "AddStatusCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Alt)
                }
        );

        public static readonly RoutedUICommand RemoveStatusCommand = new RoutedUICommand
        (
                "RemoveStatusCommand",
                "RemoveStatusCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.R, ModifierKeys.Alt)
                }
        );

        public static readonly RoutedUICommand AddFieldCommand = new RoutedUICommand
        (
                "AddFieldCommand",
                "AddFieldCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand RemoveFieldCommand = new RoutedUICommand
        (
                "RemoveFieldCommand",
                "RemoveFieldCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.R, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ShowIssuesCommand = new RoutedUICommand
        (
                "ShowIssuesCommand",
                "ShowIssuesCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand PrintIssuesCommand = new RoutedUICommand
        (
                "PrintIssuesCommand",
                "PrintIssuesCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
        );

    }
}
