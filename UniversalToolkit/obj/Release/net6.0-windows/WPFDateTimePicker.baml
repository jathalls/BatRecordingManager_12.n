   M S B A M L   `   `   `  ���� L  HUniversalToolkit, Version=11.1.1.1, Culture=neutral, PublicKeyToken=null3clr-namespace:UniversalToolkitUniversalToolkit  %    UniversalToolkit.DateTimePicker   W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:UniversalToolkit     Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4VwpftkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  Jwpftc19http://schemas.microsoft.com/netfx/2009/xaml/presentation      � 	Resources     
IconButton �� 
TargetType#   UniversalToolkit.BoolInverter  boolInverter��%3   &	        &	      �� " d�.R��� ._���   .���  � ColumnDefinitions �� $��* ��� $
��Auto �.+��� -    ��Name$ DateDisplay��$��Stretch=�$��Center=�$��0,0,0,0q�$��yyyy-MM-dd HH:mm�� �� 
IsReadOnly �� *
IsReadOnly+����  �� ElementName$ PopUpCalendarButton�� �� Path$ ActualHeight�.������  �� 
TargetType" �� �� Triggers �� 	 �� Binding	 �� *DateTextIsWrong+
 �� RelativeSource
 ��  �� AncestorType" >� �� Value True.a���  �� Property � �� �� Value
 �����.��d� -   $��PopUpCalendarButton��#��[   d� 	IsChecked$ FalseY�$��2q�$��2��{��� *
IsReadOnly+ �� 	Converter# [ ��F� $��Center=�$��Center=������        � -1    � -1 -1�     -1�        �@KL     �@KL  -1Ѐ��     р��  -1����     ����  -1�    @KL � -1@KL �    ��� � -1��� �    ��� � -1��� 	
���   �� -    �Name$ CalendarPopup��$��0,-7,0,0q�$
��Fade=� � 	StaysOpen$ False������ $ 	IsChecked�$ PopUpCalendarButton��{��� *
IsReadOnly+# [ .��&   System.Windows.Controls.Calendar  -     Name$ 
CalDisplay��$��0,-1,0,0q�