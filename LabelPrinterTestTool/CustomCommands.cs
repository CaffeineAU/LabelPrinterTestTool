﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LabelPrinterTestTool;

namespace LabelPrinterTestTool.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand SendTextCommand = new RoutedUICommand
        (
                "SendTextCommand",
                "SendTextCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ConnectCommand = new RoutedUICommand
        (
                "ConnectCommand",
                "ConnectCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.C, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand CutCommand = new RoutedUICommand
        (
                "CutCommand",
                "CutCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Alt)
                }
        );

        public static readonly RoutedUICommand ResetCommand = new RoutedUICommand
        (
                "ResetCommand",
                "ResetCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.R, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand FeedCommand = new RoutedUICommand
        (
                "FeedCommand",
                "FeedCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F, ModifierKeys.Control)
                }
        );
        public static readonly RoutedUICommand FindCommand = new RoutedUICommand
        (
                "FindCommand",
                "FindCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Alt)
                }
        );
        public static readonly RoutedUICommand PrintCommand = new RoutedUICommand
        (
                "PrintCommand",
                "PrintCommand",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P, ModifierKeys.Control)
                }
        );
    }
}
