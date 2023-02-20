namespace Diwen.Xbrl
{
    using System;

    [Flags]
    public enum InstanceOptions
    {
        None = 0,
        RemoveUnusedObjects = 1 << 0,
        CollapseDuplicateContexts = 1 << 1,
        RemoveDuplicateFacts = 1 << 2,
    }
}