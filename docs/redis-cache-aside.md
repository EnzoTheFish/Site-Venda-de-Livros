# Redis Cache-Aside no Catalog

O `Catalog.API` usa Redis via `IDistributedCache` com o padrao cache-aside:

- `GET /api/v1/products/{id}` tenta `products:{id}` no Redis.
- Em cache miss, busca no `IProductStore`, grava no Redis e retorna o produto.
- `GET /api/v1/products` usa `products:all`.
- `POST`, `PUT` e `DELETE` invalidam explicitamente as chaves afetadas nos command handlers.

## Subir ambiente

```powershell
docker-compose up --build
```

## Monitorar Redis em tempo real

```powershell
docker exec -it redis redis-cli MONITOR
```

Ao chamar duas vezes o mesmo produto, voce deve observar a primeira leitura com comandos de escrita no cache e a segunda usando a chave existente.

## Redis Insight

Acesse `http://localhost:5540` e conecte em:

- Host: `redis`
- Port: `6379`

Se estiver acessando o Redis Insight fora da rede Docker, use:

- Host: `localhost`
- Port: `6379`

## Logs de latencia

Os logs do `catalog-api` registram `Redis MISS`, `Redis HIT`, `Redis SET` e `Redis DEL`, incluindo o tempo da chamada ao cache em milissegundos.

```powershell
docker-compose logs -f catalog.api
```
