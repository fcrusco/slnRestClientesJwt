# REST Clientes + JWT (.NET 8 | VS2022)

Projeto didático de **API RESTful** com **endpoints versionados** (`/api/v1/...`) e **autenticação JWT (Bearer)**.  
Recurso único: **Cliente** (`Id`, `Nome`, `Sobrenome`). **Sem banco de dados** (armazenamento em memória).

<img width="489" height="680" alt="image" src="https://github.com/user-attachments/assets/660d8a58-c8fd-4c8a-852d-90a6b90605f9" />

---

## 🎯 Objetivos
- Demonstrar **princípios RESTful** (recursos, URIs, verbos HTTP, códigos de status, stateless).
- Expor **endpoints versionados** por rota: `v1`.
- Implementar **autenticação JWT** .
- Estruturar o projeto com **Controllers**, **Models** e **Services**.

---

## 🧱 Estrutura do Projeto
```
RestClientesJwt/
  Program.cs
  appsettings.json
  Controllers/
    v1/
      AuthController.cs
      ClientesController.cs
  Models/
    Cliente.cs
  Services/
    InMemoryStore.cs
    TokenService.cs
  appsettings.Development.json
  launchSettings.json
```

![Estrutura do projeto](./project-structure.png)

---

## 🧭 RESTful
- **Recursos & URIs estáveis:** `/api/v1/clientes`, `/api/v1/clientes/{id}`  
- **Verbos HTTP:** `GET` (ler), `POST` (criar), `PUT` (atualizar total), `DELETE` (remover)  
- **Stateless:** sem sessão de servidor; **token** enviado a cada requisição  
- **Representação:** JSON por padrão  
- **Status codes coerentes:** `200`, `201`, `204`, `400`, `401`, `404`

---

## 🔢 Versionamento de Endpoints
- Estratégia **por rota**: prefixo `/api/v1/...`  
- Facilita evolução futura (`/api/v2/...`) sem quebrar clientes existentes.

---

## 🔐 Autenticação JWT (Bearer)
1. **Login** em `POST /api/v1/auth/login` com credenciais de **DEMO**.
2. API retorna `access_token` (JWT assinado).
3. Demais endpoints exigem header:
   ```http
   Authorization: Bearer <seu_token>
   ```

> **Atenção:** credenciais/validação de login são **apenas para demo**. Em produção, valide em fonte segura (IdP/BD) e proteja a **Jwt:Key** (User Secrets / variáveis de ambiente / Key Vault).

**Credenciais de exemplo (ajuste no código conforme seu projeto):**
- `admin` / `admin`
- `teste` / `123456`

---

## 📦 Requisitos & NuGet

**Requisitos**
- .NET **8.0**
- Visual Studio **2022** (ou `dotnet CLI`)
- Postman/cURL (opcional)

**Pacotes NuGet**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (obrigatório para JWT)
- `Swashbuckle.AspNetCore` (Swagger; já vem no template Web API)

Instalação por CLI:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

## ⚙️ Configuração (`appsettings.json`)

```json
{
  "Jwt": {
    "Issuer": "RestClientesJwt.Issuer",
    "Audience": "RestClientesJwt.Audience",
    "Key": "SUA_CHAVE_SECRETA_BEM_GRANDE_AQUI_APENAS_PARA_DEMO_1234567890"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Dicas**
- Em produção, prefira **variáveis de ambiente**:
  - `Jwt__Issuer=...`
  - `Jwt__Audience=...`
  - `Jwt__Key=...`

---

## ▶️ Como Executar

### Visual Studio 2022
1. Abrir solução `RestClientesJwt`.
2. Selecionar perfil HTTPS (Kestrel/IIS Express).
3. **F5** — Swagger em `https://localhost:<porta>/swagger`.

---

## 📚 Endpoints

### 1) Autenticação
**POST** `/api/v1/auth/login`  
Request:
```json
{ "username": "admin", "password": "admin" }
```
Response `200 OK`:
```json
{
  "access_token": "<jwt>",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

### 2) Clientes (protegidos por JWT)
**Modelo**
```json
{ "id": 1, "nome": "Ana", "sobrenome": "Silva" }
```

**GET** `/api/v1/clientes` → `200 OK`  
Lista todos os clientes.

**GET** `/api/v1/clientes/{id}` → `200 OK` ou `404 Not Found`  
Obtém um cliente por id.

**POST** `/api/v1/clientes` → `201 Created` (+ header **Location**)  
Body:
```json
{ "nome": "Cecilia", "sobrenome": "Almeida" }
```

**PUT** `/api/v1/clientes/{id}` → `204 No Content` ou `404 Not Found`  
Atualização total (nome e sobrenome).

**DELETE** `/api/v1/clientes/{id}` → `204 No Content`  
Remove o cliente (idempotente).

> Header obrigatório em todos os endpoints de **Clientes**:
> ```http
> Authorization: Bearer <jwt>
> ```

---

## ✅ Códigos de Status (usados)
- **200 OK** – leitura com sucesso
- **201 Created** – criado com sucesso (`POST`) + `Location`
- **204 No Content** – operação ok sem corpo (`PUT`/`DELETE`)
- **400 Bad Request** – dados inválidos
- **401 Unauthorized** – token ausente/inválido (ou login inválido)
- **404 Not Found** – recurso não encontrado

---

## 🧩 DI (Dependency Injection) – onde usamos
- `TokenService` – **gera tokens** e valida credenciais de demo.
- `InMemoryStore` – **armazena clientes** em memória.
- Controllers recebem dependências por **construtor** (ex.: `AuthController(TokenService)`),
  o container resolve o ciclo de vida e cria instâncias conforme registrado.
  
  Injeção de dependência (DI) - Padrão de design que desacopla classes de suas dependências,
    permitindo que elas recebam as dependências de fora, em vez de criá-las internamente.
    Essa técnica resulta em um código modular, flexível, fácil de testar e manter, pois as
    classes passam a depender de abstrações (interfaces) em vez de implementações concretas.
  
---

