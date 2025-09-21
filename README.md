# PremierSoft Hackathon

## Sobre o Projeto

Este projeto é um sistema de **dashboard hospitalar** prático e fácil de usar, projetado para que gestores possam visualizar rapidamente dados importantes. A grande vantagem é que ele é **dockerizado**, o que significa que você pode rodá-lo em qualquer ambiente com o mínimo de esforço.

O dashboard é alimentado por arquivos de diversos formatos, como:

  * `.xlsx`
  * `.xml`
  * `.xls`
  * `.csv`

-----

## Pré-requisitos

Antes de começar, certifique-se de que você tem o **Docker** e o **Docker Compose** instalados na sua máquina.

  * **[Docker](https://docs.docker.com/get-docker/)**
  * **[Docker Compose](https://docs.docker.com/compose/install/)**

-----

## Como Rodar o Projeto

Siga estes passos simples para colocar o projeto no ar:

1.  **Clone o repositório** e navegue para a pasta do projeto:

    ```bash
    git clone https://github.com/seu-usuario/nome-do-projeto.git
    cd nome-do-projeto
    ```

2.  **Inicie os containers** usando o Docker Compose. A flag `-d` inicia o serviço em segundo plano:

    ```bash
    docker-compose up -d
    ```

3.  **Verifique se os containers estão rodando** corretamente:

    ```bash
    docker ps
    ```

-----

## Acessando a Aplicação

Depois de iniciar os containers, abra seu navegador e vá para o seguinte endereço:

`http://localhost:8080`

> **Nota:** A porta padrão é `8080`. Se você a modificou no arquivo `docker-compose.yml`, substitua `8080` pela porta que você configurou.

-----

## Gerenciando os Containers

Se precisar parar, reiniciar ou remover os containers, use os seguintes comandos:

  * **Para parar os containers:**

    ```bash
    docker-compose down
    ```

  * **Para parar e remover todos os containers, redes e volumes:**

    ```bash
    docker-compose down -v
    ```
