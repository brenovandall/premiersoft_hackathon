namespace Hackathon.Premiersoft.API.Engines.DataProcess.Helpers
{
    public static class DataHelper
    {
        private static string TratarCampo(string campo)
        {
            return campo.Trim().ToUpper();
        }

        public static bool CheckCampo(string campoArquivo, string campoBanco)
        {
            var campoArquivoTratado = TratarCampo(campoArquivo);

            var campoBancoTratado = TratarCampo(campoBanco);

            return campoArquivoTratado == campoBancoTratado;
        }

    }
}
