
using System.Windows;
using System.Windows.Controls;

namespace MyControl
{
    class ReadOnlyDatePicker : DatePicker
    {
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
                          name: "IsReadOnly",
                          propertyType: typeof(bool),
                          ownerType: typeof(ReadOnlyDatePicker),
                          typeMetadata: new FrameworkPropertyMetadata(
                                          defaultValue: false,
                                          flags: FrameworkPropertyMetadataOptions.AffectsRender,
                                          propertyChangedCallback: new PropertyChangedCallback(OnReadOnlyChanged)));

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) //do what when property changed
        {
            ((ReadOnlyDatePicker)d).IsHitTestVisible = !(bool)e.NewValue;
            ((ReadOnlyDatePicker)d).Focusable = !(bool)e.NewValue;
        }

        // Declare a read-write property wrapper.
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
    }

    class ReadOnlyRadioButon : RadioButton
    {
        public ReadOnlyRadioButon()
        {

        }
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
                          name: "IsReadOnly",
                          propertyType: typeof(bool),
                          ownerType: typeof(ReadOnlyRadioButon),
                          typeMetadata: new FrameworkPropertyMetadata(
                                          defaultValue: false,
                                          flags: FrameworkPropertyMetadataOptions.AffectsRender,
                                          propertyChangedCallback: new PropertyChangedCallback(OnReadOnlyChanged)));

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) //do what when property changed
        {
            ((ReadOnlyRadioButon)d).IsHitTestVisible = !(bool)e.NewValue;
            ((ReadOnlyRadioButon)d).Focusable = !(bool)e.NewValue;
        }

        // Declare a read-write property wrapper.
        public bool IsReadOnly // this will be shown at design-time
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
    }
    class ReadOnlyComboBox : ComboBox
    {
        static ReadOnlyComboBox()
        {
            IsDropDownOpenProperty.OverrideMetadata(typeof(ReadOnlyComboBox), new FrameworkPropertyMetadata(
                propertyChangedCallback: delegate
                { },
                coerceValueCallback: (d, value) =>
                {
                    if (((ReadOnlyComboBox)d).IsReadOnly)
                    {
                        // Prohibit opening the drop down when read only.
                        return false;
                    }

                    return value;
                }));

            IsReadOnlyProperty.OverrideMetadata(typeof(ReadOnlyComboBox), new FrameworkPropertyMetadata(
                propertyChangedCallback: (d, e) =>
                {
                    // When setting "read only" to false, close the drop down.
                    if (e.NewValue is true)
                    {
                        ((ReadOnlyComboBox)d).IsDropDownOpen = false;
                    }
                }));
        }
    }
}
