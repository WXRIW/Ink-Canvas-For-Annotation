using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ink_Canvas.Helpers
{
    internal class AnimationHelper
    {
        public static void ShowWithSlideFromBottomAndFade(UIElement element, double duration = 0.15)
        {
            if (element.Visibility == Visibility.Visible) return;

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var sb = new Storyboard();

            // 渐变动画
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            // 滑动动画
            var slideAnimation = new DoubleAnimation
            {
                From = element.RenderTransform.Value.OffsetY + 10, // 滑动距离
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            sb.Children.Add(fadeInAnimation);
            sb.Children.Add(slideAnimation);

            element.Visibility = Visibility.Visible;
            element.RenderTransform = new TranslateTransform();

            sb.Begin((FrameworkElement)element);
        }

        public static void ShowWithSlideFromLeftAndFade(UIElement element, double duration = 0.5)
        {
            if (element.Visibility == Visibility.Visible) return;

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var sb = new Storyboard();

            // 渐变动画
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            // 滑动动画
            var slideAnimation = new DoubleAnimation
            {
                From = element.RenderTransform.Value.OffsetX - 20, // 滑动距离
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            sb.Children.Add(fadeInAnimation);
            sb.Children.Add(slideAnimation);

            element.Visibility = Visibility.Visible;
            element.RenderTransform = new TranslateTransform();

            sb.Begin((FrameworkElement)element);
        }

        public static void HideWithSlideAndFade(UIElement element, double duration = 0.15)
        {
            if (element.Visibility == Visibility.Collapsed) return;

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var sb = new Storyboard();

            // 渐变动画
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

            // 滑动动画
            var slideAnimation = new DoubleAnimation
            {
                From = 0,
                To = element.RenderTransform.Value.OffsetY + 10, // 滑动距离
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            sb.Children.Add(fadeOutAnimation);
            sb.Children.Add(slideAnimation);

            sb.Completed += (s, e) =>
            {
                element.Visibility = Visibility.Collapsed;
            };

            element.RenderTransform = new TranslateTransform();
            sb.Begin((FrameworkElement)element);
        }
    }
}
