# 🏖️ Beach Tennis Arena

> Sistema web para gerenciamento de uma arena de Beach Tennis, desenvolvido com **Angular**, **ASP.NET Core**, **Entity Framework Core** e banco de dados relacional.

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-512BD4?logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-Frontend-DD0031?logo=angular&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Backend-239120?logo=csharp&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue)

---

## 📋 Sumário

- [Sobre o Projeto](#-sobre-o-projeto)
- [Objetivos](#-objetivos)
- [Perfis de Usuário](#-perfis-de-usuário)
- [Escopo do MVP](#-escopo-do-mvp)
- [Fluxo Principal](#-fluxo-principal)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Requisitos Funcionais](#-requisitos-funcionais)
- [Regras de Negócio](#-regras-de-negócio)
- [Modelo de Dados](#-modelo-de-dados)
- [Enums](#-enums)
- [Estrutura do Backend](#-estrutura-do-backend)
- [Endpoints](#-endpoints)
- [Sprints](#-sprints)
- [Backlog](#-backlog)
- [Segurança](#-segurança)
- [Testes](#-testes)
- [Documentação da API](#-documentação-da-api)
- [Evoluções Futuras](#-evoluções-futuras)
- [Conclusão](#-conclusão)

---

## 🎯 Sobre o Projeto

O **Beach Tennis Arena** é uma aplicação web para gerenciamento de uma arena esportiva de Beach Tennis.

O sistema tem como objetivo centralizar as operações relacionadas à utilização das quadras, permitindo que clientes consultem a disponibilidade, realizem reservas e acompanhem seus pagamentos.

Ao mesmo tempo, os administradores terão acesso a uma área administrativa para gerenciamento de clientes, quadras, reservas e pagamentos.

O projeto será desenvolvido inicialmente como um **MVP — Minimum Viable Product**, contemplando as funcionalidades essenciais para o funcionamento da arena.

A arquitetura será estruturada de forma que novas funcionalidades possam ser adicionadas futuramente sem a necessidade de grandes alterações na aplicação.

---

## 🎯 Objetivos

O principal objetivo do sistema é centralizar o gerenciamento da arena em uma única aplicação.

### Para os clientes

- Realizar cadastro;
- Efetuar login;
- Consultar quadras;
- Consultar horários disponíveis;
- Realizar reservas;
- Consultar suas reservas;
- Consultar pagamentos;
- Cancelar reservas conforme as regras do sistema.

### Para os administradores

- Gerenciar clientes;
- Gerenciar quadras;
- Ativar e inativar quadras;
- Consultar todas as reservas;
- Acompanhar reservas por data, horário e quadra;
- Alterar ou cancelar reservas;
- Consultar pagamentos;
- Confirmar pagamentos;
- Visualizar indicadores da operação.

---

# 👥 Perfis de Usuário

O sistema possuirá inicialmente dois perfis.

## 👤 Cliente

O cliente será o usuário que utiliza a arena.

### Permissões

- Criar cadastro;
- Realizar login;
- Atualizar seus dados;
- Consultar quadras;
- Consultar disponibilidade;
- Criar reservas;
- Consultar suas reservas;
- Consultar pagamentos;
- Cancelar reservas.

---

## 🛡️ Administrador

O administrador será responsável pelo gerenciamento da arena.

### Permissões

- Gerenciar clientes;
- Gerenciar quadras;
- Ativar quadras;
- Inativar quadras;
- Consultar reservas;
- Alterar reservas;
- Cancelar reservas;
- Consultar pagamentos;
- Confirmar pagamentos;
- Visualizar dashboard administrativo.

---

# 📦 Escopo do MVP

A primeira versão do sistema contemplará:

- [x] Cadastro de clientes
- [x] Gerenciamento de clientes
- [x] Cadastro de quadras
- [x] Gerenciamento de quadras
- [x] Consulta de quadras
- [x] Consulta de disponibilidade
- [x] Criação de reservas
- [x] Consulta de reservas
- [x] Cancelamento de reservas
- [x] Cálculo do valor da reserva
- [x] Registro de pagamentos
- [x] Acompanhamento de pagamentos
- [x] Autenticação
- [x] Autorização por perfil
- [x] Área administrativa

### Fora do MVP

As seguintes funcionalidades não fazem parte da primeira versão:

- Torneios;
- Aulas;
- Mensalidades;
- Planos de assinatura;
- Integração real com gateways de pagamento;
- Programa de fidelidade;
- Ranking de jogadores;
- Notificações avançadas.

Essas funcionalidades poderão ser implementadas posteriormente.

---

# 🔄 Fluxo Principal

O principal fluxo da aplicação será a reserva de uma quadra.

```text
                         ┌──────────────┐
                         │    CLIENTE   │
                         └──────┬───────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │ Acessa sistema  │
                       └────────┬────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │     Login       │
                       └────────┬────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │ Consulta quadras│
                       └────────┬────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │ Seleciona data  │
                       │ e horário       │
                       └────────┬────────┘
                                │
                                ▼
                    ┌────────────────────────┐
                    │ Verifica disponibilidade│
                    └───────────┬────────────┘
                                │
                       ┌────────┴────────┐
                       │                 │
                      NÃO               SIM
                       │                 │
                       ▼                 ▼
               ┌─────────────┐   ┌──────────────┐
               │ Indisponível│   │ Calcula valor│
               └─────────────┘   └──────┬───────┘
                                        │
                                        ▼
                               ┌─────────────────┐
                               │ Confirma reserva│
                               └────────┬────────┘
                                        │
                                        ▼
                               ┌─────────────────┐
                               │ Cria reserva   │
                               └────────┬────────┘
                                        │
                                        ▼
                               ┌─────────────────┐
                               │ Registra       │
                               │ pagamento      │
                               └────────┬────────┘
                                        │
                                        ▼
                               ┌─────────────────┐
                               │ Reserva        │
                               │ disponível     │
                               │ para consulta  │
                               └─────────────────┘

🏢 Fluxo Administrativo
                    ┌──────────────────┐
                    │  ADMINISTRADOR   │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │      Login       │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │    Dashboard     │
                    └────────┬─────────┘
                             │
             ┌───────────────┼───────────────┐
             │               │               │
             ▼               ▼               ▼
        ┌──────────┐    ┌──────────┐    ┌──────────┐
        │ Clientes │    │  Quadras │    │ Reservas │
        └──────────┘    └──────────┘    └──────────┘
                                             │
                                             ▼
                                      ┌──────────────┐
                                      │ Pagamentos   │
                                      └──────────────┘

🏗️ Arquitetura
A aplicação será dividida em duas camadas principais:

Frontend: Angular

Backend: C# / .NET / ASP.NET Core Web API

A comunicação será realizada através de uma API REST, utilizando HTTP e JSON.

┌───────────────────────────────────────┐
│                USUÁRIO                │
└────────────────────┬──────────────────┘
                     │
                     ▼
┌───────────────────────────────────────┐
│            FRONTEND ANGULAR           │
│                                       │
│  • Telas                              │
│  • Formulários                        │
│  • Navegação                          │
│  • Validações de interface            │
│  • Gerenciamento de sessão            │
│  • Requisições HTTP                   │
└────────────────────┬──────────────────┘
                     │
                 HTTP / JSON
                     │
                     ▼
┌───────────────────────────────────────┐
│          ASP.NET CORE WEB API         │
│                                       │
│  • Controllers                        │
│  • DTOs                               │
│  • Services                           │
│  • Regras de negócio                  │
│  • Autenticação                       │
│  • Autorização                        │
│  • Validações                         │
└────────────────────┬──────────────────┘
                     │
                     ▼
┌───────────────────────────────────────┐
│         ENTITY FRAMEWORK CORE         │
└────────────────────┬──────────────────┘
                     │
                     ▼
┌───────────────────────────────────────┐
│             BANCO DE DADOS            │
└───────────────────────────────────────┘

🖥️ Frontend — Angular
O Frontend será responsável pela apresentação e interação com o usuário.

Responsabilidades
Criar as telas;

Criar formulários;

Realizar validações de interface;

Controlar navegação;

Implementar calendário;

Selecionar horários;

Exibir quadras;

Exibir disponibilidade;

Exibir reservas;

Exibir pagamentos;

Exibir mensagens de erro;

Exibir mensagens de sucesso;

Controlar visualmente funcionalidades por perfil;

Realizar chamadas HTTP;

Gerenciar sessão do usuário.

Validações
Exemplos:

Campos obrigatórios;

Formato de e-mail;

Formato de CPF;

Formato de telefone;

Senhas;

Formulários incompletos;

Seleção de horários.

As validações realizadas no Frontend são voltadas principalmente à experiência do usuário. As regras críticas deverão sempre ser validadas novamente no Backend.

⚙️ Backend — ASP.NET Core
O Backend será responsável por toda a lógica de negócio, segurança e persistência.

Responsabilidades
Disponibilizar API REST;

Receber requisições;

Validar dados;

Implementar regras de negócio;

Gerenciar clientes;

Gerenciar quadras;

Gerenciar reservas;

Gerenciar pagamentos;

Controlar autenticação;

Controlar autorização;

Verificar disponibilidade;

Impedir conflitos de horários;

Calcular valores;

Controlar status das reservas;

Controlar status dos pagamentos;

Persistir dados;

Utilizar Entity Framework Core;

Garantir integridade dos relacionamentos;

Tratar erros;

Retornar códigos HTTP adequados;

Registrar logs.

🔐 Separação de Responsabilidades
O Frontend nunca deverá acessar diretamente o banco de dados.

Todo acesso deverá seguir o fluxo:

Angular
   │
   ▼
REST API
   │
   ▼
Services
   │
   ▼
Entity Framework Core
   │
   ▼
Banco de Dados

Além disso, regras críticas deverão ser verificadas no Backend.

Exemplo
O Angular pode informar:

Quadra 01
19:00
Disponível

Porém, no momento da criação da reserva, o Backend deverá consultar novamente o banco.

Isso evita situações de concorrência como:

Usuário A → Quadra 01 → 19:00
Usuário B → Quadra 01 → 19:00

Somente uma das reservas poderá ser criada.

📋 Requisitos Funcionais
Clientes
RF01 — Cadastrar cliente
O sistema deverá permitir o cadastro contendo:

Nome;

CPF;

Telefone;

E-mail;

Data de nascimento;

Senha;

Status.

RF02 — Alterar cliente
O cliente ou administrador poderá alterar:

Nome;

CPF;

Telefone;

E-mail;

Data de nascimento;

Senha;

Status.

RF03 — Consultar cliente
O administrador poderá consultar clientes cadastrados.

RF04 — Inativar cliente
O sistema deverá permitir a inativação sem necessariamente excluir o histórico.

Quadras
RF05 — Cadastrar quadra
O administrador poderá cadastrar:

Nome;

Tipo;

Valor por hora;

Status.

Exemplo:

Quadra 01
Tipo: Beach Tennis
Valor: R$ 80,00/h
Status: Ativa

RF06 — Alterar quadra
O administrador poderá alterar:

Nome;

Tipo;

Valor por hora;

Status.

RF07 — Inativar quadra
Uma quadra poderá ser temporariamente desativada.

Exemplo:

Quadra 02
Status: Manutenção

RF08 — Consultar quadras
O sistema deverá listar as quadras disponíveis.

📅 Reservas
RF09 — Criar reserva
O cliente deverá selecionar:

Quadra;

Data;

Horário inicial;

Duração.

O sistema deverá verificar a disponibilidade antes de criar a reserva.

RF10 — Verificar disponibilidade
O sistema não poderá permitir reservas conflitantes.

Exemplo:

Quadra 01
19:00 ───────── 20:00
       RESERVADA

Não será permitida uma segunda reserva:

Quadra 01
19:30 ───────── 20:30
       ❌ CONFLITO

RF11 — Consultar reservas
O cliente poderá visualizar suas próprias reservas.

O administrador poderá visualizar todas as reservas.

RF12 — Cancelar reserva
O cliente poderá cancelar uma reserva respeitando a regra de cancelamento definida pelo sistema.

RF13 — Alterar reserva
O cliente poderá alterar data ou horário desde que exista disponibilidade.

💰 Pagamentos
Inicialmente, os pagamentos serão simulados.

Não será necessária integração com gateway de pagamento no MVP.

RF14 — Registrar pagamento
Um pagamento deverá possuir:

Valor;

Data;

Método;

Status;

Reserva relacionada.

Métodos
PIX;

Cartão;

Dinheiro.

Status
Pendente;

Pago;

Cancelado;

Estornado.

RF15 — Consultar pagamento
O cliente poderá consultar o status do pagamento de suas reservas.

RF16 — Confirmar pagamento
O administrador poderá registrar ou confirmar um pagamento.

📏 Regras de Negócio
Código	Regra
RN01	Não poderá existir mais de um cliente ativo com o mesmo CPF.
RN02	O e-mail deverá ser único.
RN03	Quadras inativas não poderão receber novas reservas.
RN04	Não serão permitidas reservas com períodos sobrepostos na mesma quadra.
RN05	Toda reserva deverá possuir um cliente.
RN06	Toda reserva deverá possuir uma quadra.
RN07	O valor deverá ser calculado com base no valor/hora e duração.
RN08	Reservas poderão ser canceladas conforme as regras estabelecidas.
RN09	Reservas canceladas não deverão continuar ocupando o horário.
RN10	Uma reserva poderá possuir um pagamento correspondente.

Exemplo de cálculo
Valor da quadra: R$ 80,00/h

Duração: 2 horas

80 × 2 = R$ 160,00

Total da reserva: R$ 160,00

🗃️ Modelo de Dados
A primeira versão terá quatro entidades principais:

                 ┌──────────────┐
                 │   CLIENTE    │
                 └───────┬──────┘
                         │
                         │ 1:N
                         │
                         ▼
                 ┌──────────────┐
                 │   RESERVA    │
                 └───────┬──────┘
                         │
              ┌──────────┴──────────┐
              │                     │
              │ N:1                 │ 1:1
              ▼                     ▼
       ┌──────────────┐     ┌──────────────┐
       │    QUADRA    │     │  PAGAMENTO   │
       └──────────────┘     └──────────────┘

👤 Cliente
Cliente
├── Id
├── Nome
├── CPF
├── Email
├── Telefone
├── DataNascimento
├── Senha
└── Status

🏖️ Quadra
Quadra
├── Id
├── Nome
├── Tipo
├── ValorHora
└── Status

📅 Reserva
Reserva
├── Id
├── ClienteId
├── QuadraId
├── Data
├── HoraInicio
├── HoraFim
├── Valor
├── Status
└── DataCriacao

💳 Pagamento
Pagamento
├── Id
├── ReservaId
├── Valor
├── Metodo
├── Status
└── DataPagamento

🔢 Enums
Status do Cliente
public enum StatusCliente
{
    Ativo = 1,
    Inativo = 2
}

Status da Quadra
public enum StatusQuadra
{
    Ativa = 1,
    Inativa = 2,
    Manutencao = 3
}

Status da Reserva
public enum StatusReserva
{
    Pendente = 1,
    Confirmada = 2,
    Cancelada = 3,
    Concluida = 4
}

Status do Pagamento
public enum StatusPagamento
{
    Pendente = 1,
    Pago = 2,
    Cancelado = 3,
    Estornado = 4
}

Método de Pagamento
public enum MetodoPagamento
{
    Pix = 1,
    Cartao = 2,
    Dinheiro = 3
}

🗂️ Estrutura do Backend
A estrutura inicial do Backend será organizada da seguinte maneira:

Backend/
│
├── Controllers/
│
├── DTOs/
│   ├── Clientes/
│   ├── Quadras/
│   ├── Reservas/
│   ├── Pagamentos/
│   └── Auth/
│
├── Services/
│   ├── ClienteService.cs
│   ├── QuadraService.cs
│   ├── ReservaService.cs
│   ├── PagamentoService.cs
│   └── AuthService.cs
│
├── Entities/
│   ├── Cliente.cs
│   ├── Quadra.cs
│   ├── Reserva.cs
│   └── Pagamento.cs
│
├── Enums/
│   ├── StatusCliente.cs
│   ├── StatusQuadra.cs
│   ├── StatusReserva.cs
│   ├── StatusPagamento.cs
│   └── MetodoPagamento.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Migrations/
│
├── Program.cs
└── appsettings.json

A principal finalidade dessa organização é evitar que regras de negócio sejam concentradas diretamente nos Controllers.

🌐 Endpoints
Clientes
Método	Endpoint	Descrição
POST	/api/clientes	Cadastrar cliente
GET	/api/clientes	Listar clientes
GET	/api/clientes/{id}	Consultar cliente
PUT	/api/clientes/{id}	Alterar cliente
DELETE	/api/clientes/{id}	Inativar cliente

Quadras
Método	Endpoint	Descrição
POST	/api/quadras	Cadastrar quadra
GET	/api/quadras	Listar quadras
GET	/api/quadras/{id}	Consultar quadra
PUT	/api/quadras/{id}	Alterar quadra
DELETE	/api/quadras/{id}	Inativar quadra
GET	/api/quadras/{id}/disponibilidade	Consultar disponibilidade

Reservas
Método	Endpoint	Descrição
POST	/api/reservas	Criar reserva
GET	/api/reservas	Listar reservas
GET	/api/reservas/{id}	Consultar reserva
PUT	/api/reservas/{id}	Alterar reserva
POST	/api/reservas/{id}/cancelar	Cancelar reserva

Pagamentos
Método	Endpoint	Descrição
POST	/api/pagamentos	Registrar pagamento
GET	/api/pagamentos	Listar pagamentos
GET	/api/pagamentos/{id}	Consultar pagamento
POST	/api/pagamentos/{id}/confirmar	Confirmar pagamento

Autenticação
Método	Endpoint	Descrição
POST	/api/auth/login	Realizar login
POST	/api/auth/register	Criar conta

🗓️ Planejamento das Sprints
Sprint 1 — Estrutura e Entidades
Objetivo
Criar a base do sistema e estruturar o banco de dados.

Backend
Criar projeto .NET Web API;

Configurar Entity Framework Core;

Configurar banco de dados;

Criar entidades;

Criar Enums;

Criar relacionamentos;

Criar DbContext;

Criar migrations;

Criar banco de dados.

Entidades
Cliente;

Quadra;

Reserva;

Pagamento.

Entrega
.NET Web API
      │
      ▼
Entity Framework Core
      │
      ▼
DbContext
      │
      ▼
Migrations
      │
      ▼
Banco de Dados
      │
      ▼
Tabelas e relacionamentos

Sprint 2 — CRUD Básico
Objetivo
Disponibilizar operações básicas de cadastro, consulta, alteração e inativação.

Clientes
POST   /api/clientes
GET    /api/clientes
GET    /api/clientes/{id}
PUT    /api/clientes/{id}
DELETE /api/clientes/{id}

Quadras
POST   /api/quadras
GET    /api/quadras
GET    /api/quadras/{id}
PUT    /api/quadras/{id}
DELETE /api/quadras/{id}

Frontend
Clientes
├── Listagem
├── Cadastro
└── Edição

Quadras
├── Listagem
├── Cadastro
└── Edição

Sprint 3 — Reservas
Objetivo
Implementar o principal processo de negócio do sistema: a reserva de uma quadra.

Endpoints
POST /api/reservas
GET  /api/reservas
GET  /api/reservas/{id}
PUT  /api/reservas/{id}

POST /api/reservas/{id}/cancelar

GET /api/quadras/{id}/disponibilidade

O Backend deverá
Verificar se a quadra está ativa;

Verificar se o cliente existe;

Verificar se o horário é válido;

Verificar conflitos;

Calcular o valor;

Criar a reserva.

Tela de Reserva
┌──────────────────────────────────────────┐
│             RESERVAR QUADRA              │
├──────────────────────────────────────────┤
│                                          │
│ Data:                                    │
│ [ 20/09/2026 ]                           │
│                                          │
│ Quadra:                                  │
│ [ Quadra 01 ▼ ]                          │
│                                          │
│ Horários disponíveis:                    │
│                                          │
│ [08:00] [09:00] [10:00] [11:00]         │
│ [14:00] [15:00] [16:00] [18:00]         │
│ [20:00]                                  │
│                                          │
│ Valor: R$ 80,00                          │
│                                          │
│              [ RESERVAR ]                │
│                                          │
└──────────────────────────────────────────┘

Sprint 4 — Pagamentos
Objetivo
Implementar o controle de pagamentos relacionados às reservas.

Endpoints
POST /api/pagamentos
GET  /api/pagamentos
GET  /api/pagamentos/{id}

POST /api/pagamentos/{id}/confirmar

Tela
┌───────────────────────────────────────────────────────┐
│                  MINHAS RESERVAS                      │
├──────────┬────────────┬────────────┬──────────────────┤
│ Data     │ Quadra     │ Valor      │ Pagamento        │
├──────────┼────────────┼────────────┼──────────────────┤
│ 20/09    │ Quadra 01  │ R$ 80,00   │ 🟢 PAGO          │
│ 21/09    │ Quadra 02  │ R$ 80,00   │ 🟡 PENDENTE      │
└──────────┴────────────┴────────────┴──────────────────┘

Sprint 5 — Autenticação
Prioridade: Baixa

Objetivo
Implementar segurança, login e controle de acesso.

Implementações
Login;

Cadastro;

JWT;

Autenticação;

Autorização;

Perfil Cliente;

Perfil Administrador;

Proteção das rotas Angular;

Proteção dos endpoints;

Controle de acesso.

Endpoints
POST /api/auth/login
POST /api/auth/register

Rotas Angular
/login
/cadastro
/minhas-reservas
/reservar

/admin/clientes
/admin/quadras
/admin/reservas
/admin/pagamentos

Sprint 6 — Refinamento e Finalização
Objetivo
Realizar o refinamento geral da aplicação e preparar a entrega.

Atividades
Validações;

Tratamento de erros;

Mensagens para o usuário;

Paginação;

Filtros;

Dashboard;

Melhorias visuais;

Testes;

Documentação;

Swagger;

Deploy.

📊 Dashboard Administrativo
O Dashboard poderá apresentar informações como:

╔══════════════════════════════════════════════════════╗
║              BEACH TENNIS - DASHBOARD               ║
╠══════════════════════════════════════════════════════╣
║                                                      ║
║  CLIENTES       QUADRAS       RESERVAS HOJE         ║
║     248             6               18              ║
║                                                      ║
║  PAGAMENTOS HOJE                                      ║
║  R$ 1.240,00                                         ║
║                                                      ║
╠══════════════════════════════════════════════════════╣
║                RESERVAS RECENTES                     ║
╠════════╦════════╦════════════╦═══════════╦═══════════╣
║ Data   ║ Horário║ Quadra     ║ Cliente   ║ Status    ║
╠════════╬════════╬════════════╬═══════════╬═══════════╣
║ 20/09  ║ 18:00  ║ Quadra 01  ║ João      ║ Confirmada║
║ 20/09  ║ 19:00  ║ Quadra 03  ║ Maria     ║ Pendente  ║
║ 20/09  ║ 20:00  ║ Quadra 02  ║ Carlos    ║ Confirmada║
╚════════╩════════╩════════════╩═══════════╩═══════════╝

📌 Backlog Inicial
ID	História de Usuário	Prioridade	Sprint
US01	Como administrador, quero cadastrar clientes	Alta	1/2
US02	Como administrador, quero cadastrar quadras	Alta	1/2
US03	Como cliente, quero consultar quadras	Alta	2
US04	Como cliente, quero consultar horários disponíveis	Alta	3
US05	Como cliente, quero reservar uma quadra	Alta	3
US06	Como cliente, quero consultar minhas reservas	Alta	3
US07	Como cliente, quero cancelar uma reserva	Média	3
US08	Como cliente, quero realizar um pagamento	Alta	4
US09	Como administrador, quero confirmar pagamentos	Alta	4
US10	Como usuário, quero realizar login	Alta	5
US11	Como administrador, quero visualizar reservas	Alta	3/5
US12	Como administrador, quero visualizar pagamentos	Média	4
US13	Como administrador, quero visualizar indicadores	Baixa	6

🔒 Segurança
A segurança será implementada principalmente no Backend.

Recursos previstos
Autenticação baseada em JWT;

Autorização por perfil;

Proteção de endpoints;

Proteção das rotas Angular;

Validação de dados;

Senhas armazenadas de forma segura;

Controle de acesso;

Logs de operações relevantes.

Regra importante
Nunca confiar exclusivamente nas validações realizadas pelo Frontend.

Frontend
   │
   │ validação de UX
   ▼
Backend
   │
   │ validação de segurança
   │ validação de negócio
   ▼
Banco de Dados

🧪 Testes
Durante a etapa de refinamento deverão ser implementados testes para os principais fluxos.

Casos prioritários
Cadastro de cliente;

Alteração de cliente;

Inativação de cliente;

Cadastro de quadra;

Alteração de quadra;

Inativação de quadra;

Validação de CPF;

Validação de e-mail;

Criação de reserva;

Consulta de disponibilidade;

Prevenção de conflitos;

Cálculo do valor;

Cancelamento de reserva;

Registro de pagamento;

Confirmação de pagamento;

Login;

Autenticação;

Autorização.

📚 Documentação da API
A API deverá utilizar Swagger / OpenAPI para documentação.

A documentação deverá permitir:

Visualizar endpoints;

Visualizar parâmetros;

Visualizar DTOs;

Visualizar respostas;

Testar requisições;

Visualizar códigos HTTP;

Visualizar modelos de dados.

🛠️ Tecnologias
Frontend
Tecnologia	Utilização
Angular	Framework Frontend
TypeScript	Linguagem
HTML	Estrutura
CSS	Estilização

Backend
Tecnologia	Utilização
C#	Linguagem
.NET	Plataforma
ASP.NET Core	Web API
Entity Framework Core	ORM
JWT	Autenticação
Swagger / OpenAPI	Documentação

Banco de Dados
O sistema utilizará um banco de dados relacional acessado através do Entity Framework Core.

🚀 Deploy
A aplicação deverá ser preparada para execução em ambiente de produção.

                         PRODUÇÃO
                            │
             ┌──────────────┴──────────────┐
             │                             │
             ▼                             ▼
      ┌─────────────┐              ┌─────────────┐
      │   Angular   │              │ ASP.NET Core│
      │  Frontend   │◄────────────►│   Backend   │
      └─────────────┘   HTTPS      └──────┬──────┘
                                          │
                                          ▼
                                   ┌─────────────┐
                                   │   Database  │
                                   └─────────────┘

🔮 Evoluções Futuras
Após a conclusão do MVP, o sistema poderá evoluir para incluir:

🏆 Torneios;

🎾 Gestão de professores;

📚 Aulas particulares;

💳 Mensalidades;

📦 Planos de assinatura;

💰 Integração com gateways de pagamento;

📱 Integração com PIX;

📧 Notificações por e-mail;

💬 Notificações via WhatsApp;

📊 Relatórios financeiros;

📈 Relatórios de ocupação;

🎟️ Sistema de cupons;

⭐ Programa de fidelidade;

🏅 Ranking de jogadores;

📅 Gestão de horários de funcionamento;

🔧 Bloqueio de horários para manutenção;

📆 Integração com calendários externos.

🧩 Núcleo do Sistema
O núcleo do MVP será baseado nas seguintes relações:

                 ┌──────────────┐
                 │    CLIENTE   │
                 └──────┬───────┘
                        │
                        │ realiza
                        ▼
                 ┌──────────────┐
                 │   RESERVA    │
                 └──────┬───────┘
                        │
                 ┌──────┴───────┐
                 │              │
                 ▼              ▼
          ┌────────────┐  ┌────────────┐
          │   QUADRA   │  │ PAGAMENTO  │
          └────────────┘  └────────────┘

O fluxo principal será:

Cliente
   │
   ▼
Consulta quadras
   │
   ▼
Consulta disponibilidade
   │
   ▼
Seleciona horário
   │
   ▼
Cria reserva
   │
   ▼
Calcula valor
   │
   ▼
Registra pagamento
   │
   ▼
Acompanha reserva

📈 Visão Geral do MVP
┌────────────────────────────────────────────────────────┐
│                 BEACH TENNIS ARENA                     │
├────────────────────────────────────────────────────────┤
│                                                        │
│                       USUÁRIOS                         │
│                          │                             │
│             ┌────────────┴────────────┐                │
│             │                         │                │
│             ▼                         ▼                │
│         CLIENTE                  ADMINISTRADOR         │
│             │                         │                │
│      ┌──────┼──────┐           ┌──────┼──────┐         │
│      │      │      │           │      │      │         │
│      ▼      ▼      ▼           ▼      ▼      ▼         │
│   Cadastro Reserva Pagamento Clientes Quadras Reservas │
│             │                         │                │
│             └────────────┬────────────┘                │
│                          │                             │
│                          ▼                             │
│                    BANCO DE DADOS                     │
│                                                        │
└────────────────────────────────────────────────────────┘

🏁 Conclusão
O Beach Tennis Arena será uma aplicação web voltada para o gerenciamento completo das principais operações de uma arena esportiva.

O MVP terá como núcleo:

Clientes
   ↓
Quadras
   ↓
Disponibilidade
   ↓
Reservas
   ↓
Pagamentos

A arquitetura baseada em Angular + ASP.NET Core + Entity Framework Core permitirá uma separação clara entre apresentação, regras de negócio e persistência.

O desenvolvimento será realizado de forma incremental através de sprints, começando pela estrutura do projeto e banco de dados, passando pelos CRUDs e chegando aos principais fluxos de reservas, pagamentos e autenticação.

A estrutura também deverá permitir futuras extensões, como torneios, aulas, mensalidades, integração com pagamentos, notificações e relatórios.

👨‍💻 Projeto
Beach Tennis Arena

Sistema acadêmico / projeto de desenvolvimento de software para gerenciamento de uma arena de Beach Tennis.
