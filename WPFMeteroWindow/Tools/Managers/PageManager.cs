using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFMeteroWindow
{
    public static class PageManager
    {
        public static void HidePages() =>
            Intermediary.App.HideSettingGrid();
        
    }
}