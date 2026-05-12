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
* **User and role management** backed by an access control list

## Architecture

Shipyard is comprised of three components, each with a different role.

The web UI (port `7447`) is your primary window into the application. Designed as a progressive web app, this component gives you controls for tracking your existing packages, adding new ones, and configuring carriers.

The API (port `7448`), provides access to the application's data. Built in [OData](https://www.odata.org/) style, its endpoints support filtering and sorting, in addition to standard CRUD operations. The web UI communicates directly with the API.

The background worker (headless) is not accessible directly. It authorizes against the API via an OAuth 2.0 client credentials flow. It also relies on a message broker internally for retries on failure. The worker executes carrier steps, extracts tracking data, and publishes tracking results back to the API.

## Quick Start

1. Download the newest `compose.yml` file.
2. Download the `.env.example` file and save it as `.env`. If you previously created a `.env` file, you may want to check for new variables in `.env.example`.
3. Set variables in the `.env` file as desired. At a minimum, you must set values for `SHIPYARD_WEB_LOG_APIKEY` and `SHIPYARD_WORKER_CLIENT_SECRET`.
4. Run `docker compose -f compose.yml up -d` to pull and run the application stack.
5. Access the application in a browser at http://localhost:7447, or your configured `SHIPYARD_WEB_HOST` (default admin: `admin` / `password`).
6. (Optional) Run `docker compose -f compose.yml down` to stop and remove the application stack.

> [!NOTE]
> Running `docker compose down` deletes the containers and all of their data, except the data stored in volumes. By default, Shipyard will persist data in its database to `./data` via a bind mount. While running a subsequent `docker compose up` will re-create the application, the original database data will be retained.

## Configuration

Settings can be configured by adjusting values in the `compose.yml` file or by creating a `.env` file, located in the same directory as the `compose.yml` file.

Values in `.env` will override those in `compose.yml`.

| Setting                       | Description                                             | Default                               |
| ----------------------------- | ------------------------------------------------------- | ------------------------------------- |
| POSTGRES_DB                   | The name of the PostgreSQL database.                    | shipyard                              |
| POSTGRES_USER                 | The name of the PostgreSQL user.                        | shipyard                              |
| POSTGRES_PASSWORD             | The password of the PostgreSQL user.                    | mySecure(!)Password                   |
| RABBITMQ_USER                 | The name of the RabbitMQ user.                          | shipyard                              |
| RABBITMQ_PASSWORD             | The password of the RabbitMQ user.                      | mySecure(!)Password                   |
| SHIPYARD_BOOTSTRAP_USER       | The username of the default user to create.             | admin                                 |
| SHIPYARD_BOOTSTRAP_PASSWORD   | The password of the default user to create.             | password                              |
| SHIPYARD_BOOTSTRAP_FORCE      | If set to true, the initial user will be recreated.     | false                                 |
| SHIPYARD_USE_PROXY            | Whether Shipyard is behind a reverse proxy.             | false                                 |
| SHIPYARD_API_HOST             | The base URL hosting the Shipyard API.                  | http://localhost:7448                 |
| SHIPYARD_API_PORT             | The port on which to run the Shipyard API.              | 7448                                  |
| SHIPYARD_WEB_HOST             | The base URL hosting the Shipyard web UI.               | http://localhost:7447                 |
| SHIPYARD_WEB_PORT             | The port on which to run the Shipyard web UI.           | 7447                                  |
| SHIPYARD_WEB_LOG_APIKEY       | The API key used by the web UI to post logs to the API. | (Must set this manually!)             |
| SHIPYARD_WORKER_CLIENT_ID     | The client id of the Shipyard worker, for the API.      | 608b511a-737b-4e9a-90f5-64fb86b8469f  |
| SHIPYARD_WORKER_CLIENT_SECRET | The client secret of the Shipyard worker, for the API.  | (Must set this manually!)             |
| SHIPYARD_DEFAULT_TZ           | The default IANA time zone for the app.                 | America/Chicago                       |
| SHIPYARD_ENC_TYPE             | How to load the encryption certificate.                 | PfxFile                               |
| SHIPYARD_ENC_PATH             | The path to the encryption certificate.                 | /opt/certificates/auth-encryption.pfx |
| SHIPYARD_SIGN_TYPE            | How to load the signing certificate.                    | PfxFile                               |
| SHIPYARD_SIGN_PATH            | The path to the signing certificate.                    | /opt/certificates/auth-signing.pfx    |

> [!WARNING]
> The `SHIPYARD_ENC_CRT`, `SHIPYARD_ENC_KEY`, `SHIPYARD_SIGN_CRT`, and `SHIPYARD_SIGN_KEY` environment variables have been removed.
>
> If you created a custom certificate, merge it into a `.pfx` and move it to `./data/certificates` to be able to use it. Otherwise, a certificate will be automatically generated on app startup. Custom certificates must be named `auth-encryption.pfx` and `auth-signing.pfx`, unless you modify `SHIPYARD_ENC_PATH` or `SHIPYARD_SIGN_PATH`, respectively.

## SSL / Reverse Proxy

Shipyard does not currently manage HTTPS certificates internally. It is recommended to deploy Shipyard behind a reverse proxy such as **nginx**, **Caddy**, or **Traefik** to enable SSL.

If you use a reverse proxy, you can proxy the Web UI (port `7447`) and API (port `7448`) to serve them under your own domain with HTTPS enabled. Ensure the `SHIPYARD_API_HOST` is set to match your domain.

## Status

Shipyard is currently in **Alpha**. While it is functional and stable for personal use, breaking changes may occur between releases.

It is important to grab the latest `compose.yml` file when updating to a new version.

## License

This application is licensed under the [Apache License 2.0](LICENSE).
