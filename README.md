# Postech 4NETT Hackathon

Documentação elaborada para o microsserviço de agendamentos desenvolvido no Hackathon.

## 📌 Visão Geral

Este repositório contém a implementação de um microsserviço para agendamentos, desenvolvido em **.NET 8.0** e implantado em um ambiente **Kubernetes**. O projeto faz parte de um desafio de arquitetura de microsserviços e mensageria, com foco na escalabilidade e resiliência.

## 🚀 Tecnologias Utilizadas

- **.NET 8.0** – Backend para o microsserviço de agendamentos.
- **Kubernetes** – Orquestração dos serviços.
- **PostgreSQL** – Banco de dados.
- **TestContainers** – Testes integrados.

## 📂 Estrutura do Projeto

```
Postech-4NETT-Hackathon/
│-- kubernetes/                  # Manifests para deployment no Kubernetes
│   │-- api-agendamento-deployment.yaml
│   │-- postgres-deployment.yaml
│-- postman/                     # Coleção de requisições para testes
│-- src/
│   │-- Postech.Hackathon.Agendamentos.Api/  # Código do microsserviço
│-- tests/                        # Testes automatizados
│-- README.md                     # Este documento
```

## 🏗️ Deploy no Kubernetes

1. **Criar os recursos necessários:**
   ```sh
   kubectl apply -f kubernetes/postgres-deployment.yaml
   kubectl apply -f kubernetes/api-agendamento-deployment.yaml
   ```
2. **Verificar os pods em execução:**
   ```sh
   kubectl get pods
   ```
3. **Acessar os logs da API:**
   ```sh
   kubectl logs -f <nome-do-pod>
   ```

## 🔗 Testando a API

Para testar os endpoints, utilize a coleção Postman disponível em `postman/[Postech] Hackathon - Microsserviço de Agendamento.postman_collection.json`.

## 📜 Arquitetura e outras informações

A arquitetura completa da solução está <a href="https://miro.com/app/board/uXjVLmh7kAQ=/?share_link_id=909426844610" target="_blank">disponível no Miro</a>.

---