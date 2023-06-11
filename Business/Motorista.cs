using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Motorista : Base
    {
        [OpcoesBase(ChavePrimaria=true, UsaBD=true, UsaBusca=true)]
        public int ID { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string Nome { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string Cpf { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string RG { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string Cnh { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string Celular { get; set; }

        [OpcoesBase(UsaBD = true)]
        public string Endereco { get; set; }

    }
}
