using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace painternya.Interfaces
{
    public interface IOffsetObserver
    {
        double OffsetX { get; set; }
        double OffsetY { get; set; }

        IObservable<Unit> OffsetChanged { get; }
    }
}
