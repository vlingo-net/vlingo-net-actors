﻿namespace Vlingo.Actors
{
    // TODO: implement as a thread
    public interface IMailbox
    {
        void Close();
        bool IsClosed { get; }
        bool IsDelivering { get; }
        bool Delivering(bool flag);
        void Send(IMessage message);
        IMessage Receive();
    }
}