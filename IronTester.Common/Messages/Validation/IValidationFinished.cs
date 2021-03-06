﻿using System;
using NServiceBus;

namespace IronTester.Common.Messages.Validation
{
    public interface IValidationFinished : IEvent
    {
        Guid RequestId { get; set; }
        bool ValidationSuccessful { get; set; }
        string ValidationFailReason { get; set; }
    }
}
