   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   BackgroundLightColor  BackgroundDarkColor  TabHeaderSelectedColor  TabHeaderUnselectedColor  ContentBorderColor  ContentBorderThickness  TabSelectedBorderColor  TabUnselectedThickness  TabSelectedThickness  �� 
TargetType �� Property �� Value �� 
TargetType-   'System.Windows.Input.KeyboardNavigation    TabNavigation � RowDefinitions'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard	 �� 
TargetName
 �� TargetProperty8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value ��Name K� ZIndex K� IsItemsHost    TabIndex ��Name �� CornerRadius    DirectionalNavigation �� EndPoint �� 
StartPoint � Color � Offset �� Color ��Name �Name<  6System.Windows.Media.Animation.EasingThicknessKeyFrame p� KeyTime p� Value 	 DisabledControlDarkColor 
 ControlLightColor �� Triggers P� Property P� Value�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     /syst&clr-namespace:System;assembly=mscorlib  %v  &	        &	      &	 "     &	 3     &	 A     &	 V     &	 g     &	 u     &	 �     '�� �     7  7   7  7  '�� 1    7  7  7 	 7  7  7  7 
 7  ��#EDCEFF����#D5A1FF����#D890FF����Gray����Transparent��u�0,0,0,0q���Blue��u�0,0,1,0q�u�0,0,2,0q��� "  ��.���  �@�OverridesDefaultStyle$
 True����  �@J�SnapsToDevicePixels$
 True����  � �� �� " ��.R�� $ Local=� �� $
��Auto ��� $��* �   CommonStates��.   Disabled��. �� .���� $	 Border��$2
 ,(Border.BorderBrush).(SolidColorBrush.Color)�.g�  $ 0��$ Transparent��.+��� $ HeaderPanel��$��0��$ 1��$��0,0,0,0q�$
 True��$ 1��
������ �� $ Border��$��1��8��  $ 0,0,5,5��$ Local=�$ 	Contained=�$ 2������ $ 0.5,1&�$ 0.5,0&�.��� 8  $ 0Q�� 8  $ 1Q����� 8  ._��� $ PART_SelectedContentHost��$��4q�$��SelectedContent���� "  ��.���  � �� �� " ��.R�� $
 Root��   SelectionStates��.   
Unselected��  Selected��. �� .���� $	 Border��$N
 H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $ 0��8   r� $
 (Border.BorderThickness)�$	 Border��.	�  $ 0��8    CommonStates��.   
Normal��  	MouseOver��  Disabled��. �� .���� $	 Border��$N
 H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�.g�  $ 0��8  �� $	 Border��$2
 ,(Border.BorderBrush).(SolidColorBrush.Color)�.g�  $ 0��8  .+��� $ Border��$��0,0,1,0q�8�� $ 5,5,0,0������ 8  ���� $	 0,0&�$	 0,1&���� � 8  $	 0.0Q�� 8  $	 1.0Q�._��� $ ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q�$
��True�� P�  �@��
IsSelected$
 True��. ���  �@K�ZIndex$	 100��