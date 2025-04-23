using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuApp.Entidades.Entity;

namespace TuApp.Entidades.Res.Chat
{
    public class ResCargarChatPaciente : ResBase
    {
        public List<Mensaje> chatspaciente { get; set; }
    }
}