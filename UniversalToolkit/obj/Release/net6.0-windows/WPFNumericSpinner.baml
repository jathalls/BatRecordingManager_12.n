   M S B A M L   `   `   `  ���� L  HUniversalToolkit, Version=11.1.1.1, Culture=neutral, PublicKeyToken=null3clr-namespace:UniversalToolkitUniversalToolkit  %    UniversalToolkit.NumericSpinner   W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:UniversalToolkit     -     >�Name$  root_numeric_spinner�� � 	Resources    updown_button_style �� 
TargetType ��Name  ThemeBrushDark  
ThemeBrush �� CornerRadius ��Name �� Triggers P� Property P� Value	 �� 
TargetName  ThemeBrushActive
 �� Value �� Property �� 
TargetType��%  &	        '�� �     7   �� " ��.R��� $ br��$��0q�#���  #���  $ 0��._��� $ cp��#��z��#��z��#��z�� P�  �@J�IsMouseOver$
 true��. ��� $	 br��#
 �   � ��P�  � {�$ false��. ��� $	 br�� � ��

 ������� " ��.���  � ��8
   .����  J� OpacityMask !�  !� Visual �� 
���   � J� SnapsToDevicePixels$
 True��$ 4������ *ActualWidth+ �� RelativeSource �� *FindAncestor+ �� AncestorType
 Border���� *ActualHeight+ �� *FindAncestor+
 Border._��  � RowDefinitions �� ��  � ColumnDefinitions �� �� $��22 �.+��� -    ��Name$ tb_main��$��Medium"�$��14(�$��Center=�$	��5,1q�$��0��$��2��$	��0.0���� -    ��Name$ cmdUp��$��1��$��0��$��Center=�$��Center=�$��5�� �� 	ClickMode$ Press=�$��Webdings)� � ToolTip$^ XClick to increase value. CTRL-click to increase by 1/10*, SHIFT-Click to increase by 10*��$��18(�$|�False���� -   $ cmdDown��$��1��$��1��$��Center=�$��Center=�$��6��$ Press=�$��Webdings)�$��18(�$|�False��$��0,1,0,0q�$^ XClick to decrease value. CTRL-click to decrease by 1/10*, SHIFT-Click to decrease by 10*�����	#FF5D5C5C$��-1,1,1,1q��� 
������� J� IsHitTestVisible$ False��$��1q�$ 4��$��2��$��2�� �� Padding$ 0q�$��0q�