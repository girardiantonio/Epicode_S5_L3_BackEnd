using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Epicode_S5_L3_BackEnd.Models
{
    public class Articolo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Prezzo { get; set; }
        public string Descrizione { get; set; }
        public string ImmagineCopertinaUrl { get; set; }
        public string ImmaginiAggiuntiveUrls1 { get; set; }
        public string ImmaginiAggiuntiveUrls2 { get; set; }
        public bool StatoDisponibile { get; set; }
    }
}