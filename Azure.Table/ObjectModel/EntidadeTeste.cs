using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Table.ObjectModel;

namespace Azure.Table.ObjectModel
{
    public class EntidadeTeste : TableEntityBase
    {
        public string Identificador { get; set; }
        public DateTime Data { get; set; }
        public EntidadeRelacional ListaEntidadesRelacionais { get; set; }
    }

    public class EntidadeRelacional
    {
        public string Nome { get; set; }
        public string SobreNome { get; set; }
    }

    public class OutraEntidade : TableEntityBase
    {
        public string OutroAtributo { get; set; }
    }
}
