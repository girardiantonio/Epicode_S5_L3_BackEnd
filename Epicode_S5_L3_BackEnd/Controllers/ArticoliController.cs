using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epicode_S5_L3_BackEnd.Models;

namespace Epicode_S5_L3_BackEnd.Controllers
{
    public class ArticoliController : Controller
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DB_ConnString"].ConnectionString;
        }

        [HttpGet]
        public ActionResult Vetrina()
        {
            List<Articolo> elencoArticoliDisponibili = GetArticoliDisponibiliFromDatabase();
            return View(elencoArticoliDisponibili);
        }

        [HttpGet]
        public ActionResult Gestione()
        {
            List<Articolo> elencoArticoli = GetArticoliFromDatabase();
            return View(elencoArticoli);
        }

        [HttpPost]
        public ActionResult Elimina(int id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Articoli WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Gestione");
        }

        [HttpGet]
        public ActionResult AggiungiArticolo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AggiungiArticolo(Articolo model)
        {
            if (ModelState.IsValid)
            {
                string query = "INSERT INTO Articoli (Nome, Prezzo, Descrizione, ImmagineCopertinaUrl, ImmaginiAggiuntiveUrls1, ImmaginiAggiuntiveUrls2, StatoDisponibile) " +
                                "VALUES (@Nome, @Prezzo, @Descrizione, @ImmagineCopertinaUrl, @ImmaginiAggiuntiveUrls1, @ImmaginiAggiuntiveUrls2, @StatoDisponibile)";

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nome", model.Nome);
                        cmd.Parameters.AddWithValue("@Prezzo", model.Prezzo);
                        cmd.Parameters.AddWithValue("@Descrizione", model.Descrizione);
                        cmd.Parameters.AddWithValue("@ImmagineCopertinaUrl", model.ImmagineCopertinaUrl);
                        cmd.Parameters.AddWithValue("@ImmaginiAggiuntiveUrls1", model.ImmaginiAggiuntiveUrls1);
                        cmd.Parameters.AddWithValue("@ImmaginiAggiuntiveUrls2", model.ImmaginiAggiuntiveUrls2);
                        cmd.Parameters.AddWithValue("@StatoDisponibile", model.StatoDisponibile);

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Vetrina");
            }
            ViewBag.Error = "Il modello non è valido. Correggi gli errori e riprova.";
            return View();
        }

        [HttpGet]
        public ActionResult DettaglioUtente(int id)
        {
            var prodotto = GetArticoloById(id);

            if (prodotto == null)
            {
                return HttpNotFound();
            }

            return View(prodotto);
        }

        [HttpGet]
        public ActionResult DettaglioAdmin(int id)
        {
            var prodotto = GetArticoloById(id);

            if (prodotto == null)
            {
                return HttpNotFound();
            }

            return View(prodotto);
        }

        [HttpPost]
        public ActionResult SalvaModifiche(Articolo prodottoModificato)
        {
            if (ModelState.IsValid)
            {
                string query = "UPDATE Articoli SET Nome = @Nome, Prezzo = @Prezzo, Descrizione = @Descrizione, " +
                                "ImmagineCopertinaUrl = @ImmagineCopertinaUrl, ImmaginiAggiuntiveUrls1 = @ImmaginiAggiuntiveUrls1, " +
                                "ImmaginiAggiuntiveUrls2 = @ImmaginiAggiuntiveUrls2, StatoDisponibile = @StatoDisponibile " +
                                "WHERE Id = @Id";

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", prodottoModificato.Id);
                        cmd.Parameters.AddWithValue("@Nome", prodottoModificato.Nome);
                        cmd.Parameters.AddWithValue("@Prezzo", prodottoModificato.Prezzo);
                        cmd.Parameters.AddWithValue("@Descrizione", prodottoModificato.Descrizione);
                        cmd.Parameters.AddWithValue("@ImmagineCopertinaUrl", prodottoModificato.ImmagineCopertinaUrl);
                        cmd.Parameters.AddWithValue("@ImmaginiAggiuntiveUrls1", prodottoModificato.ImmaginiAggiuntiveUrls1);
                        cmd.Parameters.AddWithValue("@ImmaginiAggiuntiveUrls2", prodottoModificato.ImmaginiAggiuntiveUrls2);
                        cmd.Parameters.AddWithValue("@StatoDisponibile", prodottoModificato.StatoDisponibile);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("DettaglioAdmin", new { id = prodottoModificato.Id });
            }

            return View("DettaglioAdmin", prodottoModificato);
        }

        private List<Articolo> GetArticoliDisponibiliFromDatabase()
        {
            List<Articolo> articoli = new List<Articolo>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Id, Nome, Prezzo, ImmagineCopertinaUrl FROM Articoli WHERE StatoDisponibile = 1";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Articolo articolo = new Articolo
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                                ImmagineCopertinaUrl = reader["ImmagineCopertinaUrl"].ToString()
                            };
                            articoli.Add(articolo);
                        }
                    }
                }
            }

            return articoli;
        }

        private List<Articolo> GetArticoliFromDatabase()
        {
            List<Articolo> articoli = new List<Articolo>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Articoli";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Articolo articolo = new Articolo
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                                Descrizione = reader["Descrizione"].ToString(),
                                ImmagineCopertinaUrl = reader["ImmagineCopertinaUrl"].ToString(),
                                ImmaginiAggiuntiveUrls1 = reader["ImmaginiAggiuntiveUrls1"].ToString(),
                                ImmaginiAggiuntiveUrls2 = reader["ImmaginiAggiuntiveUrls2"].ToString(),
                                StatoDisponibile = Convert.ToBoolean(reader["StatoDisponibile"])
                            };
                            articoli.Add(articolo);
                        }
                    }
                }
            }

            return articoli;
        }

        private Articolo GetArticoloById(int id)
        {
            Articolo articolo = null;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Articoli WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            articolo = new Articolo
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                                Descrizione = reader["Descrizione"].ToString(),
                                ImmagineCopertinaUrl = reader["ImmagineCopertinaUrl"].ToString(),
                                ImmaginiAggiuntiveUrls1 = reader["ImmaginiAggiuntiveUrls1"].ToString(),
                                ImmaginiAggiuntiveUrls2 = reader["ImmaginiAggiuntiveUrls2"].ToString(),
                                StatoDisponibile = Convert.ToBoolean(reader["StatoDisponibile"])
                            };
                        }
                    }
                }
            }

            return articolo;
        }
    }
}