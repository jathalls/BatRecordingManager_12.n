﻿#pragma checksum "..\..\..\WPFNumericSpinner.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3098EAC3A7213BFD3F1A05561D66BDAAE26B7FEA"
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
using UniversalToolkit;


namespace UniversalToolkit {
    
    
    /// <summary>
    /// NumericSpinner
    /// </summary>
    public partial class NumericSpinner : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\WPFNumericSpinner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal UniversalToolkit.NumericSpinner root_numeric_spinner;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\WPFNumericSpinner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private System.Windows.Controls.TextBox tb_main;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\WPFNumericSpinner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private System.Windows.Controls.Button cmdUp;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\WPFNumericSpinner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private System.Windows.Controls.Button cmdDown;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UniversalToolkit;component/wpfnumericspinner.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WPFNumericSpinner.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.root_numeric_spinner = ((UniversalToolkit.NumericSpinner)(target));
            return;
            case 2:
            this.tb_main = ((System.Windows.Controls.TextBox)(target));
            
            #line 76 "..\..\..\WPFNumericSpinner.xaml"
            this.tb_main.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tb_main_TextChanged);
            
            #line default
            #line hidden
            
            #line 77 "..\..\..\WPFNumericSpinner.xaml"
            this.tb_main.LostFocus += new System.Windows.RoutedEventHandler(this.tb_main_LostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cmdUp = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\..\WPFNumericSpinner.xaml"
            this.cmdUp.Click += new System.Windows.RoutedEventHandler(this.cmdUp_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cmdDown = ((System.Windows.Controls.Button)(target));
            
            #line 107 "..\..\..\WPFNumericSpinner.xaml"
            this.cmdDown.Click += new System.Windows.RoutedEventHandler(this.cmdDown_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

