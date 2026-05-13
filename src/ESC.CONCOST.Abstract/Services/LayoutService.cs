using Microsoft.AspNetCore.Components;
using System;

namespace ESC.CONCOST.Abstract;

public class LayoutService
{
    private RenderFragment? _sidebarHeader;
    public RenderFragment? SidebarHeader
    {
        get => _sidebarHeader;
        set
        {
            if (_sidebarHeader != value)
            {
                _sidebarHeader = value;
                NotifyChanged();
            }
        }
    }

    public event Action? OnNotifyChanged;

    private void NotifyChanged() => OnNotifyChanged?.Invoke();
}
