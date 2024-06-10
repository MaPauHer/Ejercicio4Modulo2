using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio4Modulo2
{
    internal class RegInputVenta
    {
        //|Posicion	|	Nombre del Campo     | Tipo de dato  |
        //|---------|------------------------|-------------- |
        //|     1   | Fecha del informe      | Fecha(10)     |
        //|	   11	| Codigo del vendedor    | varchar(3)    |
        //|    14   | Venta                  | numerico(11)  |
        //|    25   |Venta a empresa grande  | varchar(1) => Flag / mapearlo como true o false |

        public DateTime FechaInforme { get; set; }
        public string CodigoVendedor { get; set; }
        public decimal Venta { get; set; }
        public bool VtaEmpresaGrande { get; set; }

    }
}
