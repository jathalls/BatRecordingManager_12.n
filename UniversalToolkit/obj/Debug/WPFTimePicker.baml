   M S B A M L   `   `   `  ����  UniversalToolkit$    UniversalToolkit.WPFTimePicker   5      ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$  UserControl��5	       $
��Auto��6   $
��Auto��6   .��5      � -   5         �Name$ 
LayoutRoot��5      $
��Auto��6   $
��Auto��6,    � ColumnDefinitions 5   
   �� 5      $
��0.2* �6   �� 5      $��0.05* �6   �� 5      $
��0.2* �6   �� 5      $��0.05* �6   �� 5      $
��0.2* �6   .+�5   
   � -   5        $
 hour��5      |�. .+�5      �� -   5         ��Name$ mmTxt��5      $
��Wrap=�6'    �� TextAlignment$ Center=�5      $��Center=�6)   $��14(�6E   ��5   ;   ��  �� Path$ Hours� �� ElementName$ UserControl�� �� Mode$ Default=� �� StringFormat$ 00��{�5      �� *
IsReadOnly+5   E   5!      � 5#   
   $��1��6   .+�5%      �� -   5        $
 sep1��5%      $
��Wrap=�6'   $��Center=�6;   $��14(�5'      $��:��6!   $ Center=�6*   ��5%   V   Z� 5'   *   5)      � 5+   
   -   5        $	 min��5+       $��2��6   |�. .+�5-      �� -   5        $ ddTxt��5-      $
��Wrap=�6)   $ Center=�50      $��Center=�6*   $��14(�6F   ��5-   =   �� $ Minutes�$ UserControl��$ Default=�$ 00��{�5.      �� *
IsReadOnly+50   F   52      � 54   
   $��3��6   .+�56      �� -   5        $
 sep2��56      $
��Wrap=�6'   $��Center=�6;   $��14(�58      $��:��6    $ Center=�6)   ��56   V   Z� 58   )   5:      � 5<   
   -	   5        $	��sec��5<       $��4��6   |�. .+�5>      �� -
   5        $ yyTxt��5>      $
��Wrap=�6(   $ Center=�5@      $��Center=�6)   $��14(�6E   ��5>   <   �� $ Seconds�$ UserControl��$ Default=�$ 00��{�5?      �� *
IsReadOnly+5@   E   5B      5F      5H      