   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   BackgroundLightColor  BackgroundDarkColor  TabHeaderSelectedColor  TabHeaderUnselectedColor  ContentBorderColor  ContentBorderThickness  TabSelectedBorderColor  TabUnselectedThickness  TabSelectedThickness  �� 
TargetType �� Property �� Value �� 
TargetType-   'System.Windows.Input.KeyboardNavigation    TabNavigation � RowDefinitions'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard	 �� 
TargetName
 �� TargetProperty8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value ��Name K� ZIndex K� IsItemsHost    TabIndex ��Name �� CornerRadius    DirectionalNavigation �� EndPoint �� 
StartPoint � Color � Offset �� Color ��Name �Name<  6System.Windows.Media.Animation.EasingThicknessKeyFrame p� KeyTime p� Value 	 DisabledControlDarkColor 
 ControlLightColor �� Triggers P� Property P� Value�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      /syst&clr-namespace:System;assembly=mscorlib  5      %�  &	        &	      &	 >     &	 ]     &	 y     &	 �     &	 �     &	 �     &	 �     '��     7  7   7  7  '�� �    7  7  7 	 7  7  7  7 
 7  ��5      #EDCEFF��62   ��5      #D5A1FF��61   ��5      #D890FF��64   ��5      Gray��63   ��5
      Transparent��64   u�5      0,0,0,0q�68   ��5      Blue��61   u�5      0,0,1,0q�68   u�5      0,0,2,0q�66   �� 5      "  ��6   .�5   
   ��  �@�OverridesDefaultStyle$
 True��62   �� 5   
    �@J�SnapsToDevicePixels$
 True��60   �� 5   
    � �� 5      �� 5      " ��6"   .R�5      � $ Local=�6    5      �� 5      $
��Auto �6,   �� 5      $��* �6,    5        5      CommonStates��. 5   "     Disabled��. 5   &   �� .��5    *   �� $	 Border��6G   $2
 ,(Border.BorderBrush).(SolidColorBrush.Color)�6f   .g�5!   .     $ 0��6B   $ Transparent��6N   5"   +   5#   '   5$   #   5%      .+�5'      �� $ HeaderPanel��6#   $��0��5(   #   $ 1��5)   #   $��0,0,0,0q�5*   #   $
 True��5+   #   $ 1��5,   #   
������ 5-   #   �� 5/      $ Border��6!   $��1��50   !   8��  $ 0,0,5,5��52   !   $ Local=�53   !   $ 	Contained=�54   !   $ 2��55   !   ��56      �� 57   "   $ 0.5,1&�66   $ 0.5,0&�58   6   .��59   &   � 8  $ 0Q�6a   � 5:   &   8  $ 1Q�6`   5;   #   ��5=      �� 5>   "   8  62   ._�5@      �� $ PART_SelectedContentHost��6/   $��4q�5A   /   $��SelectedContent��5B   /   5C      5D      5E      5G      5H      �� 5J      "  ��6   .�5K   
   ��  � �� 5L      �� 5M      " ��6"   .R�5N      � $
 Root��6    5O        5P      SelectionStates��. 5Q   "     
Unselected��6.     5R   "   Selected��. 5S   &   �� .��5T   *   �� $	 Border��6G   $N
 H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6f   .g�5U   .     $ 0��6B   8   6N   5V   +   r� 5W   *   $
 (Border.BorderThickness)�6K   $	 Border��6�   .	�5X   .     $ 0��6F   8  6R   5Y   +   5Z   '   5[   #   5\        5]      CommonStates��. 5^   "     
Normal��6.     5_   "   	MouseOver��6.     5`   "   Disabled��. 5a   &   �� .��5b   *   �� $	 Border��6G   $N
 H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6f   .g�5c   .     $ 0��6B   8  6N   5d   +   �� 5e   *   $	 Border��6G   $2
 ,(Border.BorderBrush).(SolidColorBrush.Color)�6f   .g�5f   .     $ 0��6B   8  6N   5g   +   5h   '   5i   #   5j      .+�5l      �� $ Border��6!   $��0,0,1,0q�5m   !   8�� $ 5,5,0,0��5o   !   ��5p      �� 5q   "   8  62   ��5s      �� 5u   "   $	 0,0&�66   $	 0,1&�6G   ��5w   *   � � 5x   .   8  $	 0.0Q�6g   � 5y   .   8  $	 1.0Q�6m   5z   +   5|   #   ._�5      �� $ ContentSite��6/   $��Center=�5�   1   $��Center=�5�   1   $��Header��5�   1   $��	12,2,12,2q�5�   1   $
��True��5�   1   5�      5�       5�      P� 5�       �@��
IsSelected$
 True��68   . �5�      ��  �@K�ZIndex$	 100��6=   5�      5�      5�      5�      5�      