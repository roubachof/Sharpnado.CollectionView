using System;
using System.Collections.Generic;

namespace DragAndDropSample.Services
{
    public enum ErrorType
    {
        None = 0,
        Unknown = 1,
        Network = 2,
        Server = 3,
        NoData = 4,
    }

    public class ErrorEmulator
    {
        public ErrorType ErrorType { get; set; }

        public TimeSpan DefaultLoadingTime => TimeSpan.FromSeconds(2);
    }
}
