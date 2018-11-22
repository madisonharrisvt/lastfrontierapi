using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastFrontierApi.Services
{
    public interface IPdfService
    {
        void CreateCharacterSheetById(int id, string src, string des);
    }
}
