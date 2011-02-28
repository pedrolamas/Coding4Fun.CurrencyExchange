//using System.Collections;
//using System.Windows;
//using System.Windows.Controls.Primitives;

//namespace Coding4Fun.CurrencyExchange.Helpers
//{
//    public static class MultiSelectorBehaviour
//    {
//        public static readonly DependencyProperty SynchronizedSelectedItemsProperty =
//            DependencyProperty.RegisterAttached("SynchronizedSelectedItems", typeof(IList), typeof(MultiSelectorBehaviour), new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

//        public static IList GetSynchronizedSelectedItems(DependencyObject obj)
//        {
//            return (IList)obj.GetValue(SynchronizedSelectedItemsProperty);
//        }

//        public static void SetSynchronizedSelectedItems(DependencyObject obj, IList value)
//        {
//            obj.SetValue(SynchronizedSelectedItemsProperty, value);
//        }

//        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
//        {
//            if (e.OldValue != null)
//            {

//            }

//            var list = e.NewValue as IList;
//            var selector = dependencyObject as Selector;

//            if (list != null && selector != null)
//            {

//            }
//        }



//    }

//    //public class ListSynchronizer : IWeakEventListener
//    //{
//    //}
//}