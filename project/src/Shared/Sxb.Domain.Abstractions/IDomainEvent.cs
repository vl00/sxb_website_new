﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
namespace Sxb.Domain
{
    public interface IDomainEvent : INotification
    {
    }
}
