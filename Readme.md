# WebApiMacuco

Esta é uma API para gerenciamento de faces, desenvolvida em ASP.NET Core 8.0.302. A API inclui operações CRUD e autenticação JWT.

## Tecnologias Utilizadas

- ASP.NET Core 8.0.302
- Entity Framework Core
- SQL Server
- JWT (JSON Web Token) para autenticação
- Swagger para documentação da API

## Endpoints Disponíveis

### Autenticação

- `POST /api/UserRecords/autenticar`
  - Autentica um usuário e retorna um token JWT.
  - Exemplo de request body:
    ```json
    {
      "username": "admin",
      "password": "password"
    }
    ```

### Faces

- `POST /api/UserRecords/incluir-face`
  - Adiciona uma nova face.
  - Exige autenticação JWT.
  - Exemplo de request body:
    ```json
    {
      "user_id": 0,
      "description": "string",
      "document": "string",
      "phone": "string",
      "email": "string",
      "observation": "string",
      "base64": "string",
      "user_cod": "string",
      "birth_date": "string",
      "created_at": "2024-06-24T18:45:16.996Z",
      "updated_at": "2024-06-24T18:45:16.996Z"
    }
    ```

- `GET /api/UserRecords/listar-todas-as-faces`
  - Lista todas as faces.
  - Exige autenticação JWT.

- `GET /api/UserRecords/listar-faces-por-gestor-empresa`
  - Lista faces por gestor.
  - Exige autenticação JWT.
  - Query Parameter: `gestor`

- `POST /api/UserRecords/match-faces`
  - Verifica a correspondência de uma face.
  - Exige autenticação JWT.
  - Exemplo de request body:
    ```json
    {
      "faceTemplate": "string"
    }
    ```

## Configuração

### Clonando o Repositório

```bash
git clone https://github.com/seu-usuario/WebApiMacuco.git
cd WebApiMacuco
