﻿#pragma checksum "C:\Users\moss\Documents\Visual Studio 2010\hyaku\Hyaku03\Views\GameBoardView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D0C8F5FA3C835B3A71421F57FAB8D6F6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Hyaku.Views {
    
    
    public partial class GameBoardView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.StackPanel LayoutRoot;
        
        internal System.Windows.Controls.Grid NumberQueueDisplay;
        
        internal System.Windows.Controls.TextBlock NextNumberTextLabel;
        
        internal System.Windows.Controls.TextBlock NextNumberTextBlock;
        
        internal System.Windows.Controls.TextBlock ScoreTextBlock;
        
        internal System.Windows.Controls.Grid GameGrid;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Hyaku;component/Views/GameBoardView.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.StackPanel)(this.FindName("LayoutRoot")));
            this.NumberQueueDisplay = ((System.Windows.Controls.Grid)(this.FindName("NumberQueueDisplay")));
            this.NextNumberTextLabel = ((System.Windows.Controls.TextBlock)(this.FindName("NextNumberTextLabel")));
            this.NextNumberTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("NextNumberTextBlock")));
            this.ScoreTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("ScoreTextBlock")));
            this.GameGrid = ((System.Windows.Controls.Grid)(this.FindName("GameGrid")));
        }
    }
}

