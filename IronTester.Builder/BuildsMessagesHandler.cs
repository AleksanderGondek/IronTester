﻿using System;
using IronTester.Common.Messages.Builds;
using IronTester.Common.Messages.CancelRequest;
using NServiceBus;

namespace IronTester.Builder
{
    public class BuildsMessagesHandler : IHandleMessages<IPleaseBuild>, IHandleMessages<IPleaseCancel>
    {
        public IBus Bus { get; set; }

        public void Handle(IPleaseBuild message)
        {
            var willHandleRequest = !BuildsWorker.Requests.ContainsKey(message.RequestId);

            if (willHandleRequest)
            {
                BuildsWorker.Requests.TryAdd(message.RequestId,
                    new RequestModel()
                    {
                        SourceCodeLocation = message.SourceCodeLocation,
                        WasProcessed = false,
                        IsValid = false,
                        ValidationFailReason = null
                    });
            }

            Bus.Publish<IBuildsRequestConfirmation>(x =>
            {
                x.RequestId = message.RequestId;
                x.WillBuild = willHandleRequest;
                x.DenialReason = willHandleRequest ? null : "Request already exists within builder!";
            });
        }

        public void Handle(IPleaseCancel message)
        {
            if (!BuildsWorker.Requests.ContainsKey(message.RequestId)) return;
            RequestModel removedItem;
            BuildsWorker.Requests.TryRemove(message.RequestId, out removedItem);
        }
    }
}
