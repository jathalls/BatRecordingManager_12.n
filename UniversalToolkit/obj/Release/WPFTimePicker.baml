   M S B A M L   `   `   `  ����   UniversalToolkit$    UniversalToolkit.WPFTimePicker   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     >�Name$  UserControl��$
��Auto��$
��Auto��.��� -    �Name$ 
LayoutRoot��$
��Auto��$
��Auto�� � ColumnDefinitions �� $
��0.2* ��� $��0.05* ��� $
��0.2* ��� $��0.05* ��� $
��0.2* �.+�� -   $
 hour��|�. .+��� -    ��Name$ mmTxt��$
��Wrap=� �� TextAlignment$ Center=�$��Center=�$��14(�����  �� Path$ Hours� �� ElementName$ UserControl�� �� Mode$ Default=� �� StringFormat$ 00��{��� *
IsReadOnly+� $��1��.+��� -   $
 sep1��$
��Wrap=�$��Center=�$��14(�$��:��$ Center=���Z� � -   $	 min��$��2��|�. .+��� -   $ ddTxt��$
��Wrap=�$ Center=�$��Center=�$��14(����� $ Minutes�$ UserControl��$ Default=�$ 00��{��� *
IsReadOnly+� $��3��.+��� -   $
 sep2��$
��Wrap=�$��Center=�$��14(�$��:��$ Center=���Z� � -	   $	��sec��$��4��|�. .+��� -
   $ yyTxt��$
��Wrap=�$ Center=�$��Center=�$��14(����� $ Seconds�$ UserControl��$ Default=�$ 00��{��� *
IsReadOnly+