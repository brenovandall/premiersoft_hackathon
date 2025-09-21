using Hackathon.Premiersoft.API.Engines.Parsers;
using Hackathon.Premiersoft.API.Utils;

// Console app simples para testar o parser CID-10
Console.WriteLine("=== Teste do Parser CID-10 ===\n");

try
{
    var cid10List = await Cid10TestHelper.TestCid10Parser();
    
    Console.WriteLine($"Códigos CID-10 encontrados: {cid10List.Count}\n");
    
    foreach (var cid10 in cid10List.Take(10)) // Mostrar apenas os primeiros 10
    {
        Console.WriteLine($"Código: {cid10.Codigo} | Descrição: {cid10.Descricao}");
    }
    
    if (cid10List.Count > 10)
    {
        Console.WriteLine($"... e mais {cid10List.Count - 10} códigos.");
    }
    
    // Validar os códigos
    var validation = Cid10TestHelper.ValidateCid10List(cid10List);
    Console.WriteLine($"\n=== Resultado da Validação ===");
    Console.WriteLine(validation.GetSummary());
    
    if (validation.Errors.Any())
    {
        Console.WriteLine("\nErros encontrados:");
        foreach (var error in validation.Errors.Take(5))
        {
            Console.WriteLine($"- {error}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();