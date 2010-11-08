/*
 * DotNetChannel: http://dnchannel.blogspot.com
 * This solution part of post: http://dnchannel.blogspot.com/2010/01/silverlight-3-auto-select-text-in.html
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;


namespace Coding4Fun.CurrencyExchange.Helpers
{
    public class AutoSelect
    {
        /// <summary>
        /// AutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSelectTextProperty = DependencyProperty.RegisterAttached("AutoSelectText",
                                                                               typeof(Boolean),
                                                                               typeof(AutoSelect),
                                                                               new PropertyMetadata(OnAutoSelectTextChanged));

        /// <summary>
        /// PreventAutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty PreventAutoSelectTextProperty = DependencyProperty.RegisterAttached("PreventAutoSelectText",
                                                                              typeof(Boolean),
                                                                              typeof(AutoSelect),
                                                                              null);

        /// <summary>
        /// This method is called when the value of the AutoSelectText property
        /// is set from the xaml
        /// </summary>
        /// <param name="d">The content control on which the property is set.</param>
        /// <param name="e"></param>
        private static void OnAutoSelectTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //This works because of event bubbling. 
            FrameworkElement frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                if ((bool)e.NewValue)
                    frameworkElement.GotFocus += OnGotFocus;
                else
                    frameworkElement.GotFocus -= OnGotFocus;
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            //Since we are using routed events, the sender parameter will not be the textbox that currently has focus. 
            //It will the root level content control (Grid) which has the AutoSelectText attached property.
            //The FocusManager class is used to get a reference to the control that has the focus.
            TextBox textBox = FocusManager.GetFocusedElement() as TextBox;
            if (textBox != null && !(bool)textBox.GetValue(PreventAutoSelectTextProperty))
                textBox.SelectAll();
        }


        #region Dependency property Get/Set
        public static Boolean GetAutoSelectText(DependencyObject target)
        {
            return (Boolean)target.GetValue(AutoSelectTextProperty);
        }
        public static void SetAutoSelectText(DependencyObject target, Boolean value)
        {
            target.SetValue(AutoSelectTextProperty, value);
        }
        public static Boolean GetPreventAutoSelectText(DependencyObject target)
        {
            return (Boolean)target.GetValue(PreventAutoSelectTextProperty);
        }
        public static void SetPreventAutoSelectText(DependencyObject target, Boolean value)
        {
            target.SetValue(PreventAutoSelectTextProperty, value);
        }
        #endregion
    }
}