   M S B A M L   `   `   `  ���� T  PBatCallAnalysisControlSet, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:BatCallAnalysisControlSetBatCallAnalysisControlSet  )    #BatCallAnalysisControlSet.ChartGrid   W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:BatCallAnalysisControlSet     .���   � ShowGridLines$
  True�� � RowDefinitions �� $��30 ��� -    ��Name$ topRow��$
��100* ��� -   $ 	BottomRow��$
��100* � � ColumnDefinitions �� -    ��Name$ 
leftColumn��$
��100* ��� -   $ rightColumn��$
��100* �.+��� -    ��Name$ ColumnSplitter��$��1��$��0��$��3��$
��Left=�$��Stretch=�
���   � �� ShowsPreview$
 True��$��3���� -   $ RowSplitter��$��2��$��0��$��2��$��Stretch=�$	��Top=�
���   �$
 True��$��3���� -    ��Name$ 
helpButton��$��?��$��40��$
��Left=�$��0��$��0��$��16(�$��	ExtraBold"�]� -    ]�Name$ TopRightPanel��$��Stretch=�$��Stretch=�$��1��$��1��]� -	   $ BottomLeftPanel��$��Stretch=�$��Stretch=�$��0��$��2��]� -
   $ BottomRightPanel��$��Stretch=�$��Stretch=�$��1��$��2��]� -   $ TopLeftPanel��$��Stretch=�$��Stretch=�$��0��$��1��.+�,   &BatCallAnalysisControlSet.CallDataForm  -   	  Name$	 callForm��$��Stretch=�$��Stretch=�$��0��$��1��