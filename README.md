# Shipyard

> Self-hosted package tracking platform with scriptable carrier logic — built on Selenium for flexible, reliable automation.

## Features

* **Selenium-based tracking** with configurable step sequences
* **Scriptable extraction logic** using Jint and HTML Agility Pack
* **Regex-driven status rules** for mapping carrier messages to statuses
* **Per-carrier execution history**, including logs and downloadable artifacts
* **Dashboard view** for tracking package statuses at a glance
* **Email notifications** (SMTP) for out-for-delivery and delivered updates
* **Docker-first deployment** via a single `compose.yml` file

## Architecture

Shipyard is comprised of three components, each with a different role.

The web UI (port `7447`) is your primary window into the application. Designed as a progressive web app, this component gives you controls for tracking your existing packages, adding new ones, and configuring carriers.

The API (port `7448`), provides access to the application's data. Built in [OData](https://www.odata.org/) style, its endpoints support filtering and sorting, in addition to standard CRUD operations. The web UI communicates directly with the API.

The background worker (headless) is not accessible directly. It communicates with the API via a message bus and uses the bus internally for retries on failure. The worker executes carrier steps, extracts tracking data, and publishes tracking results.

## Quick Start

1. Download the `compose.yml` file.
2. Run `docker compose -f compose.yml up -d` to pull and run the application stack.
3. Access the application in a browser at http://localhost:7447, or your configured `SHIPYARD_API_HOST` (default admin: `admin` / `password`).
4. (Optional) Run `docker compose -f compose.yml down` to stop and remove the application stack.

> [!NOTE]
> Running `docker compose down` deletes the containers and all of their data, except the data stored in volumes. By default, Shipyard will persist data in its database to a volume. While running a subsequent `docker compose up` will re-create the application, the original database data will be retained.

## Configuration

Settings can be configured by adjusting values in the `compose.yml` file or by creating a `.env` file, located in the same directory as the `compose.yml` file.

Values in `.env` will override those in `compose.yml`.

| Setting                     | Description                                 | Default               |
| --------------------------- | ------------------------------------------- | --------------------- |
| POSTGRES_DB                 | The name of the PostgreSQL database.        | shipyard              |
| POSTGRES_USER               | The name of the PostgreSQL user.            | shipyard              |
| POSTGRES_PASSWORD           | The password of the PostgreSQL user.        | mySecure(!)Password   |
| RABBITMQ_USER               | The name of the RabbitMQ user.              | shipyard              |
| RABBITMQ_PASSWORD           | The password of the RabbitMQ user.          | mySecure(!)Password   |
| SHIPYARD_BOOTSTRAP_USER     | The username of the default user to create. | admin                 |
| SHIPYARD_BOOTSTRAP_PASSWORD | The password of the default user to create. | password              |
| SHIPYARD_API_HOST           | The base URL hosting Shipyard.              | http://localhost:7448 |
| SHIPYARD_API_PORT           | The port on which to run the Shipyard API.  | 7448                  |
| SHIPYARD_WEB_PORT           | The port on which to run the Shipyard UI.   | 7447                  |

> [!WARNING]
> It is **strongly recommended** to replace the default authentication certificates.
> 
> Generate two self-signed certificates and use them to set the `Security__Certificates__AuthEncryption`  and `Security__Certificates__AuthSigning` environment variables in `compose.yml`.  
> 
> Without updating these values, a malicious user familiar with this repository could craft valid authentication tokens. A future release will automate this step on first run.

## SSL / Reverse Proxy

Shipyard does not currently manage HTTPS certificates internally. It is recommended to deploy Shipyard behind a reverse proxy such as **nginx**, **Caddy**, or **Traefik** to enable SSL.

If you use nginx, you can proxy the Web UI (port `7447`) and API (port `7448`) to serve them under your own domain with HTTPS enabled. Ensure the `SHIPYARD_API_HOST` is set to match your domain.

## Status

Shipyard is currently in **Alpha**. While it is functional and stable for personal use, breaking changes may occur between releases.

It is important to grab the latest `compose.yml` file when updating to a new version.

## License

This application is licensed under the [Apache License 2.0](LICENSE).
