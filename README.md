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

### Docker (Ubuntu / Docker Desktop)

```bash
# Build e iniciar
docker compose up -d --build
```

### Podman com systemd ativo (Debian/Ubuntu com systemd --user funcional)

```bash
podman compose up -d --build
```

### Podman sem systemd --user (Alma Linux no WSL2 / VPS sem PAM completo)

**Passo 1 — Configurar o host** (executar UMA ÚNICA VEZ):

```bash
chmod +x scripts/setup-podman-rootless.sh
./scripts/setup-podman-rootless.sh
```

O script desabilita aardvark-dns e pasta, que exigem D-Bus/systemd.

**Passo 2 — Abrir NOVA sessão SSH** (para carregar as variáveis de ambiente):

```bash
source ~/.bashrc
podman run --rm alpine echo ok   # teste
```

**Passo 3 — Iniciar o projeto**:

```bash
podman-compose up -d
```

Se o Passo 3 ainda falhar com erro de rede, use o **fallback com host networking**:

```bash
podman-compose -f docker-compose.yml -f docker-compose.podman.yml --env-file .env.podman up -d
```

Serviços (qualquer modo):
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

## Troubleshooting

### "aardvark-dns failed to start" + "pasta process" (Podman 5.x rootless)

**Causa**: Podman 5.x usa `netavark`/`aardvark-dns` para DNS e `pasta` para rede
rootless. Ambos exigem systemd --user (socket D-Bus em `/run/user/1000/bus`).
No WSL2 ou VPS sem sessão `systemd --user`, esses sockets não existem.

**Solução primária** — configurar o host (recomendado):

```bash
./scripts/setup-podman-rootless.sh
# Abrir NOVA sessão
podman-compose up -d
```

O script cria `~/.config/containers/containers.conf` com:
- `dns_bind_port = 0` → desliga aardvark-dns (SEM isso o container não sobe)
- `default_rootless_network_cmd = "slirp4netns"` → evita pasta
- `cgroup_manager = "cgroupfs"` → evita cgroups via systemd

**Solução fallback** — host networking (se o script não resolver):

```bash
podman-compose -f docker-compose.yml -f docker-compose.podman.yml --env-file .env.podman up -d
```

O `docker-compose.podman.yml` usa `network_mode: "host"`, que pula todo o
stack netavark/aardvark/pasta. Os contêineres compartilham a rede do host.

### "NU1301: Unable to load the service index for source https://api.nuget.org/v3/index.json"

O `dotnet restore` falha durante o build por falta de DNS no contêiner build.
O `build.network: host` no compose resolve isso. Se ainda falhar, execute o
`setup-podman-rootless.sh` ou use o fallback `.podman.yml`.

### "HEALTHCHECK is not supported for OCI image format"

Apenas aviso — Podman usa formato OCI por padrão. O container funciona.

### SQL Server não fica pronto (healthcheck timeout)

SQL Server 2022 precisa de no mínimo 2 GB de RAM. Em WSL ou VPS com pouca memória:

```bash
# Aumentar memória do WSL2: crie %USERPROFILE%\.wslconfig
[wsl2]
memory=8GB
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
