﻿using System;

namespace Vlingo.Actors
{
    public class Cancellable__Proxy : ICancellable
    {
        private readonly Actor actor;
        private readonly IMailbox mailbox;

        public Cancellable__Proxy(Actor actor, IMailbox mailbox)
        {
            this.actor = actor;
            this.mailbox = mailbox;
        }
        public bool Cancel()
        {
            if (!actor.IsStopped)
            {
                Action<ICancellable> consumer = actor => actor.Cancel();
                mailbox.Send(new LocalMessage<ICancellable>(actor, consumer, "Cancel()"));
                return true;
            }

            actor.DeadLetters.FailedDelivery(new DeadLetter(actor, "Cancel()"));
            return false;
        }
    }
}
