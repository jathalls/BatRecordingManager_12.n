﻿#pragma checksum "..\..\..\WPFTimePicker.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "36C4DD385DDAAE858E3450B2E1351812C134F2FE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace UniversalToolkit {
    
    
    /// <summary>
    /// WPFTimePicker
    /// </summary>
    public partial class WPFTimePicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal UniversalToolkit.WPFTimePicker UserControl;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid hour;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock mmTxt;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock sep1;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid min;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ddTxt;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock sep2;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid sec;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\WPFTimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock yyTxt;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UniversalToolkit;component/wpftimepicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WPFTimePicker.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((UniversalToolkit.WPFTimePicker)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.hour = ((System.Windows.Controls.Grid)(target));
            
            #line 27 "..\..\..\WPFTimePicker.xaml"
            this.hour.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 4:
            this.mmTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.sep1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.min = ((System.Windows.Controls.Grid)(target));
            
            #line 43 "..\..\..\WPFTimePicker.xaml"
            this.min.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ddTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.sep2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.sec = ((System.Windows.Controls.Grid)(target));
            
            #line 60 "..\..\..\WPFTimePicker.xaml"
            this.sec.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 10:
            this.yyTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

