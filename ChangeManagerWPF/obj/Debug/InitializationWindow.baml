   M S B A M L   `   `   `  ����K  GChangeManagerWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ChangeManagerWPFChangeManagerWPF  5   	   +    %ChangeManagerWPF.InitializationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:ChangeManagerWPF     5   	     � Title$  ChangeManager��5   	   $	��200��6   $	��600��6,   .��5	      � $��15q�6    � ColumnDefinitions 5
   
   �� 5      $��10 �6   �� 5      $
��Auto �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��5q�6   $��1��6   $��1��6+   .��68   ContractManager Address:6R   �� 5   
   -   5        $��initAddress��5      $��5q�6   $��2��60   $��1��6@   .��6M   
0x1234..6W   �� 5   
   $��5q�6   $��1��6   $��3��6+   .��68   Github Project:6I   �� 5   
   -   5        $��initGitProject��5      $��5q�6   $��2��63   $��3��6C   .��6Q   <owner>/<project>5   ,   � 5   
   $��5q�6   $��4��6   $��2��6,   $��Right=�6=   .+�5       �� -   5        $��5q�5       $��60��6    .��6?    Use ContractManager at address6_   �� 5!      -   5        $��5q�5!      $��60��6    .��6>   Create new ContractManager6Z   5"      5#      5$      