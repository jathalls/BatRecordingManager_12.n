   M S B A M L   `   `   `  ����L  HUniversalToolkit, Version=11.1.1.1, Culture=neutral, PublicKeyToken=null3clr-namespace:UniversalToolkitUniversalToolkit  5      %    UniversalToolkit.DateTimePicker   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:UniversalToolkit     5      Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4VwpftkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  5      Jwpftc19http://schemas.microsoft.com/netfx/2009/xaml/presentation    5        � 	Resources  5         
IconButton �� 
TargetType#   UniversalToolkit.BoolInverter  boolInverter��%s   &	        &	 L     �� 5   
   " d�6-   .R�5      �� ._�5      �� 5      5        5   
   6   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ DateDisplay��5      $��Stretch=�5      $��Center=�5      $��0,0,0,0q�5      $��yyyy-MM-dd HH:mm��5!   _    �� 
IsReadOnly 5       �� *
IsReadOnly+��5!      ��  �� ElementName$ PopUpCalendarButton�� �� Path$ ActualHeight�.��6w   ��5"      �� 5#       �� 
TargetType" ��6    �� Triggers 5$      �� 5%      	 �� Binding	 6&   �� *DateTextIsWrong+
 �� RelativeSource
 ��  �� AncestorType" >� �� Value True6�   .a�5&      ��  �� Property � �� �� Value
 �����6;   5'      5)      .��5*      5+      d� 5,   
   -   5        $��PopUpCalendarButton��5/      #��[  5,       d� 	IsChecked$ FalseY�50      $��2q�6)   $��2��52      {�51      �� *
IsReadOnly+ �� 	Converter# [ ��55      F� 56      $��Center=�6   $��Center=�64   �����        � -1    � -1 -1�     -1�        �@KL     �@KL  -1Ѐ��     р��  -1����     ����  -1�    @KL � -1@KL �    ��� � -1��� �    ��� � -1��� 	
���   �58      5;      � 5=   
   -   5         �Name$ CalendarPopup��5>      $��0,-7,0,0q�6'   $
��Fade=�5?       � 	StaysOpen .  ��5=      �� $ 	IsChecked�$ PopUpCalendarButton��{�5@      �� *
IsReadOnly+# [ .��5B      &   System.Windows.Controls.Calendar  -   5          Name$ 
CalDisplay��5B   0   $��0,-1,0,0q�6   6G   5C      5E      5F      