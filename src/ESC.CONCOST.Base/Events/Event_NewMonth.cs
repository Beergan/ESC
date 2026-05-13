using System;
using MediatR;

namespace ESC.CONCOST.Base;

public class Event_NewMonth : INotification
{
    public DateTime Date { get; set; }
}