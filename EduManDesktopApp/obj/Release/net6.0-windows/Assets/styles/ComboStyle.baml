   M S B A M L   `   `   `  ���� K  GEduManDesktopApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MyControl	MyControl  W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ComboBoxToggleButton  �� 
TargetType � ColumnDefinitions'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� TargetProperty �� 
TargetName8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime  ButtonBotColor �� Value  DisabledControlDarkColor  DisabledForegroundColor  DisabledBorderDarkColor	 ��Name
 �� CornerRadius  ComboboxGradientBG �� Color F�Name  DarkMasterTextCBrush   MyControl.ReadOnlyComboBox �� 
TargetType �� Property �� Value R� KeyTime  System.Windows.Visibility   Visible R� Value   Hidden d�Name �� 	ClickMode d� 	IsChecked �� Mode �� RelativeSource ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate �Name �� IsDropDownOpen � AllowsTransparency  �Name! J� SnapsToDevicePixels" � ActualWidth# �� MaxDropDownHeight  BorderMediumColor  ControlLightColor$ K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation%   DirectionalNavigation& ��Name' �� 
IsReadOnly( �� 
IsReadOnly) �� Triggers* P� Property+ P� Value, �� 
TargetName- P� 
SourceName. �� Padding 	 SelectedBackgroundColor 
 SelectedUnfocusedColor�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     ucclr-namespace:MyControl  %z  &	        7  7  7  7  7  7  7  7  '      7  7   '�� H
    7 	 7 
 �� "  d�.R��  �� �� $��20 �   CommonStates��.   
Normal��  	MouseOver��. �� .���� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8     Pressed��  Disabled��. �� .���� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $* $(Shape.Fill).(SolidColorBrush.Color)�$ Arrow��.g�  $ 0��8  �� $P J(Border.BorderBrush).(GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    CheckStates��.   Checked��. �� .���� $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    	Unchecked��  Indeterminate��.+��� $	 Border��$��2��$
 5��8�� $��0q����� $ 	LightBlue���� $��0��$
 5,0,0,5��8�� $��01q�F� $ Arrow��$��1��$��Center=�$��Center=�"����        � 	=  	= � z     	���� 8  �� "  .���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � ��$
 Auto=���  � ��$
 Auto=���  � ��$
 true����  � ��$	 120����  � ��$ 20����  � �� �� "  ��.R��    CommonStates��.   
Normal��  	MouseOver��  Disabled��. �� .���� $ PART_EditableTextBox��$6 0(TextElement.Foreground).(SolidColorBrush.Color)�.g�  $ 0��8     
EditStates��.   Editable��. �� .��T� $ (UIElement.Visibility)�$ PART_EditableTextBox��..�i� $ 0��# Z T� $ (UIElement.Visibility)�$ ContentSite��..�i� $ 0��# Z   
Uneditable��.+�d� $ ToggleButton��8�� $��2��$|�false��$ Press=� �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+�� $ ContentSite��$ False��#��z #��z #��z��$��3,0,23,0q�$��Stretch=�$
��Left=�� $ Popup��$��Bottom=�#��z $
 True��$|�False��$��Slide=�.��� $  DropDown��$
! True��#��z" #��z# .+��� $	 DropDownBorder��$��1q�$
 0,0,5,5������ # �  ���� # �  �� $��4,6,4,6q�$
! True��.���� $
$ True��$% 	Contained=��� $& PART_EditableTextBox��$��Stretch=�$��Bottom=�$��0,0,22,0q�#��z��$
|�True��$y�Hidden=�#( z' ) P� * �@��HasItems$+ false��. ��� $, DropDownBorder�� � ��$ 95��P� * �@��
IsGrouping$
+ true��. ���  � ��$ false��P� $- Popup��* �@�AllowsTransparency$
+ true��. ��� $, DropDownBorder�� �@��CornerRadius$ 4���� $, DropDownBorder�� � ��$ 0,2,0,0q��� " ��.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � �� �� "  ��.R��� $	 Border��$. 2q�$
! true��
������    SelectionStates��.   
Unselected��  Selected��. �� .���� $ Border��$0 *(Panel.Background).(SolidColorBrush.Color)�.g�  $ 0��8     SelectedUnfocused��. �� .���� $ Border��$0 *(Panel.Background).(SolidColorBrush.Color)�.g�  $ 0��8  ._��� 