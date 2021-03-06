﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Quantum.Utils
{
    public static class UIUtils
    {
        #region Parent

        public static DependencyObject GetVisualParent(this DependencyObject dependencyObject)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return dependencyObject.CaseType((Visual v) => VisualTreeHelper.GetParent(v)).
                                    CaseType((FrameworkContentElement f) => f.Parent).
                                    Default(o => throw new NotSupportedException($"DependencyObject.GetVisualParent() does not support type {o.GetType().Name}. \n " +
                                                                                 $"The only supported types are {typeof(Visual).Name} and {typeof(FrameworkContentElement).Name}.")).
                                    Result;
        }

        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate = null)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            predicate = predicate ?? (o => true);

            DependencyObject parent = dependencyObject.GetVisualParent();
            while(parent != null)
            {
                if(predicate(parent))
                {
                    yield return parent;
                }
                parent = parent.GetVisualParent();
            }
        }

        public static IEnumerable<TObject> GetVisualAncestorsOfType<TObject>(this DependencyObject dependencyObject) where TObject : DependencyObject
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return dependencyObject.GetVisualAncestors(o => (o is TObject)).Cast<TObject>();
        }
        
        public static DependencyObject FindVisualParent(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            predicate.AssertParameterNotNull(nameof(predicate));

            var parent = dependencyObject.GetVisualParent();
            while(parent != null)
            {
                if(predicate(parent))
                {
                    return parent;
                }
                parent = parent.GetVisualParent();
            }
            return null;
        }

        public static TObject FindVisualParentOfType<TObject>(this DependencyObject dependencyObject) where TObject : DependencyObject
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return dependencyObject.FindVisualParent(o => (o is TObject)) as TObject;
        }

        #endregion Parent
        
        #region Children

        public static DependencyObject GetVisualChild(this DependencyObject dependencyObject, int childIndex)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return VisualTreeHelper.GetChild(dependencyObject, childIndex);
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate = null)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            predicate = predicate ?? (o => true);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var visualChild = dependencyObject.GetVisualChild(i);
                if(predicate(visualChild))
                {
                    yield return dependencyObject.GetVisualChild(i);
                }
            }
        }

        public static IEnumerable<TChild> GetVisualChildrenOfType<TChild>(this DependencyObject dependencyObject) where TChild : DependencyObject
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return dependencyObject.GetVisualChildren(o => (o is TChild)).Cast<TChild>();
        }
        
        #endregion Children

        #region Descendants
        
        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject dependencyObject, Predicate<DependencyObject> predicate = null)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            predicate = predicate ?? (o => true);

            var result = new List<DependencyObject>();
            foreach (var child in dependencyObject.GetVisualChildren())
            {
                if (predicate(child)) result.Add(child);
                result.AddRange(child.GetVisualDescendants(predicate));
            }

            return result;
        }

        public static IEnumerable<TChild> GetVisualDescendantsOfType<TChild>(this DependencyObject dependencyObject) where TChild : DependencyObject
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            return dependencyObject.GetVisualDescendants(o => (o is TChild)).Cast<TChild>();
        }

        #endregion Descendants
        
        #region Element

        public static bool IsVisualChildOf(this DependencyObject dependencyObject, DependencyObject ancestor)
        {
            dependencyObject.AssertNotNull(nameof(dependencyObject));
            ancestor.AssertParameterNotNull(nameof(ancestor));

            var parent = dependencyObject;
            while(true) {
                parent = parent.GetVisualParent();

                if (parent == ancestor) {
                    return true;
                }

                if(parent == null) {
                    return false;
                }
            }
        }

        #endregion Element
        
        #region GetSetValue

        public static void CheckedSetValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, object value)
        {
            dependencyProperty.AssertParameterNotNull("dependencyProperty");
            dependencyObject.
               AssertNotNull(dependencyProperty.Name).
               SetValue(dependencyProperty, value);
        }

        public static object CheckedGetValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            dependencyProperty.AssertParameterNotNull("dependencyProperty");
            return dependencyObject.
               AssertNotNull(dependencyProperty.Name).
               GetValue(dependencyProperty);
        }

        #endregion GetSetValue

        #region Misc

        /// <summary>
        /// Ensures the specified FrameworkElement is loaded before executing the specified action. <para/>
        /// If the element is loaded, the action will be executed right away. Otherwise, the execution will be delayed until the element is loaded.
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="action"></param>
        public static void EnsureLoaded(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.AssertParameterNotNull(nameof(FrameworkElement));
            action.AssertParameterNotNull(nameof(action));

            if(frameworkElement.IsLoaded)
            {
                action();
            }

            else
            {
                void loadHandler(object sender, RoutedEventArgs e)
                {
                    action();
                    frameworkElement.Loaded -= loadHandler;
                }

                frameworkElement.Loaded += loadHandler;
            }
        }


        /// <summary>
        /// Ensures the specified FrameworkElement is unloaded before executing the specified action. <para/>
        /// If the element is unloaded, the action will be executed right away. Otherwise, the execution will be delayed until the element is unloaded.
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="action"></param>
        public static void EnsureUnloaded(this FrameworkElement frameworkElement, Action action)
        {
            frameworkElement.AssertParameterNotNull(nameof(frameworkElement));
            action.AssertParameterNotNull(nameof(action));

            if(!frameworkElement.IsLoaded)
            {
                action();
            }
            
            else
            {
                void unloadHandler(object sender, RoutedEventArgs e)
                {
                    action();
                    frameworkElement.Unloaded -= unloadHandler;
                }

                frameworkElement.Unloaded += unloadHandler;
            }
        }

        #endregion

    }
}
