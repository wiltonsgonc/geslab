# LabScheduler

Sistema de gestão de laboratório com controle de equipamentos, insumos, estoque e agendamento para uso de equipamentos.

## Stack

| Camada | Tecnologia |
|---|---|
| Backend | C# 10.0 (.NET 10), ASP.NET Core Web API |
| Frontend | Blazor (Interactive Server + WebAssembly) |
| Banco | SQL Server 2022 |
| ORM | Entity Framework Core 10 |
| Container | Podman / Docker |
| Auth (dev) | Mock JWT |

## Estrutura

```
src/
├── LabScheduler.Domain/          # Entidades, enums, DTOs
├── LabScheduler.Infrastructure/  # DbContext, Services (regras de negócio)
├── LabScheduler.Api/             # API REST (controllers + mock auth)
└── LabScheduler.Web/             # Blazor (Server + Client)
    ├── LabScheduler.Web/         #   Servidor (páginas, componentes, serviços)
    └── LabScheduler.Web.Client/  #   WASM (componentes interativos)
```

## Funcionalidades

### Público (sem autenticação)
- **Calendário** — navegação mensal com visualização de reservas por dia
- **Nova Reserva** — formulário com:
  - Dados do solicitante (nome, e-mail, telefone, instituição)
  - Seleção de data e horário (07h às 18h)
  - Múltiplos equipamentos
  - Insumos opcionais (reagentes, solventes, etc.)
  - Validação de conflito de horário

### Administrativo (autenticado)
- **Dashboard** — indicadores (reservas hoje/semana, equipamentos, insumos em alerta)
- **Equipamentos** — CRUD com status (disponível, manutenção, inativo)
- **Insumos** — CRUD com categoria, CAS number, controle de estoque mínimo
- **Movimentos** — entrada/saída/ajuste/perda/devolução com saldo contábil
- **Alertas** — notificações de estoque baixo com resolução e ação tomada
- **Reservas** — gerenciamento (iniciar, concluir, cancelar)

## Credenciais de Desenvolvimento

| Usuário | Senha | Papel |
|---|---|---|
| `admin` | `lab123` | Administrador |
| `lab` | `lab123` | Técnico |
| `viewer` | `lab123` | Visualizador |

Rota: `http://localhost:8002/login`

## Como Executar

### Com Podman

```bash
# Clonar e acessar
git clone <repo> lab
cd lab

# (opcional) Ajustar senhas no .env
# SA_PASSWORD=LabScheduler@2024

# Build e iniciar
podman compose up -d --build
```

Serviços:
| Serviço | Porta |
|---|---|
| SQL Server | `1433` |
| API | `5000` |
| Web | `8002` |

### Local (sem container)

Pré-requisitos: .NET 10 SDK, SQL Server 2022.

```bash
# Ajustar connection string em src/LabScheduler.Api/appsettings.json
# "Server=localhost,1433;Database=LabScheduler;User Id=sa;Password=...;TrustServerCertificate=True"

# API
cd src/LabScheduler.Api
dotnet run

# Web (outro terminal)
cd src/LabScheduler.Web/LabScheduler.Web
dotnet run
```

## API Endpoints

### Equipamentos
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/equipamentos` | Listar todos |
| GET | `/api/equipamentos/{id}` | Obter por ID |
| POST | `/api/equipamentos` | Criar |
| PUT | `/api/equipamentos/{id}` | Atualizar |
| DELETE | `/api/equipamentos/{id}` | Excluir (soft delete) |

### Insumos
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/insumos` | Listar todos |
| GET | `/api/insumos/estoque-baixo` | Filtrar estoque baixo |
| GET | `/api/insumos/{id}` | Obter por ID |
| POST | `/api/insumos` | Criar |
| PUT | `/api/insumos/{id}` | Atualizar |
| DELETE | `/api/insumos/{id}` | Excluir |

### Movimentos de Estoque
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/movimentos-estoque?insumoId=` | Listar (filtro opcional) |
| POST | `/api/movimentos-estoque` | Registrar entrada/saída |

### Reservas
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/reservas?data=` | Listar (filtro opcional) |
| GET | `/api/reservas/calendario?inicio=&fim=` | Eventos do calendário |
| GET | `/api/reservas/horarios-disponiveis?data=` | Slots livres |
| GET | `/api/reservas/dashboard` | Indicadores do admin |
| POST | `/api/reservas` | Criar reserva |
| PATCH | `/api/reservas/{id}/status` | Alterar status |
| DELETE | `/api/reservas/{id}` | Excluir |

### Alertas
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/alertas?status=` | Listar alertas |
| GET | `/api/alertas/ativos/count` | Total de ativos |
| POST | `/api/alertas/{id}/resolver` | Resolver alerta |

### Autenticação (Mock Dev)
| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/login` | Login (body: username, password) |
| GET | `/api/auth/me` | Dados do usuário logado |
