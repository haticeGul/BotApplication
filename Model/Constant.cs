using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotApplication
{
    public class Constant
    {
        public enum SelectionSubject
        {
            CancelPolicy, CreatePolicy, CollectionRequest, EarlyClose, DeleteHypotec, SendMailNoDebt
        }

        public enum Selection
        {
            Insurance, Credit
        }
    }
}