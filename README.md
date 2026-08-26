
# WeatherForecastAPI

![Badge](https://img.shields.io/badge/Status-Conclu%C3%ADdo-green)
![GitHub issues](https://img.shields.io/github/issues/MatheusAV/WeatherForecastApp)
![GitHub stars](https://img.shields.io/github/stars/MatheusAV/WeatherForecastApp)
![GitHub forks](https://img.shields.io/github/forks/MatheusAV/WeatherForecastApp)

## 📝 Descrição

A **WeatherForecastAPI** é uma API RESTful desenvolvida em .NET 8 com arquitetura em camadas (DDD simplificado), que consome a API externa **OpenWeatherMap** para fornecer:  

- Previsão do tempo atual  
- Previsão para os próximos 5 dias  
- Histórico de buscas  
- Cache com SQL Server para otimização de chamadas externas  

Conta com tratamento global de erros, logs estruturados com **Serilog**, documentação Swagger e serviço em background para limpeza periódica do cache.

---

## 🚀 Funcionalidades

- 🔥 **Previsão do Tempo Atual**: Consulta a temperatura, umidade, descrição e velocidade do vento para uma cidade.
- 📆 **Previsão Estendida**: Fornece previsão diária para os próximos 5 dias.
- 🕒 **Histórico de Buscas**: Lista as últimas cidades pesquisadas pelo usuário.
- 💾 **Cache Persistente**: Armazena previsões em banco SQL Server para evitar chamadas desnecessárias à API externa.
- ♻️ **Serviço em Background**: Limpa entradas expiradas do cache automaticamente a cada 1 hora.
- 📖 **Documentação Swagger**: Interface interativa para explorar os endpoints.

---

## 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
- Chave de API do [OpenWeatherMap](https://openweathermap.org/api)

---

## 🛠️ Configuração do Projeto Backend

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/MatheusAV/WeatherForecastApp.git
   cd WeatherForecastAPI
   ```

2. **Restaure as dependências do projeto:**
   ```bash
   dotnet restore
   ```

3. **Configure o arquivo `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=SEU_SERVIDOR;Database=WeatherForecastDb;Trusted_Connection=True;"
   },
   "WeatherApi": {
     "BaseUrl": "https://api.openweathermap.org/data/2.5/",
     "ApiKey": "SUA_API_KEY_AQUI"
   }
   ```

4. **Execute as migrations para criar o banco:**
   ```bash
   dotnet ef database update --project WeatherForecast.Infrastructure
   ```

5. **Execute a aplicação:**
   ```bash
   dotnet run --project WeatherForecast.Api
   ```

6. **Acesse a documentação Swagger:**
   [http://localhost:5000/swagger](http://localhost:5000/swagger)  

---

## 📂 Estrutura do Projeto

```plaintext
WeatherForecastAPI/
├── WeatherForecast.Api/             # Camada de interface (Web API)
│   ├── Controllers/                 # Endpoints REST
│   ├── Program.cs                   # Configuração principal da API
│   ├── appsettings.json             # Configurações da aplicação
├── WeatherForecast.Application/     # Camada de aplicação (Serviços e DTOs)
│   ├── Services/                    # Serviços de aplicação
│   ├── Interfaces/                  # Contratos dos serviços
│   ├── DTOs/                        # Data Transfer Objects
├── WeatherForecast.Domain/          # Camada de domínio (Entidades e Regras)
│   ├── Entities/                    # Entidades principais
│   ├── Interfaces/                  # Interfaces para infraestrutura
├── WeatherForecast.Infrastructure/  # Camada de infraestrutura (Banco e API externa)
│   ├── Data/                        # DbContext e Configurações EF Core
│   ├── ExternalServices/            # Cliente para API OpenWeatherMap
│   ├── BackgroundServices/          # Serviço de limpeza de cache
├── WeatherForecast.Tests/           # Testes automatizados (unitários e integração)
│   ├── Application/                 # Testes de serviços de aplicação
│   ├── Infrastructure/              # Testes de background services
│   ├── Integration/                 # Testes de integração com WebApplicationFactory
└── README.md                        # Documentação do projeto
```

---

## 🧬 Endpoints

### 🌤️ 1. Previsão Atual
- **Endpoint:** `GET /api/weather/current?city={cidade}`
- **Descrição:** Retorna a previsão do tempo atual para a cidade informada.
- **Exemplo de resposta:**
  ```json
  {
    "city": "São Paulo",
    "temperature": 25.3,
    "humidity": 78,
    "description": "nublado",
    "windSpeed": 3.6,
    "retrievedFromCache": false
  }
  ```

---

### 📅 2. Previsão 5 Dias
- **Endpoint:** `GET /api/weather/forecast?city={cidade}`
- **Descrição:** Fornece a previsão diária para os próximos 5 dias.
- **Exemplo de resposta:**
  ```json
  {
    "city": "São Paulo",
    "dailyForecasts": [
      {
        "date": "2025-07-22",
        "temperatureMin": 18.2,
        "temperatureMax": 27.5,
        "description": "parcialmente nublado"
      },
      ...
    ],
    "retrievedFromCache": true
  }
  ```

---

### 🕓 3. Histórico de Buscas
- **Endpoint:** `GET /api/history?limit=5`
- **Descrição:** Retorna as últimas cidades pesquisadas.
- **Exemplo de resposta:**
  ```json
  [
    { "city": "São Paulo", "lastSearchedAt": "2025-07-21T16:15:30Z" },
    { "city": "Rio de Janeiro", "lastSearchedAt": "2025-07-21T15:50:12Z" }
  ]
  ```

---

## 🧪 Testes

O projeto inclui **testes unitários** e **testes de integração**:

✅ Unitários para Services e Background Services  
✅ Integração com WebApplicationFactory (testando endpoints reais com banco InMemory)

### Executar os testes:
```bash
dotnet test
```

---

## 📝 Tecnologias Utilizadas

- .NET 8.0 - Framework principal
- SQL Server - Banco de dados relacional
- Entity Framework Core - ORM para persistência
- Swagger - Documentação interativa da API
- Serilog - Logging estruturado
- xUnit/Moq - Testes unitários
- Microsoft.AspNetCore.Mvc.Testing - Testes de integração
