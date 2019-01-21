﻿// Copyright (c) 2012-2018 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Threading;
using System.Threading.Tasks;
using Vlingo.Common;

namespace Vlingo.Actors.Plugin.Mailbox.AgronaMPSCArrayQueue
{
    public class ManyToOneConcurrentArrayQueueDispatcher : IRunnable, IDispatcher
    {
        private readonly Backoff backoff;
        private readonly int throttlingCount;
        private readonly AtomicBoolean closed;

        private readonly CancellationTokenSource cancellationTokenSource;
        private Task started;
        private readonly object mutex = new object();

        internal ManyToOneConcurrentArrayQueueDispatcher(
            int mailboxSize,
            long fixedBackoff,
            int throttlingCount,
            int totalSendRetries)
        {
            backoff = fixedBackoff == 0L ? new Backoff() : new Backoff(fixedBackoff);
            RequiresExecutionNotification = fixedBackoff == 0L;
            Mailbox = new ManyToOneConcurrentArrayQueueMailbox(this, mailboxSize, totalSendRetries);
            this.throttlingCount = throttlingCount;
            closed = new AtomicBoolean(false);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Close() => closed.Set(true);

        public bool IsClosed => closed.Get() || cancellationTokenSource.IsCancellationRequested;

        public void Execute(IMailbox mailbox)
        {
            cancellationTokenSource.Cancel();
        }

        public bool RequiresExecutionNotification { get; }

        public void Run()
        {
            while (!IsClosed)
            {
                if (!Deliver())
                {
                    backoff.Now();
                }
            }
        }

        public void Start()
        {
            lock (mutex)
            {
                if (started != null)
                {
                    return;
                }

                started = Task.Run(() => Run(), cancellationTokenSource.Token);
            }
        }

        internal IMailbox Mailbox { get; }

        private bool Deliver()
        {
            for (int idx = 0; idx < throttlingCount; ++idx)
            {
                var message = Mailbox.Receive();
                if (message == null)
                {
                    return idx > 0; // we delivered at least one message
                }

                if (!IsClosed)
                {
                    message.Deliver();
                }
            }
            return true;
        }
    }
}
