﻿using System;

namespace Vlingo.Actors
{
    public class Stowage
    {
        public bool IsDispersing { get; }
        public bool IsStowing { get; set; }

        internal IMessage Head { get; set; }

        public void DispersingMode()
        {
            throw new System.NotImplementedException();
        }

        public void StowingMode()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        internal void Stow(IMessage message)
        {
            throw new NotImplementedException();
        }

        internal IMessage SwapWith(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}