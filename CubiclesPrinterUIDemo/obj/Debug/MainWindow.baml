   M S B A M L   `   `   `  ����P  LCubiclesPrinterUIDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:CubiclesPrinterUIDemoCubiclesPrinterUIDemo  5      &     CubiclesPrinterUIDemo.MainWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      5local#clr-namespace:CubiclesPrinterUIDemo     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        � Title$  Cubicles UI Print Demo��5      $	��300��5	      $	��200��5
       � SizeToContent$ WidthAndHeight=�5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��	24,12,0,0q�6   $��Computer Name��6#   �� 5   
   -   5         ��Name$ ComputerText��5      $��1��5      $��	24,3,24,0q�5      $��cubicle08-pc��5      �� 5   
   -   5         ��Name$ PrintColorBtn��5      $��2��5       $	��160��5!      $��
24,12,24,0q�5"      $
��Left=�5#      $	��Top=�5$      $��Print Window (Color)��5&      �� 5(   
   -   5        $ PrintBlackBtn��5)      $��3��5*      $	��160��5+      $��
24,12,24,0q�5,      $
��Left=�5-      $	��Top=�5.      $��Print Window (No Color)��50      �� 52   
   -   5        $ PreparingBtn��53      $��4��54      $	��160��55      $��
24,12,24,0q�56      $
��Left=�57      $	��Top=�58      $��Preparing Window��5:      �� 5<   
   -   5        $ 
SendingBtn��5=      $��5��5>      $	��160��5?      $��24,12,24,24q�5@      $
��Left=�5A      $	��Top=�5B      $��Sending Window��5D      5E      5F      