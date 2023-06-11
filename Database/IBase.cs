using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public interface IBase
    {
        int Key { get; } //método que verifica se é uma chave primaria, se for criara uma chave primaria no banco de dados, somente retornara informação. 
        void Salvar(); //método que irá salvar os registros no banco de dados.
        void Excluir(); //método que irá excluir registros no banco de dados.
        void CriarTabela(); //método que criara tabelas no banco de dados.
        List<IBase> Todos(); //retornara IBase tudo que implementar através dela recebera uma generalização do tipos de classes. 
        List<IBase> Buscar();
    }
}
