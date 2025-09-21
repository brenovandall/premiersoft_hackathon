using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class XmlFileReadController : ControllerBase
    {
        private IFileReaderEngineFactory Factory { get; set; }
        IRepository<Import, Guid> ImportRepository { get; set; }
        public XmlFileReadController(IFileReaderEngineFactory factory, IRepository<Import, Guid> importRepo)
        {
            Factory = factory;
            ImportRepository = importRepo;
        }

        /// <summary>
        /// Lê dados de um arquivo XML.
        /// </summary>
        /// <returns>Mensagem de sucesso ou erro</returns>
        [HttpGet("ler")]
        public IActionResult GetXmlData()
        {
            try
            {
                var factory = Factory.CreateFactory(".xml");
                factory.Run(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                return Ok($"Erro: {ex.Message}");
            }

            return Ok("Arquivo XML lido com sucesso.");
        }

        /// <summary>
        /// Retorna status OK.
        /// </summary>
        /// <returns>Status OK</returns>
        [HttpGet("ok")]
        public IActionResult RetornaOk()
        {
            var resposta = new RespostaOk { Status = "ok" };
            return Ok(resposta);
        }

        public class RespostaOk
        {
            public string Status { get; set; }
        }
    }
}
