   M S B A M L   `   `   `  ����L  HUniversalToolkit, Version=11.1.1.1, Culture=neutral, PublicKeyToken=null3clr-namespace:UniversalToolkitUniversalToolkit  5      %    UniversalToolkit.NumericSpinner   5      ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:UniversalToolkit     5      -   5          >�Name$  root_numeric_spinner��5       � 	Resources 5	         updown_button_style �� 
TargetType ��Name  ThemeBrushDark  
ThemeBrush �� CornerRadius ��Name �� Triggers P� Property P� Value	 �� 
TargetName  ThemeBrushActive
 �� Value �� Property �� 
TargetType��%�  &	        '�� �    7   �� 5
   
   " ��66   .R�5      �� $ br��6   $��0q�6!   #���  65   #���  5      $ 0��6G   ._�5      �� $ cp��6#   #��z��6/   #��z��5   +   #��z��5   +   5       5      P� 5       �@J�IsMouseOver . . �5      �� $	 br��6   #
 �  6C    � ��5      P� 5       � {� .  . �5      �� $	 br��6    � ��

 �����6C   5      5      �� 5   
   " ��6   .�5      ��  � ��8
   6)   5      5      .��5      ��  J� OpacityMask 5    
   !� 5!       !� Visual 5"      �� 5#      
���   � J� SnapsToDevicePixels . $ 4��5$      ��5%      �� *ActualWidth+ �� RelativeSource �� *FindAncestor+ �� AncestorType$ BorderL���5&      �� *ActualHeight+ �� *FindAncestor+$ BorderL�5(      ._�5+      �  � RowDefinitions 5,      �� 5-      �� 5.       � ColumnDefinitions 50      �� 51      �� 52      $��22 �6'   .+�54      �� -   5         ��Name$ tb_main��54      $��Medium"�6E   $��14(�6Y   $��Center=�6g   $	��5,1q�6�   $��0��55      $��2��6*   $��0��6;   �� 56      -   5         ��Name$ cmdUp��56      $��1��6B   $��0��6R   $
��auto��6_   $
��auto��6l   ���	#FF5B5959��57      F� 58      $��Center=�6   $��Center=�6<   
���   �"���� 	=     �     	= � z  	= 	6d   5:      �� 5;      -   5        $ cmdDown��5;      $��1��6D   $��1��6T   $
��auto��6a   $
��auto��6n   ��5<      F� 5=      $��Center=�6   $��Center=�6<   
���   �"����        � z     � 	=  	= 	6d   5?      �� 5@      
������� J� IsHitTestVisible .  $��1q�6E   $ 4��6Y   $��2��6j   $��2��5A       �� Padding$ 0q�6-   $��0q�69   5B      5E      5F      