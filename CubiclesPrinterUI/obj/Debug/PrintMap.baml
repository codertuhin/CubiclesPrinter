   M S B A M L   `   `   `  ����L  HCubiclesPrinterUI, Version=1.0.1.5, Culture=neutral, PublicKeyToken=nullK*clr-namespace:CubiclesPrinterUI.ConvertersCubiclesPrinterUI.Converters  5           CubiclesPrinterUI.PrintMap   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      A
converters*clr-namespace:CubiclesPrinterUI.Converters     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        � 	Resources  5      6   0CubiclesPrinterUI.Converters.FootprintsConverter   FootprintsConverter<   6CubiclesPrinterUI.Converters.MapLabelPositionConverter  MapLabelPositionConverter>   8CubiclesPrinterUI.Converters.MapLabelVisibilityConverter  MapLabelVisibilityConverter��%W   &	        &	      &	 &       5   
   6)     5   
   6/     5   
   61   5      .��5      � $��2��6   '  !System.Windows.VisualStateManager   VisualStateGroups 5   
   %  System.Windows.VisualStateGroup  5      -   5        PrinterHighlight��   States. 5         System.Windows.VisualState  -   5        Glow��   
Storyboard. 5      �� .��5      U�  g� RepeatBehavior$ Forever��5       �� 
TargetName$ PrinterGlow��5       �� TargetProperty$ Opacity�5      .\�5      ��  O� KeyTime$ 0:0:0.0��63    O� Value$ 0Q�6E   �� 5      $ 0:0:0.5��63   $ 1Q�6E   �� 5      $ 0:0:1��63   $ 0Q�6C   5      5      5      5       .+�5#   
   �� $��24,48,24,48q�6   $Z��Tpack://application:,,,/CubiclesPrinterUI;component/Assets/PrinterMap/printer-map.png��6%   �� 5%   
   $��24,48,24,48q�6   ��5&      `� 5'      	 `� 	Converter#	 [  6   .1�5(      �� 
 �� Path$
 UserPC�6   �� 5)      $
 SelectedPrinter�6   5*      5,      � 5/   
   -   5         �Name$ UserLocationLabel��50      $
��Left=�52      $	��Top=�53      ��51      �� *UserPC+ �� 	Converter# [  �� ConverterParameter$ CubicleLabel�� � RowDefinitions 54      �� 55      $
��Auto �6    �� 56      $
��Auto �6    �� 57      $
��Auto �6     � ColumnDefinitions 59      �� 5:      $
��Auto �6#   �� 5;      $
��Auto �6#   .+�5=      � $��0,0,0,-1q�5>      $��Center=�5?        PrimaryGreen#��[ 5@       � Points �    � ��[  �  � y�5B      �� *UserPC+# [ $ Bottom���� 5C      $��1��6   #��[ 6"   ._�5D      �� $��	12,6,12,6q�5E      $��14(�5F        SemiBoldWeight#��[ 5G      
�������$��You are here��5I      5J      � 5L      $��1��5M      $��1��5N      $��-1,0,0,0q�5O      $��Center=�5P      #��[ 5Q       �    � ��[  � y�5S      �� *UserPC+# [ $
 Side��� 5U      $��2��5V      $��0,-1,0,0q�5W      $��Center=�5X      #��[ 5Y       �    � ��[  � y�5[      �� *UserPC+# [ $	 Top��5\      � 5_   
   $
��Left=�5a      $	��Top=�5b      ��5`      �� *SelectedPrinter+# [ $ PrinterLabel�� 5c      �� 5d      $
��Auto �6    �� 5e      $
��Auto �6    �� 5f      $
��Auto �6    .+�5i      � $��0,0,0,-1q�5j      $��Center=�5k        HighlightPurple#��[ 5l       �    �  �  � ��[ 5m      �� 5o      $��1��6   #��[ 6"   ._�5p      �� $��	12,6,12,6q�5q      $��14(�5r      #��[ 5s      
�������$��Your printer is here��5u      5v      �� 5x      $��2��5y        ChangePrinterCombo#��[ 5|      ��5z      �� *
Printers+ �� SelectedItem 5{      �� *SelectedPrinter+ �� Mode$ TwoWay=���5}      �� 5~      .R�5      �� ��6$   �� *DisplayName+5�      5�      5�      �� 5�   
   -   5         ��Name$ PrinterGlow��5�      $
��17.5��5�      $
��17.5��5�      $
��Left=�5�      $	��Top=�5�        HalfPrimaryGreen#��[ 5�      #��[ 5�      $��2q�5�       J� Opacity$ 0Q�5�      ��5�      �� *SelectedPrinter+# [ $ PrinterGlow��5�      5�      5�      