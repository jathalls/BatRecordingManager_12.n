   M S B A M L   `   `   `  ����T  PBatCallAnalysisControlSet, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:BatCallAnalysisControlSetBatCallAnalysisControlSet  5      )    #BatCallAnalysisControlSet.ChartGrid   5      W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:BatCallAnalysisControlSet     5      .��5	      �   � ShowGridLines$
  True��6    � RowDefinitions 5
   
   �� 5      $��30 �6   �� 5      -   5         ��Name$ topRow��5      $
��100* �6,   �� 5      -   5        $ 	BottomRow��5      $
��100* �6/    � ColumnDefinitions 5   
   �� 5      -   5         ��Name$ 
leftColumn��5      $
��100* �63   �� 5      -   5        $ rightColumn��5   -   $
��100* �6   6-   .+�5   
   �� -   5         ��Name$ ColumnSplitter��5      $��1��5      $��0��6   $��3��6+   $
��Left=�5      $��Stretch=�5      
���   � �� ShowsPreview$
 True��5      $��3��5      �� 5   
   -   5        $ RowSplitter��5      $��2��5      $��0��6   $��2��6*   $��Stretch=�5       $	��Top=�5!      
���   �$
 True��5#      $��3��5$      �� 5%   
   -   5         ��Name$ 
helpButton��5&      $��?��5'      $��40��5(      $
��Left=�5)      $��0��5*      $��0��5+      $��16(�6   $��	ExtraBold"�6(   6?   ]� 5-   
   -   5         ]�Name$ TopRightPanel��5.      $��Stretch=�5/      $��Stretch=�50      $��1��51      $��1��52      ]� 54   
   -	   5        $ BottomLeftPanel��55      $��Stretch=�56      $��Stretch=�57      $��0��58      $��2��59      ]� 5;   
   -
   5        $ BottomRightPanel��5<      $��Stretch=�5=      $��Stretch=�5>      $��1��5?      $��2��5@      ]� 5B   
   -   5        $ TopLeftPanel��5C      $��Stretch=�5D      $��Stretch=�5E      $��0��5F      $��1��5G      .+�5H      ,   &BatCallAnalysisControlSet.CallDataForm  -   5        	  Name$	 callForm��5I      $��Stretch=�5K      $��Stretch=�5L      $��0��5M      $��1��5N      5O      5P      5Q      