   M S B A M L   `   `   `  ����K  GEduManDesktopApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MyControl	MyControl  5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ComboBoxToggleButton  �� 
TargetType � ColumnDefinitions'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� TargetProperty �� 
TargetName8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime  ButtonBotColor �� Value  DisabledControlDarkColor  DisabledForegroundColor  DisabledBorderDarkColor	 ��Name
 �� CornerRadius  ComboboxGradientBG �� Color F�Name  DarkMasterTextCBrush   MyControl.ReadOnlyComboBox �� 
TargetType �� Property �� Value R� KeyTime  System.Windows.Visibility   Visible R� Value   Hidden d�Name �� 	ClickMode d� 	IsChecked �� Mode �� RelativeSource ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate �Name �� IsDropDownOpen � AllowsTransparency  �Name! J� SnapsToDevicePixels" � ActualWidth# �� MaxDropDownHeight  BorderMediumColor  ControlLightColor$ K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation%   DirectionalNavigation& ��Name' �� 
IsReadOnly( �� 
IsReadOnly) �� Triggers* P� Property+ P� Value, �� 
TargetName- P� 
SourceName. �� Padding 	 MediumMasterColor 
 LightMasterColor�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      ucclr-namespace:MyControl  5      %>  &	        7  7  7  7  7  7  7  7  '  �    7  7   '�� �    7 	 7 	 7 
 �� 5      "  d�5      .R�5   
   �  5      �� 5      �� 5	      $��20 �6#    5        5      CommonStates��. 5        
Normal��6"     5      	MouseOver��. 5      �� .��5      �� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6;   $ Border��5   ;   .g�5   "     $ 0��66   8   6B   5      5      5        5      Pressed��6"     5      Disabled��. 5      �� .��5      �� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6;   $ Border��5   ;   .g�5   "     $ 0��66   8  6B   5      �� 5      $* $(Shape.Fill).(SolidColorBrush.Color)�6;   $ Arrow��5   ;   .g�5   "     $ 0��66   8  6B   5       �� 5!      $P J(Border.BorderBrush).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6;   $ Border��5"   ;   .g�5#   "     $ 0��66   8  6B   5$      5%      5&      5'        5(      CheckStates��. 5)        Checked��. 5*      �� .��5+      �� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6;   $ Border��5,   ;   .g�5-   "     $ 0��66   8  6B   5.      5/      50        51      	Unchecked��6"     52      Indeterminate��6"   53      .+�55      �� $	 Border��6   $��2��56      $
 5��57      8�� $��0q�59      ��5:      �� 5;      $ 	LightBlue��6&   5=      �� 5>      $��0��6   $
 5,0,0,5��5?      8�� $��01q�5A      5C      F� 5D      $ Arrow��6   $��1��5E      $��Center=�5F      $��Center=�5G      "����        � 	=  	= � z     	��5I      �� 5J      8  6&   5L      5M      5N      �� 5V      "  6   .�5W   
   ��  �@J�SnapsToDevicePixels$
 true��60   �� 5X   
    �@�OverridesDefaultStyle$
 true��62   �� 5Y   
    � ��$
 Auto=�6G   �� 5Z   
    � ��$
 Auto=�6E   �� 5[   
    � ��$
 true��6:   �� 5\   
    � ��$	 120��6%   �� 5]   
    � ��$ 20��6&   �� 5^   
    � �� 5_      �� 5`      "  ��6"   .R�5a      �  5b        5c      CommonStates��. 5d   "     
Normal��6.     5e   "   	MouseOver��6.     5f   "   Disabled��. 5g   &   �� .��5h   *   �� $ PART_EditableTextBox��6G   $6 0(TextElement.Foreground).(SolidColorBrush.Color)�5i   G   .g�5j   .     $ 0��6B   8   5k   B   5l   +   5m   '   5n   #   5o        5p      
EditStates��. 5q   "     Editable��. 5r   &   �� .��5s   *   T� $ (UIElement.Visibility)�6H   $ PART_EditableTextBox��5t   2   ..�5u   .   i� $ 0��6E   # Z 6R   5v   +   T� 5w   *   $ (UIElement.Visibility)�6H   $ ContentSite��6|   ..�5x   .   i� $ 0��6E   # Z 6R   5y   +   5z   '   5{   #     5|   "   
Uneditable��6.   5}      .+�5      d� $ ToggleButton��6'   8�� $��2��5�   '   $|�false��5�   '   $ Press=�5�   '    5�   '   �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+�� 5�      $ ContentSite��6+   $ False��5�   +   #��z 5�   +   #��z 5�   +   #��z��5�   +   $��3,0,23,0q�5�   +   $��Stretch=�5�   +   $
��Left=�5�   +   5�      � 5�      $ Popup��6    $��Bottom=�5�       #��z 5�       $
 True��5�       $|�False��5�       $��Slide=�5�       .��5�      � $  DropDown��6#   $
! True��5�   #   #��z" 5�   #   #��z# 5�   #   .+�5�   "   �� $	 DropDownBorder��6)   $��1q�5�   )   $
 0,0,5,5��5�   )   ��5�   &   �� 5�   *   # �  6:   ��5�   &   �� 5�   *   # �  6:   5�   #   �� 5�   "   $��4,6,4,6q�6/   $
! True��6@   .��5�   &   �� $
$ True��61   $% 	Contained=�6D   5�   #   5�      5�      �� 5�      $& PART_EditableTextBox��6"   $��Stretch=�5�   "   $��Bottom=�5�   "   $��0,0,22,0q�5�   "   #��z��5�   "   $
|�True��5�   "   $y�Hidden=�5�   "   #( z' 5�   "   5�      ) 5�      P� 5�      * �@��HasItems$+ false��66   . �5�      �� $, DropDownBorder��6%    � ��$ 95��6W   5�      P� 5�      * �@��
IsGrouping$
+ true��68   . �5�      ��  � ��$ false��6N   5�      P� 5�      $- Popup��6"   * �@�AllowsTransparency$
+ true��6S   . �5�      �� $, DropDownBorder��6%    �@��CornerRadius$ 4��6Y   �� 5�      $, DropDownBorder��6%    � ��$ 0,2,0,0q�6S   5�      5�      5�      5�      �� 5�      " ��5�      .�5�   
   ��  �@J�SnapsToDevicePixels$
 true��60   �� 5�   
    �@�OverridesDefaultStyle$
 true��62   �� 5�   
    � �� 5�      �� 5�      "  ��6"   .R�5�      �� $	 Border��6   $. 2q�5�      $
! true��5�      
������  5�        5�      CommonStates��. 5�   "     
Normal��6.     5�   "   	MouseOver��. 5�   &   �� .��5�   *   �� $ Border��6G   $0 *(Panel.Background).(SolidColorBrush.Color)�5�   1   .g�5�   .     $ 0��6B   8   6N   5�   +   5�   '   5�   #   5�        5�      SelectionStates��. 5�   "     
Unselected��6.     5�   "   Selected��. 5�   &   �� .��5�   *   �� $ Border��6G   $0 *(Panel.Background).(SolidColorBrush.Color)�5�   1   .g�5�   .     $ 0��6B   8  6N   5�   +   5�   '   5�   #     5�   "   SelectedUnfocused��. 5�   &   �� .��5�   *   �� $ Border��6G   $0 *(Panel.Background).(SolidColorBrush.Color)�5�   1   .g�5�   .     $ 0��6B   8  6N   5�   +   5�   '   5�   #   5�      ._�5�      �� 5�      5�      5�      5�      5�      