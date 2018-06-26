﻿using System.Collections.ObjectModel;

namespace Quantum.Services
{
    public abstract class MultipleSelection<T> : SelectionBase<ObservableCollection<T>>
    {
        public MultipleSelection(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public MultipleSelection(IObjectInitializationService initSvc, ObservableCollection<T> defaultValue, bool raiseOnDefaultValueSet = false)
            : base(initSvc, defaultValue, raiseOnDefaultValueSet)
        {
        }

        protected override void OnSelectedValueChanged()
        {
            Value.CollectionChanged += (sender, e) => Raise();
        }
    }
}
