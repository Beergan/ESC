using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract
{
    public interface IMailSettingService
    {
        Task SendMail(params MailRequest[] mails);
    }
}
