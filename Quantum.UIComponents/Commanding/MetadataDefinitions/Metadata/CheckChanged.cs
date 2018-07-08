﻿using System;

namespace Quantum.Command
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class CheckChanged : IMenuMetadata, ISubMenuMetadata
    {
        public Action<bool> OnCheckChanged { get; private set; }
        public CheckChanged(Action<bool> onCheckChanged)
        {
            OnCheckChanged = onCheckChanged;
        }
    }
}
