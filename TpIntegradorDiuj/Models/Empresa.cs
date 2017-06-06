﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpIntegradorDiuj.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public int Nombre { get; set; }
        public virtual List<Balance> Balances { get; set; }
        public virtual List<Indicador> Indicadores { get; set; }
    }
}