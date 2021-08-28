﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFMeteroWindow
{
    public static class ParallaxEffectPresenter
    {
        public static Image Image { get; set; } = null;

        public static double MoveMultipler { get; set; } = -1.5;

        public static void Init()
        {
            if (Image == null || MoveMultipler == 0) return;

            var windowWidth = Intermediary.App.MainGrid.ActualWidth;
            var windowHeight = Intermediary.App.MainGrid.ActualHeight;

            CountImageOffset(new Point(windowWidth / 2d, windowHeight / 2d));
        }

        public static void MakeParallaxEffect(object sender, MouseEventArgs e)
        {
            if (Image == null || MoveMultipler == 0) return;
            CountImageOffset(e.GetPosition(Intermediary.App.MainGrid));
        }

        private static void CountImageOffset(Point mousePosition)
        {
            var currentPosition = mousePosition;

            var windowWidth = Intermediary.App.MainGrid.ActualWidth;
            var windowHeight = Intermediary.App.MainGrid.ActualHeight;

            Intermediary.App.ScaleTranslateTransform.CenterX = windowWidth / 2d;
            Intermediary.App.ScaleTranslateTransform.CenterY = windowHeight / 2d;

            var centerPosition = new Point(windowWidth / 2d, windowHeight / 2d);

            var offsetX = MoveMultipler * (currentPosition.X - centerPosition.X);
            var offsetY = MoveMultipler * (currentPosition.Y - centerPosition.Y);

            offsetX /= windowWidth / 20d;
            offsetY /= windowHeight / 20d;

            Intermediary.App.MoveTranslateTransform.X = offsetX;
            Intermediary.App.MoveTranslateTransform.Y = offsetY;
        }
    }
}