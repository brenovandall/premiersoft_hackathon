using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Data.Extensions
{
    public class DataSeed
    {
        public static IEnumerable<Estados> States =>
        new List<Estados>
        {
            new Estados()
            {
                Codigo_uf = 11,
                Uf = "RO",
                Nome = "Rondônia",
                Latitude = -11.5052M,
                Longitude = -63.5806M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 12,
                Uf = "AC",
                Nome = "Acre",
                Latitude = -9.97499M,
                Longitude = -67.8243M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 13,
                Uf = "AM",
                Nome = "Amazonas",
                Latitude = -3.11866M,
                Longitude = -60.0212M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 14,
                Uf = "RR",
                Nome = "Roraima",
                Latitude = 2.81972M,
                Longitude = -60.6733M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 15,
                Uf = "PA",
                Nome = "Pará",
                Latitude = -1.45502M,
                Longitude = -48.5024M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 16,
                Uf = "AP",
                Nome = "Amapá",
                Latitude = 0.0389M,
                Longitude = -51.0664M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 17,
                Uf = "TO",
                Nome = "Tocantins",
                Latitude = -10.184M,
                Longitude = -48.3336M,
                Regiao = "Norte"
            },
            new Estados()
            {
                Codigo_uf = 21,
                Uf = "MA",
                Nome = "Maranhão",
                Latitude = -2.52972M,
                Longitude = -44.3028M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 22,
                Uf = "PI",
                Nome = "Piauí",
                Latitude = -5.08921M,
                Longitude = -42.8016M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 23,
                Uf = "CE",
                Nome = "Ceará",
                Latitude = -3.71722M,
                Longitude = -38.5433M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 24,
                Uf = "RN",
                Nome = "Rio Grande do Norte",
                Latitude = -5.795M,
                Longitude = -35.2094M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 25,
                Uf = "PB",
                Nome = "Paraíba",
                Latitude = -7.11509M,
                Longitude = -34.8641M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 26,
                Uf = "PE",
                Nome = "Pernambuco",
                Latitude = -8.04666M,
                Longitude = -34.8771M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 27,
                Uf = "AL",
                Nome = "Alagoas",
                Latitude = -9.66599M,
                Longitude = -35.735M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 28,
                Uf = "SE",
                Nome = "Sergipe",
                Latitude = -10.9091M,
                Longitude = -37.0677M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 29,
                Uf = "BA",
                Nome = "Bahia",
                Latitude = -12.9718M,
                Longitude = -38.5011M,
                Regiao = "Nordeste"
            },
            new Estados()
            {
                Codigo_uf = 31,
                Uf = "MG",
                Nome = "Minas Gerais",
                Latitude = -19.9191M,
                Longitude = -43.9386M,
                Regiao = "Sudeste"
            },
            new Estados()
            {
                Codigo_uf = 32,
                Uf = "ES",
                Nome = "Espírito Santo",
                Latitude = -20.3155M,
                Longitude = -40.3128M,
                Regiao = "Sudeste"
            },
            new Estados()
            {
                Codigo_uf = 33,
                Uf = "RJ",
                Nome = "Rio de Janeiro",
                Latitude = -22.9068M,
                Longitude = -43.1729M,
                Regiao = "Sudeste"
            },
            new Estados()
            {
                Codigo_uf = 35,
                Uf = "SP",
                Nome = "São Paulo",
                Latitude = -23.5505M,
                Longitude = -46.6333M,
                Regiao = "Sudeste"
            },
            new Estados()
            {
                Codigo_uf = 41,
                Uf = "PR",
                Nome = "Paraná",
                Latitude = -25.4284M,
                Longitude = -49.2733M,
                Regiao = "Sul"
            },
            new Estados()
            {
                Codigo_uf = 42,
                Uf = "SC",
                Nome = "Santa Catarina",
                Latitude = -27.5954M,
                Longitude = -48.548M,
                Regiao = "Sul"
            },
            new Estados()
            {
                Codigo_uf = 43,
                Uf = "RS",
                Nome = "Rio Grande do Sul",
                Latitude = -30.033M,
                Longitude = -51.23M,
                Regiao = "Sul"
            },
            new Estados()
            {
                Codigo_uf = 50,
                Uf = "MS",
                Nome = "Mato Grosso do Sul",
                Latitude = -20.4428M,
                Longitude = -54.6464M,
                Regiao = "Centro-Oeste"
            },
            new Estados()
            {
                Codigo_uf = 51,
                Uf = "MT",
                Nome = "Mato Grosso",
                Latitude = -15.601M,
                Longitude = -56.0974M,
                Regiao = "Centro-Oeste"
            },
            new Estados()
            {
                Codigo_uf = 52,
                Uf = "GO",
                Nome = "Goiás",
                Latitude = -16.6864M,
                Longitude = -49.2643M,
                Regiao = "Centro-Oeste"
            },
            new Estados()
            {
                Codigo_uf = 53,
                Uf = "DF",
                Nome = "Distrito Federal",
                Latitude = -15.7797M,
                Longitude = -47.9297M,
                Regiao = "Centro-Oeste"
            }
        };

        public static IEnumerable<Municipios> Cities =>
        new List<Municipios>
        {
            new Municipios()
            {
                Codigo_ibge = "1100205",
                Nome = "Porto Velho",
                Latitude = -8.76077M,
                Longitude = -63.8999M,
                Capital = true,
                Codigo_uf = "11",
                Siafi_id = 7049,
                Ddd = 69,
                Fuso_horario = "America/Porto_Velho",
                Populacao = 539354
            },
            new Municipios()
            {
                Codigo_ibge = "1200401",
                Nome = "Rio Branco",
                Latitude = -9.97499M,
                Longitude = -67.8243M,
                Capital = true,
                Codigo_uf = "12",
                Siafi_id = 0139,
                Ddd = 68,
                Fuso_horario = "America/Rio_Branco",
                Populacao = 419452
            },
            new Municipios()
            {
                Codigo_ibge = "1302603",
                Nome = "Manaus",
                Latitude = -3.11866M,
                Longitude = -60.0212M,
                Capital = true,
                Codigo_uf = "13",
                Siafi_id = 0255,
                Ddd = 92,
                Fuso_horario = "America/Manaus",
                Populacao = 2219580
            },
            new Municipios()
            {
                Codigo_ibge = "1400100",
                Nome = "Boa Vista",
                Latitude = 2.81972M,
                Longitude = -60.6733M,
                Capital = true,
                Codigo_uf = "14",
                Siafi_id = 0651,
                Ddd = 95,
                Fuso_horario = "America/Boa_Vista",
                Populacao = 445441
            },
            new Municipios()
            {
                Codigo_ibge = "1501402",
                Nome = "Belém",
                Latitude = -1.45502M,
                Longitude = -48.5024M,
                Capital = true,
                Codigo_uf = "15",
                Siafi_id = 0261,
                Ddd = 91,
                Fuso_horario = "America/Belem",
                Populacao = 1511723
            },
            new Municipios()
            {
                Codigo_ibge = "1600303",
                Nome = "Macapá",
                Latitude = 0.0389M,
                Longitude = -51.0664M,
                Capital = true,
                Codigo_uf = "16",
                Siafi_id = 0391,
                Ddd = 96,
                Fuso_horario = "America/Belem",
                Populacao = 522357
            },
            new Municipios()
            {
                Codigo_ibge = "1702109",
                Nome = "Palmas",
                Latitude = -10.184M,
                Longitude = -48.3336M,
                Capital = true,
                Codigo_uf = "17",
                Siafi_id = 0791,
                Ddd = 63,
                Fuso_horario = "America/Araguaina",
                Populacao = 335458
            },
            new Municipios()
            {
                Codigo_ibge = "2105302",
                Nome = "São Luís",
                Latitude = -2.52972M,
                Longitude = -44.3028M,
                Capital = true,
                Codigo_uf = "21",
                Siafi_id = 0985,
                Ddd = 98,
                Fuso_horario = "America/Fortaleza",
                Populacao = 1108975
            },
            new Municipios()
            {
                Codigo_ibge = "2211001",
                Nome = "Teresina",
                Latitude = -5.08921M,
                Longitude = -42.8016M,
                Capital = true,
                Codigo_uf = "22",
                Siafi_id = 1153,
                Ddd = 86,
                Fuso_horario = "America/Fortaleza",
                Populacao = 868075
            },
            new Municipios()
            {
                Codigo_ibge = "2304400",
                Nome = "Fortaleza",
                Latitude = -3.71722M,
                Longitude = -38.5433M,
                Capital = true,
                Codigo_uf = "23",
                Siafi_id = 1389,
                Ddd = 85,
                Fuso_horario = "America/Fortaleza",
                Populacao = 2686612
            },
            new Municipios()
            {
                Codigo_ibge = "1100205",
                Nome = "Porto Velho",
                Latitude = -8.76077M,
                Longitude = -63.8999M,
                Capital = true,
                Codigo_uf = "11",
                Siafi_id = 7049,
                Ddd = 69,
                Fuso_horario = "America/Porto_Velho",
                Populacao = 539354
            },
            new Municipios()
            {
                Codigo_ibge = "1200401",
                Nome = "Rio Branco",
                Latitude = -9.97499M,
                Longitude = -67.8243M,
                Capital = true,
                Codigo_uf = "12",
                Siafi_id = 0139,
                Ddd = 68,
                Fuso_horario = "America/Rio_Branco",
                Populacao = 419452
            },
            new Municipios()
            {
                Codigo_ibge = "1302603",
                Nome = "Manaus",
                Latitude = -3.11866M,
                Longitude = -60.0212M,
                Capital = true,
                Codigo_uf = "13",
                Siafi_id = 0255,
                Ddd = 92,
                Fuso_horario = "America/Manaus",
                Populacao = 2219580
            },
            new Municipios()
            {
                Codigo_ibge = "1400100",
                Nome = "Boa Vista",
                Latitude = 2.81972M,
                Longitude = -60.6733M,
                Capital = true,
                Codigo_uf = "14",
                Siafi_id = 0651,
                Ddd = 95,
                Fuso_horario = "America/Boa_Vista",
                Populacao = 445441
            },
            new Municipios()
            {
                Codigo_ibge = "1501402",
                Nome = "Belém",
                Latitude = -1.45502M,
                Longitude = -48.5024M,
                Capital = true,
                Codigo_uf = "15",
                Siafi_id = 0261,
                Ddd = 91,
                Fuso_horario = "America/Belem",
                Populacao = 1511723
            },
            new Municipios()
            {
                Codigo_ibge = "1600303",
                Nome = "Macapá",
                Latitude = 0.0389M,
                Longitude = -51.0664M,
                Capital = true,
                Codigo_uf = "16",
                Siafi_id = 0391,
                Ddd = 96,
                Fuso_horario = "America/Belem",
                Populacao = 522357
            },
            new Municipios()
            {
                Codigo_ibge = "1702109",
                Nome = "Palmas",
                Latitude = -10.184M,
                Longitude = -48.3336M,
                Capital = true,
                Codigo_uf = "17",
                Siafi_id = 0791,
                Ddd = 63,
                Fuso_horario = "America/Araguaina",
                Populacao = 335458
            },
            new Municipios()
            {
                Codigo_ibge = "2105302",
                Nome = "São Luís",
                Latitude = -2.52972M,
                Longitude = -44.3028M,
                Capital = true,
                Codigo_uf = "21",
                Siafi_id = 0985,
                Ddd = 98,
                Fuso_horario = "America/Fortaleza",
                Populacao = 1108975
            },
            new Municipios()
            {
                Codigo_ibge = "2211001",
                Nome = "Teresina",
                Latitude = -5.08921M,
                Longitude = -42.8016M,
                Capital = true,
                Codigo_uf = "22",
                Siafi_id = 1153,
                Ddd = 86,
                Fuso_horario = "America/Fortaleza",
                Populacao = 868075
            },
            new Municipios()
            {
                Codigo_ibge = "2304400",
                Nome = "Fortaleza",
                Latitude = -3.71722M,
                Longitude = -38.5433M,
                Capital = true,
                Codigo_uf = "23",
                Siafi_id = 1389,
                Ddd = 85,
                Fuso_horario = "America/Fortaleza",
                Populacao = 2686612
            },
            new Municipios()
            {
                Codigo_ibge = "2408102",
                Nome = "Natal",
                Latitude = -5.795M,
                Longitude = -35.2094M,
                Capital = true,
                Codigo_uf = "24",
                Siafi_id = 1761,
                Ddd = 84,
                Fuso_horario = "America/Fortaleza",
                Populacao = 751300
            },
            new Municipios()
            {
                Codigo_ibge = "2507507",
                Nome = "João Pessoa",
                Latitude = -7.11509M,
                Longitude = -34.8641M,
                Capital = true,
                Codigo_uf = "25",
                Siafi_id = 1961,
                Ddd = 83,
                Fuso_horario = "America/Fortaleza",
                Populacao = 833932
            },
            new Municipios()
            {
                Codigo_ibge = "2611606",
                Nome = "Recife",
                Latitude = -8.04666M,
                Longitude = -34.8771M,
                Capital = true,
                Codigo_uf = "26",
                Siafi_id = 2731,
                Ddd = 81,
                Fuso_horario = "America/Recife",
                Populacao = 1636094
            },
            new Municipios()
            {
                Codigo_ibge = "2704302",
                Nome = "Maceió",
                Latitude = -9.66599M,
                Longitude = -35.735M,
                Capital = true,
                Codigo_uf = "27",
                Siafi_id = 2901,
                Ddd = 82,
                Fuso_horario = "America/Maceio",
                Populacao = 957916
            },
            new Municipios()
            {
                Codigo_ibge = "2800308",
                Nome = "Aracaju",
                Latitude = -10.9091M,
                Longitude = -37.0677M,
                Capital = true,
                Codigo_uf = "28",
                Siafi_id = 3031,
                Ddd = 79,
                Fuso_horario = "America/Maceio",
                Populacao = 672614
            },
            new Municipios()
            {
                Codigo_ibge = "2927408",
                Nome = "Salvador",
                Latitude = -12.9718M,
                Longitude = -38.5011M,
                Capital = true,
                Codigo_uf = "29",
                Siafi_id = 3849,
                Ddd = 71,
                Fuso_horario = "America/Bahia",
                Populacao = 2886698
            },
            new Municipios()
            {
                Codigo_ibge = "3106200",
                Nome = "Belo Horizonte",
                Latitude = -19.9191M,
                Longitude = -43.9386M,
                Capital = true,
                Codigo_uf = "31",
                Siafi_id = 4123,
                Ddd = 31,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 2513451
            },
            new Municipios()
            {
                Codigo_ibge = "3205309",
                Nome = "Vitória",
                Latitude = -20.3155M,
                Longitude = -40.3128M,
                Capital = true,
                Codigo_uf = "32",
                Siafi_id = 5625,
                Ddd = 27,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 369534
            },
            new Municipios()
            {
                Codigo_ibge = "3304557",
                Nome = "Rio de Janeiro",
                Latitude = -22.9068M,
                Longitude = -43.1729M,
                Capital = true,
                Codigo_uf = "33",
                Siafi_id = 6001,
                Ddd = 21,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 6718903
            },
            new Municipios()
            {
                Codigo_ibge = "3550308",
                Nome = "São Paulo",
                Latitude = -23.5505M,
                Longitude = -46.6333M,
                Capital = true,
                Codigo_uf = "35",
                Siafi_id = 7107,
                Ddd = 11,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 11451245
            },
            new Municipios()
            {
                Codigo_ibge = "4106902",
                Nome = "Curitiba",
                Latitude = -25.4284M,
                Longitude = -49.2733M,
                Capital = true,
                Codigo_uf = "41",
                Siafi_id = 7535,
                Ddd = 41,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 1963726
            },
            new Municipios()
            {
                Codigo_ibge = "4205407",
                Nome = "Florianópolis",
                Latitude = -27.5954M,
                Longitude = -48.548M,
                Capital = true,
                Codigo_uf = "42",
                Siafi_id = 8101,
                Ddd = 48,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 516524
            },
            new Municipios()
            {
                Codigo_ibge = "4314902",
                Nome = "Porto Alegre",
                Latitude = -30.033M,
                Longitude = -51.23M,
                Capital = true,
                Codigo_uf = "43",
                Siafi_id = 8801,
                Ddd = 51,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 1332844
            },
            new Municipios()
            {
                Codigo_ibge = "5002704",
                Nome = "Campo Grande",
                Latitude = -20.4428M,
                Longitude = -54.6464M,
                Capital = true,
                Codigo_uf = "50",
                Siafi_id = 9061,
                Ddd = 67,
                Fuso_horario = "America/Campo_Grande",
                Populacao = 916001
            },
            new Municipios()
            {
                Codigo_ibge = "5103403",
                Nome = "Cuiabá",
                Latitude = -15.601M,
                Longitude = -56.0974M,
                Capital = true,
                Codigo_uf = "51",
                Siafi_id = 9181,
                Ddd = 65,
                Fuso_horario = "America/Cuiaba",
                Populacao = 626266
            },
            new Municipios()
            {
                Codigo_ibge = "5208707",
                Nome = "Goiânia",
                Latitude = -16.6864M,
                Longitude = -49.2643M,
                Capital = true,
                Codigo_uf = "52",
                Siafi_id = 9437,
                Ddd = 62,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 1536097
            },
            new Municipios()
            {
                Codigo_ibge = "5300108",
                Nome = "Brasília",
                Latitude = -15.7797M,
                Longitude = -47.9297M,
                Capital = true,
                Codigo_uf = "53",
                Siafi_id = 9701,
                Ddd = 61,
                Fuso_horario = "America/Sao_Paulo",
                Populacao = 3094325
            }
        };

        public static IEnumerable<Cid10> Cids =>
        new List<Cid10>
        {
            new Cid10()
            {
                Codigo = "A00",
                Descricao = "Cólera"
            },
            new Cid10()
            {
                Codigo = "A00",
                Descricao = "Cólera"
            },
            new Cid10()
            {
                Codigo = "A01",
                Descricao = "Febres tifóide e paratifóide"
            },
            new Cid10()
            {
                Codigo = "A02",
                Descricao = "Outras infecções por Salmonella"
            },
            new Cid10()
            {
                Codigo = "A03",
                Descricao = "Shiguelose"
            },
            new Cid10()
            {
                Codigo = "A04",
                Descricao = "Outras infecções intestinais bacterianas"
            },
            new Cid10()
            {
                Codigo = "A05",
                Descricao = "Outras intoxicações alimentares bacterianas"
            },
            new Cid10()
            {
                Codigo = "A06",
                Descricao = "Amebíase"
            },
            new Cid10()
            {
                Codigo = "A07",
                Descricao = "Outras doenças intestinais causadas por protozoários"
            },
            new Cid10()
            {
                Codigo = "A08",
                Descricao = "Infecções intestinais virais, outras e as não especificadas"
            },
            new Cid10()
            {
                Codigo = "A09",
                Descricao = "Diarréia e gastroenterite de origem infecciosa presumível"
            },
            new Cid10()
            {
                Codigo = "B20",
                Descricao = "Doença pelo vírus da imunodeficiência humana [HIV], resultando em doenças infecciosas e parasitárias"
            },
            new Cid10()
            {
                Codigo = "C34",
                Descricao = "Neoplasia maligna dos brônquios e do pulmão"
            },
            new Cid10()
            {
                Codigo = "E10",
                Descricao = "Diabetes mellitus insulino-dependente"
            },
            new Cid10()
            {
                Codigo = "E11",
                Descricao = "Diabetes mellitus não insulino-dependente"
            },
            new Cid10()
            {
                Codigo = "F32",
                Descricao = "Episódios depressivos"
            },
            new Cid10()
            {
                Codigo = "F41",
                Descricao = "Outros transtornos ansiosos"
            },
            new Cid10()
            {
                Codigo = "G40",
                Descricao = "Epilepsia"
            },
            new Cid10()
            {
                Codigo = "I10",
                Descricao = "Hipertensão essencial (primária)"
            },
            new Cid10()
            {
                Codigo = "I21",
                Descricao = "Infarto agudo do miocárdio"
            },
            new Cid10()
            {
                Codigo = "J45",
                Descricao = "Asma"
            },
            new Cid10()
            {
                Codigo = "K35",
                Descricao = "Apendicite aguda"
            },
            new Cid10()
            {
                Codigo = "N39",
                Descricao = "Outros transtornos do trato urinário"
            },
            new Cid10()
            {
                Codigo = "O80",
                Descricao = "Parto único espontâneo"
            },
            new Cid10()
            {
                Codigo = "Z00",
                Descricao = "Exame geral e investigação de pessoas sem queixas ou diagnóstico relatado"
            }
        };

        public static IEnumerable<Hospitais> Hospitais =>
        new List<Hospitais>
        {
            new Hospitais()
            {
                Codigo = "1b2c137e-75e1-4644-b1ca-c04e1055443a",
                Nome = "Hospital Municipal Santo Antônio",
                Bairro = "Jardim",
                Especialidades = "Neurologia;Ortopedia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "4309654"),
                Leitos_totais = 335
            },
            new Hospitais()
            {
                Codigo = "c794ca06-1e7a-4012-99e1-1642d336232e",
                Nome = "Hospital Santa Santo Antônio",
                Bairro = "Centro",
                Especialidades = "Infectologia;Clínica Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1505304"),
                Leitos_totais = 794
            },
            new Hospitais()
            {
                Codigo = "76886037-2645-41b2-96fb-ca7b6bb13465",
                Nome = "Hospital Instituto Vida",
                Bairro = "Parque",
                Especialidades = "Ginecologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1501303"),
                Leitos_totais = 140
            },
            new Hospitais()
            {
                Codigo = "06f01236-efdb-4ac0-b9a1-ab544b5ff11f",
                Nome = "Hospital Regional das Clínicas",
                Bairro = "Parque",
                Especialidades = "Cirurgia Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "2918605"),
                Leitos_totais = 489
            },
            new Hospitais()
            {
                Codigo = "58eaab6a-8210-4213-b6dc-45e23f0d9aaa",
                Nome = "Hospital Central",
                Bairro = "Centro",
                Especialidades = "Cirurgia Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "2801504"),
                Leitos_totais = 421
            },
            new Hospitais()
            {
                Codigo = "73fb448e-1379-4639-8fde-f396a27f3cd3",
                Nome = "Hospital Santa de Saúde",
                Bairro = "Lago",
                Especialidades = "Endocrinologia;Pediatria",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5001243"),
                Leitos_totais = 193
            },
            new Hospitais()
            {
                Codigo = "8626a104-6e27-46c2-824a-4464d554664f",
                Nome = "Hospital Regional São Lucas",
                Bairro = "Recanto",
                Especialidades = "Urologia;Oncologia;Oftalmologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5219001"),
                Leitos_totais = 430
            },
            new Hospitais()
            {
                Codigo = "8d5dac2f-4eb5-4bba-8a33-1175371634de",
                Nome = "Hospital Universitário São Lucas",
                Bairro = "Montanha",
                Especialidades = "Cirurgia Geral;Dermatologia;Radiologia;Oftalmologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "2301000"),
                Leitos_totais = 639
            },
            new Hospitais()
            {
                Codigo = "9e54dc6a-5b26-4580-bd29-40b5836b202a",
                Nome = "Hospital São Esperança",
                Bairro = "Centro",
                Especialidades = "Dermatologia;Pediatria;Ortopedia;Endocrinologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5102793"),
                Leitos_totais = 816
            },
            new Hospitais()
            {
                Codigo = "312134e8-d503-4b01-a990-a6399eee9fe6",
                Nome = "Hospital São Lucas",
                Bairro = "Recanto",
                Especialidades = "Neurologia;Psiquiatria;Urologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "4312385"),
                Leitos_totais = 112
            },
            new Hospitais()
            {
                Codigo = "ad4c39f2-40e3-41c8-9abb-8b24602f29c5",
                Nome = "Hospital Universitário São Lucas",
                Bairro = "Nossa Senhora",
                Especialidades = "Nefrologia;Pediatria;Cirurgia Geral;Clínica Geral;Cardiologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "3303401"),
                Leitos_totais = 1049
            },
            new Hospitais()
            {
                Codigo = "3c5ba594-c75c-4ef2-8e2a-71883b77e3b1",
                Nome = "Hospital Santa Bom Pastor",
                Bairro = "Nossa Senhora",
                Especialidades = "Radiologia;Dermatologia;Nefrologia;Anestesiologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1101302"),
                Leitos_totais = 1056
            },
            new Hospitais()
            {
                Codigo = "7f0a1b82-468e-4994-b111-36673989356b",
                Nome = "Hospital Regional das Clínicas",
                Bairro = "Bosque",
                Especialidades = "Anestesiologia;Ginecologia;Psiquiatria",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "2201572"),
                Leitos_totais = 834
            },
            new Hospitais()
            {
                Codigo = "8950d115-ac7c-4a55-b044-493fe222806b",
                Nome = "Hospital Santa Vida",
                Bairro = "Vila",
                Especialidades = "Oncologia;Neurologia;Ginecologia;Cirurgia Geral;Pediatria",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "3551009"),
                Leitos_totais = 1110
            },
            new Hospitais()
            {
                Codigo = "73e0b2b9-e92b-4a5e-8580-31c121d4280f",
                Nome = "Hospital Santa Esperança",
                Bairro = "Recanto",
                Especialidades = "Psiquiatria;Cardiologia;Ortopedia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5105507"),
                Leitos_totais = 519
            },
            new Hospitais()
            {
                Codigo = "c7fbda49-2e23-47b4-8b32-7d1f0f757e1f",
                Nome = "Hospital Clínica São Lucas",
                Bairro = "Recanto",
                Especialidades = "Nefrologia;Ginecologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "4208906"),
                Leitos_totais = 988
            },
            new Hospitais()
            {
                Codigo = "05a897dc-0bd0-498c-8c09-21d4ac9e1f3e",
                Nome = "Hospital Universitário Vida",
                Bairro = "Montanha",
                Especialidades = "Urologia;Radiologia;Nefrologia;Dermatologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5300108"),
                Leitos_totais = 1416
            },
            new Hospitais()
            {
                Codigo = "a0a4ffb2-0a5d-492e-a8b8-334f8dfc805a",
                Nome = "Hospital Instituto Central",
                Bairro = "Bela Vista",
                Especialidades = "Psiquiatria;Clínica Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1600279"),
                Leitos_totais = 679
            },
            new Hospitais()
            {
                Codigo = "fe1d1025-8dad-4c12-a6af-1a1f4b8efe32",
                Nome = "Hospital Regional das Clínicas",
                Bairro = "Jardim",
                Especialidades = "Psiquiatria;Dermatologia;Oftalmologia;Infectologia;Ortopedia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "3203106"),
                Leitos_totais = 223
            },
            new Hospitais()
            {
                Codigo = "5b17c45c-d83a-46c4-ae07-d6f9a27a0888",
                Nome = "Hospital Centro Médico Santo Antônio",
                Bairro = "Parque",
                Especialidades = "Ortopedia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "4125555"),
                Leitos_totais = 674
            },
            new Hospitais()
            {
                Codigo = "8a2c29fd-35ca-4b2f-9e44-bbf94ec4cea6",
                Nome = "Hospital São das Clínicas",
                Bairro = "Recanto",
                Especialidades = "Cirurgia Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1505494"),
                Leitos_totais = 1083
            },
            new Hospitais()
            {
                Codigo = "8038991d-2bd8-4e20-8477-1d4092a17a0c",
                Nome = "Hospital São Lucas",
                Bairro = "Parque",
                Especialidades = "Ginecologia;Dermatologia;Psiquiatria;Ortopedia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5300108"),
                Leitos_totais = 1245
            },
            new Hospitais()
            {
                Codigo = "b3815312-2315-40ba-b08f-b88585c769ac",
                Nome = "Hospital Clínica de Saúde",
                Bairro = "Centro",
                Especialidades = "Psiquiatria",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1302702"),
                Leitos_totais = 543
            },
            new Hospitais()
            {
                Codigo = "1bdc9f1e-00ba-4a00-a470-52ce04c7d518",
                Nome = "Hospital Nossa Senhora Bom Pastor",
                Bairro = "Nossa Senhora",
                Especialidades = "Cirurgia Geral",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "5300108"),
                Leitos_totais = 1913
            },
            new Hospitais()
            {
                Codigo = "0002a084-b162-494b-a7c8-565bf2a1e453",
                Nome = "Hospital São Lucas",
                Bairro = "Montanha",
                Especialidades = "Endocrinologia;Oncologia",
                Municipio = Cities.FirstOrDefault(m => m.Codigo_ibge == "1400209"),
                Leitos_totais = 900
            },
        };
    }
}
