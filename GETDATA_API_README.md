# API GetData - Endpoints de Consulta

Este documento descreve os endpoints disponíveis no controller GetData para consultar dados do sistema.

## Base URL
```
/v1/GetData
```

## Endpoints Disponíveis

### 1. Health Check
**GET** `/v1/GetData/health`

Verifica se a API está funcionando.

**Resposta:**
```json
{
  "status": "API GetData está funcionando",
  "timestamp": "2025-09-21T10:30:00.000Z"
}
```

### 2. Estados
**GET** `/v1/GetData/estados`

Retorna todos os estados cadastrados.

**Resposta:**
```json
[
  {
    "id": "uuid",
    "codigo_uf": 11,
    "uf": "RO",
    "nome": "Rondônia",
    "latitude": -10.83,
    "longitude": -63.34,
    "regiao": "Norte"
  }
]
```

### 3. Municípios
**GET** `/v1/GetData/municipios`

Retorna todos os municípios cadastrados.

**Resposta:**
```json
[
  {
    "id": "uuid",
    "codigo_ibge": "1100015",
    "nome": "Alta Floresta D'Oeste",
    "latitude": -11.93,
    "longitude": -61.99,
    "capital": false,
    "codigo_uf": "11",
    "siafi_id": 1234,
    "ddd": 69,
    "fuso_horario": "America/Porto_Velho",
    "populacao": 25000,
    "erros": ""
  }
]
```

### 4. Municípios por Estado
**GET** `/v1/GetData/municipios/estado/{codigoUf}`

Retorna todos os municípios de um estado específico.

**Parâmetros:**
- `codigoUf` (string): Código UF do estado (ex: "11", "12", "AC", "SP")

**Exemplo:**
```
GET /v1/GetData/municipios/estado/11
```

### 5. Pacientes
**GET** `/v1/GetData/pacientes`

Retorna todos os pacientes cadastrados com informações do município e CID-10.

**Resposta:**
```json
[
  {
    "id": "uuid",
    "codigo": "PAC001",
    "cpf": "123.456.789-00",
    "genero": "M",
    "nome_completo": "João da Silva",
    "convenio": true,
    "municipio": "São Paulo",
    "cid10": "A00",
    "descricaoCid10": "Cólera"
  }
]
```

### 6. Médicos
**GET** `/v1/GetData/medicos`

Retorna todos os médicos cadastrados com informações do município.

**Resposta:**
```json
[
  {
    "id": "uuid",
    "codigo": "MED001",
    "nome_completo": "Dr. Maria Santos",
    "especialidade": "Cardiologia",
    "municipio": "São Paulo"
  }
]
```

### 7. Hospitais
**GET** `/v1/GetData/hospitais`

Retorna todos os hospitais cadastrados com informações da cidade.

**Resposta:**
```json
[
  {
    "id": "uuid",
    "codigo": "HOSP001",
    "nome": "Hospital das Clínicas",
    "bairro": "Cerqueira César",
    "cidade": "São Paulo",
    "leitos_totais": 2000
  }
]
```

## Tratamento de Erros

Todos os endpoints retornam erro 500 em caso de falha interna:

```json
{
  "message": "Erro interno do servidor",
  "error": "Detalhes do erro..."
}
```

## Tecnologias Utilizadas

- **ASP.NET Core 8.0**
- **Entity Framework Core**
- **SQL Server**
- **DTOs** para transferência de dados
- **Dependency Injection** para gerenciamento de serviços

## Como Usar no Frontend

Exemplo de uso com JavaScript/TypeScript:

```javascript
// Buscar todos os estados
const estados = await fetch('/v1/GetData/estados')
  .then(response => response.json());

// Buscar municípios de um estado específico
const municipios = await fetch('/v1/GetData/municipios/estado/SP')
  .then(response => response.json());

// Buscar todos os pacientes
const pacientes = await fetch('/v1/GetData/pacientes')
  .then(response => response.json());
```