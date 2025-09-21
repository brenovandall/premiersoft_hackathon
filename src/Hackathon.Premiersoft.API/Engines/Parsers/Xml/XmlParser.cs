using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Xml
{
    public class XmlParser : IXmlParser
    {
        private readonly S3Service _s3Service;

        public XmlParser(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public async Task ParseXmlAsync(Import import)
        {
            try
            {
                Console.WriteLine($"Parsing arquivo XML: {import.FileName}");
                
                if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                    throw new Exception("URL do arquivo não encontrado!");

                using var reader = await _s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);
                var xmlMappedData = await ParseXmlDataAsync(reader);

                if (xmlMappedData != null && xmlMappedData.Records.Any())
                {
                    Console.WriteLine($"XML processado com sucesso: {xmlMappedData.Records.Count} registros encontrados");
                    Console.WriteLine($"Elementos mapeados: {string.Join(", ", xmlMappedData.Elements.Select(e => e.Name))}");
                    
                    // Aqui você pode implementar a lógica de salvamento dos dados mapeados
                    // similar ao que é feito no CSV
                }
                else
                {
                    Console.WriteLine("Nenhum registro válido encontrado no arquivo XML");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer parse do arquivo XML: {ex.Message}");
                throw;
            }
        }

        public async Task<XmlMappedData> ParseXmlDataAsync(TextReader reader)
        {
            var elements = new List<XmlElementInfo>();
            var records = new List<XmlRecord>();
            var namespaces = new List<string>();
            var validationErrors = new List<string>();

            try
            {
                var xmlContent = await reader.ReadToEndAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                var rootElement = xmlDoc.DocumentElement;
                if (rootElement == null)
                {
                    return CreateEmptyXmlResult("Documento XML inválido ou vazio");
                }

                // Detectar namespaces
                DetectNamespaces(rootElement, namespaces);

                // Analisar a estrutura do XML para identificar registros repetidos
                var recordNodes = FindRecordNodes(rootElement);
                
                if (!recordNodes.Any())
                {
                    return CreateEmptyXmlResult("Nenhum registro encontrado no XML");
                }

                // Mapear elementos baseado no primeiro registro
                elements = MapElementsFromFirstRecord(recordNodes.First());

                // Processar todos os registros
                for (int i = 0; i < recordNodes.Count; i++)
                {
                    var recordNode = recordNodes[i];
                    var record = ProcessXmlRecord(recordNode, elements, i);
                    records.Add(record);
                }

                var elementMapping = CreateElementMapping(elements);

                return new XmlMappedData
                {
                    Elements = elements,
                    Records = records,
                    ElementMapping = elementMapping,
                    Metadata = new XmlMetadata
                    {
                        TotalRecords = records.Count,
                        TotalElements = elements.Count,
                        ProcessedAt = DateTime.UtcNow,
                        ValidationErrors = validationErrors,
                        RootElement = rootElement.Name,
                        Namespaces = namespaces
                    }
                };
            }
            catch (XmlException xmlEx)
            {
                validationErrors.Add($"Erro de XML: {xmlEx.Message}");
                return CreateEmptyXmlResult($"Erro ao processar XML: {xmlEx.Message}");
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Erro geral: {ex.Message}");
                return CreateEmptyXmlResult($"Erro inesperado: {ex.Message}");
            }
        }

        private void DetectNamespaces(XmlNode node, List<string> namespaces)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.StartsWith("xmlns"))
                    {
                        namespaces.Add($"{attr.Name}: {attr.Value}");
                    }
                }
            }
        }

        private List<XmlNode> FindRecordNodes(XmlElement rootElement)
        {
            var recordNodes = new List<XmlNode>();

            // Estratégia 1: Procurar por elementos filhos diretos que se repetem
            var childGroups = rootElement.ChildNodes.Cast<XmlNode>()
                .Where(n => n.NodeType == XmlNodeType.Element)
                .GroupBy(n => n.Name)
                .Where(g => g.Count() > 1)
                .ToList();

            if (childGroups.Any())
            {
                // Usar o grupo com mais elementos
                var mainGroup = childGroups.OrderByDescending(g => g.Count()).First();
                recordNodes.AddRange(mainGroup);
            }
            else
            {
                // Estratégia 2: Se não há elementos repetidos, cada filho direto é um registro
                recordNodes.AddRange(rootElement.ChildNodes.Cast<XmlNode>()
                    .Where(n => n.NodeType == XmlNodeType.Element));
            }

            return recordNodes;
        }

        private List<XmlElementInfo> MapElementsFromFirstRecord(XmlNode recordNode)
        {
            var elements = new List<XmlElementInfo>();
            var index = 0;

            foreach (XmlNode childNode in recordNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    var elementName = childNode.Name;
                    var normalizedName = NormalizeElementName(elementName);
                    var xpath = GetXPath(childNode);
                    
                    elements.Add(new XmlElementInfo
                    {
                        Index = index++,
                        Name = elementName,
                        NormalizedName = normalizedName,
                        InferredType = typeof(string), // Será inferido durante o processamento
                        XPath = xpath
                    });
                }
            }

            return elements;
        }

        private XmlRecord ProcessXmlRecord(XmlNode recordNode, List<XmlElementInfo> elements, int recordIndex)
        {
            var data = new Dictionary<string, object>();
            var rawValues = new List<string>();

            foreach (var element in elements)
            {
                var childNode = recordNode.SelectSingleNode(element.Name);
                var value = childNode?.InnerText ?? "";
                
                rawValues.Add(value);
                var convertedValue = ConvertValue(value, element);
                data[element.NormalizedName] = convertedValue;

                // Inferir tipo se ainda for string
                if (element.InferredType == typeof(string) && !string.IsNullOrEmpty(value))
                {
                    element.InferredType = InferType(value);
                }
            }

            return new XmlRecord
            {
                Index = recordIndex,
                Data = data,
                RawValues = rawValues,
                XmlPath = GetXPath(recordNode)
            };
        }

        private Dictionary<string, int> CreateElementMapping(List<XmlElementInfo> elements)
        {
            return elements.ToDictionary(e => e.NormalizedName, e => e.Index);
        }

        private Type InferType(string value)
        {
            if (int.TryParse(value, out _))
                return typeof(int);

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return typeof(double);

            if (DateTime.TryParse(value, out _))
                return typeof(DateTime);

            if (bool.TryParse(value, out _))
                return typeof(bool);

            return typeof(string);
        }

        private object ConvertValue(string value, XmlElementInfo element)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                return element.InferredType.Name switch
                {
                    "Int32" => int.Parse(value),
                    "Double" => double.Parse(value, CultureInfo.InvariantCulture),
                    "DateTime" => DateTime.Parse(value),
                    "Boolean" => bool.Parse(value),
                    _ => value
                };
            }
            catch
            {
                return value; // Retorna string se conversão falhar
            }
        }

        private string NormalizeElementName(string elementName)
        {
            return elementName
                .Replace(" ", "_")
                .Replace("-", "_")
                .ToLowerInvariant();
        }

        private string GetXPath(XmlNode node)
        {
            var path = new StringBuilder();
            
            while (node != null && node.NodeType == XmlNodeType.Element)
            {
                var index = 1;
                var sibling = node.PreviousSibling;
                
                while (sibling != null)
                {
                    if (sibling.NodeType == XmlNodeType.Element && sibling.Name == node.Name)
                        index++;
                    sibling = sibling.PreviousSibling;
                }

                path.Insert(0, $"/{node.Name}[{index}]");
                node = node.ParentNode;
            }

            return path.ToString();
        }

        private XmlMappedData CreateEmptyXmlResult(string error)
        {
            return new XmlMappedData
            {
                Elements = new List<XmlElementInfo>(),
                Records = new List<XmlRecord>(),
                ElementMapping = new Dictionary<string, int>(),
                Metadata = new XmlMetadata
                {
                    ValidationErrors = new List<string> { error },
                    ProcessedAt = DateTime.UtcNow
                }
            };
        }
    }
}