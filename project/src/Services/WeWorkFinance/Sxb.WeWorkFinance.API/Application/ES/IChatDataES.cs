using Sxb.WeWorkFinance.API.Application.Models;
using System.Collections.Generic;

namespace Sxb.WeWorkFinance.API.Application.ES
{
    public interface IChatDataES
    {
        void ImportChatData(List<MessageModel> messages);
    }
}
