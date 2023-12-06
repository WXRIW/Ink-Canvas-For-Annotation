﻿using Ink_Canvas.Helpers;
using System.Windows;
using System.Windows.Input;

namespace Ink_Canvas
{
    /// <summary>
    /// Interaction logic for StopwatchWindow.xaml
    /// </summary>
    public partial class OperatingGuideWindow : Window
    {
        public OperatingGuideWindow()
        {
            InitializeComponent();
            AnimationHelper.ShowWithSlideFromBottomAndFade(this, 0.25);
        }

        private void BtnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void WindowDragMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void SCManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e) {
            e.Handled = true;
        }
    }
}