   M S B A M L   `   `   `  ����U  QBatCallAnalysisControlSet, Version=11.1.1.1, Culture=neutral, PublicKeyToken=nullE'clr-namespace:BatCallAnalysisControlSetBatCallAnalysisControlSet  5      ,    &BatCallAnalysisControlSet.CallDataForm   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:BatCallAnalysisControlSet     5      .��5
      �   � RowDefinitions  5   
   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��10 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��10 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��* �6   �� 5      $��30 �6    � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $	��200 �6   �� 5      $��80 �6   �� 5      $��80 �6   �� 5      $��80 �6   �� 5       $��20 �6   �� 5!      $��90 �6   �� 5"      $��* �6   .+�5%   
   �� $	��Min��6   $��Center=�6   $��2��6;   $��0��6K   �� 5&   
   $
��Mean��6   $��Center=�6   $��3��6<   $��0��6L   �� 5'   
   $	��Max��6   $��Center=�6   $��4��6;   $��0��6K   �� 5)   
   $��Range/2 or SD��6   $��Center=�6(   $��6��6E   $��0��6U   �� 5*   
   $&�� Enter Mean and SD or Min and Max��6   $��7��6;   $��0��6K   �� 5,   
   $��Start Frequency kHz��6   $��Right=�6.   $��1��6J   $��1��6Z    � ToolTip$/ )Highest frequency in the fundamental (H1)��6g   �� 5-   
   $��Peak Frequency kHz��6   $��Right=�6-   $��1��6I   $��2��6Y   $' !Frequency of Maximum energy in H1��6f   �� 5.   
   $��End Frequency kHz��6   $��Right=�6,   $��1��6H   $��3��6X   $" Lowest frequency in the call��6e   �� 50   
   $��Pulse Duration ms��6   $��Right=�6,   $��1��6H   $��5��6X   $ Pulse length in ms��6e   �� 51   
   $��Pulse Interval ms��6   $��Right=�6,   $��1��6H   $��6��6X   $2 ,start-start Interval  to the following pulse��6e   �� 53   
   $��Bandwidth kHz��6   $��Right=�6(   $��1��6D   $��8��6T   $@ :Difference between the highest and lowest frequency inn H1��6a   �� 54   
   $��Knee Frequency kHz��6   $��Right=�6-   $��1��6I   $��9��6Y   $F @In some Myotis, the frequency of the steep to shallow inflection��6f   �� 55   
   $��Heel Frequency kHz��6   $��Right=�6-   $��1��6I   $��10��6Y   $E ?In some Myotis the frequency of the shallow to steep inflection��6g   /   )BatCallAnalysisControlSet.ParameterSetter  57   
   -   5          Name$ startFrequencySetter��58      $��1��59      $��2��5:      $��5��5;         MaxPermitted$	 210Q�5<        5>   
   -   5        $ peakFrequencySetter��5?      $��2��5@      $��2��5A      $��5��5B      $	 210Q�5C        5E   
   -   5        $ endFrequencySetter��5F      $��3��5G      $��2��5H      $��5��5I      $	 210Q�5J        5L   
   -   5        $ durationSetter��5M      $��5��5N      $��2��5O      $��5��5P      $	 250Q�5Q        5S   
   -   5        $ intervalSetter��5T      $��6��5U      $��2��5V      $��5��5W      $
 1000Q�5X        5Z   
   -   5        $ bandwidthSetter��5[      $��8��5\      $��2��5]      $��5��5^      $	 210Q�5_        5a   
   -   5        $ KneeFrequencySetter��5b      $��9��5c      $��2��5d      $��5��5e      $	 210Q�5f        5h   
   -   5        $ heelFrequencySetter��5i      $��10��5j      $��2��5k      $��5��5l      $	 210Q�5m      &   System.Windows.Controls.DataGrid  5o   
   -	   5          Name$ callDataGrid��5p      $��11��5q      $��1��5r      $��7��5s      $��10q�5t         AutoGenerateColumns .  $y�Visible=�5v      "   VerticalScrollBarVisibility$ Visible=�5x      ��5w      ��  �� Path$ dataGridData�	   Columns	 5y      0  *System.Windows.Controls.DataGridTextColumn  5z      ,  &System.Windows.Controls.DataGridColumn
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 60 6U      Header$ Position��6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� $ Start� �� StringFormat$ F1��   ClipboardContentBinding 6`   Z� 6�     5{      $
 50 6R   $	 Fhi��6�    6%   �� $	 Fhi�$ F1�� 6]   Z� 6�     5|      $
 50 6R   $	 Fpk��6�    6%   �� $	 Fpk�$ F1�� 6]   Z� 6�     5}      $
 50 6R   $	 Flo��6�    6%   �� $	 Flo�$ F1�� 6^   Z� 6�     5~      $
 50 6Q   $ Fk��6�    6%   �� $ Fk�$ F1�� 6]   Z� 6�     5      $
 50 6Q   $ Fh��6�    6%   �� $ Fh�$ F1�� 6]   Z� 6�     5�      $
 50 6R   $ Durtn��6�    6%   �� $	 Dur�$ F2�� 6^   Z� 6�   5�      �� 5�   
   -
   5         ��Name$ SetCallParametersButton��5�      $��Set Call Parameters��5�      $	��150��5�      $��3��5�      $��4��5�      $��12��5�      6   �� 5�   
   -   5        $ clearButton��5�      $��CLEAR��5�      $��60��5�      $��2��5�      $��12��5�      6   �� 5�   
   -   5        $ pasteButton��5�      $��Paste��5�      $��60��5�      $��1��5�      $��12��5�      $. (Paste ZC Analysis data from Kaleidoscope��5�      6@   �� 5�   
   -   5        $ 
SaveButton��5�      $
��Save��5�      $��60��5�      $��12��5�      {�. $��7��5�      6   5�      5�      